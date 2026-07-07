using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class FlockManagerJob : FlockManagerBase
{
    private Dictionary<Monster, Vector2> lastDirections = new Dictionary<Monster, Vector2>();
    private Dictionary<Monster, Vector2> velocities = new Dictionary<Monster, Vector2>();

    private NativeArray<float2> positions;
    private NativeArray<float2> playerPositions;
    private NativeArray<float2> results;
    private int capacity = 0;

    private JobHandle jobHandle;
    private List<Monster> scheduledMonsters = new List<Monster>();
    private bool jobScheduled = false;

    public override void Init()
    {
        enabled = false;
    }

    public override void SetupHost()
    {
        enabled = true;
    }

    public override void SetupGuest()
    {
        enabled = false;
    }

    public override void AddMonster(Monster monster)
    {
        monsters.Add(monster);
        monsterTargets[monster] = AssignTarget(monster);
    }

    public override void RemoveMonster(Monster monster)
    {
        monsters.Remove(monster);
        lastDirections.Remove(monster);
        velocities.Remove(monster);
        monsterTargets.Remove(monster);
    }

    private void EnsureCapacity(int requiredCount)
    {
        if (requiredCount <= capacity)
            return;

        if (capacity > 0)
        {
            positions.Dispose();
            playerPositions.Dispose();
            results.Dispose();
        }

        capacity = math.max(requiredCount, capacity * 2);
        positions = new NativeArray<float2>(capacity, Allocator.Persistent);
        playerPositions = new NativeArray<float2>(capacity, Allocator.Persistent);
        results = new NativeArray<float2>(capacity, Allocator.Persistent);
    }

    private void FixedUpdate()
    {
        if (jobScheduled)
        {
            jobHandle.Complete();
            ApplyResults();
            jobScheduled = false;
        }

        int count = monsters.Count;
        if (count == 0)
            return;

        EnsureCapacity(count);

        scheduledMonsters.Clear();
        scheduledMonsters.AddRange(monsters);

        for (int i = 0; i < count; i++)
        {
            var monster = scheduledMonsters[i];
            positions[i] = (Vector2)monster.transform.position;

            if (monsterTargets.TryGetValue(monster, out var target) && target != null)
                playerPositions[i] = (Vector2)target.position;
            else
                playerPositions[i] = (Vector2)monster.transform.position;
        }

        FlockJob job = new FlockJob
        {
            positions = positions,
            playerPositions = playerPositions,
            balanceRadius = balanceRadius,
            cohesionSeparationBalance = cohesionSeparationBalance,
            count = count,
            results = results
        };

        jobHandle = job.Schedule(count, 32);
        jobScheduled = true;
    }

    private void ApplyResults()
    {
        var req = new REQ_MONSTER_MOVE();

        for (int i = 0; i < scheduledMonsters.Count; i++)
        {
            Monster monster = scheduledMonsters[i];
            if (monster == null || !monster.gameObject.activeInHierarchy)
                continue;

            if (!lastDirections.ContainsKey(monster))
            {
                lastDirections[monster] = Vector2.zero;
                velocities[monster] = Vector2.zero;
            }

            Vector2 moveDirection = (Vector2)results[i];
            Vector2 velocity = velocities[monster];
            Vector2 smoothDirection = Vector2.SmoothDamp(
                lastDirections[monster], moveDirection, ref velocity, 0.3f
            );
            velocities[monster] = velocity;
            lastDirections[monster] = smoothDirection.normalized;

            monster.Move(smoothDirection.normalized * monster.Data.moveSpeed);

            var info = new MonsterMoveInfo();
            info.MonsterSeq = monster.MonsterSeq;
            info.PosX = monster.transform.position.x;
            info.PosY = monster.transform.position.y;
            info.DirX = smoothDirection.normalized.x;
            info.DirY = smoothDirection.normalized.y;
            req.MoveInfo.Add(info);
        }

        if (req.MoveInfo.Count > 0)
            Managers.Instance.Network.Udp.Send(PacketId.MSG_REQ_MONSTER_MOVE, req);
    }

    private void OnDestroy()
    {
        if (jobScheduled)
            jobHandle.Complete();

        if (capacity > 0)
        {
            positions.Dispose();
            playerPositions.Dispose();
            results.Dispose();
        }
    }
}
