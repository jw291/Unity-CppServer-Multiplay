using System.Collections.Generic;
using UnityEngine;

public class FlockManager : FlockManagerBase
{
    private Dictionary<Monster, Vector2> lastDirections = new Dictionary<Monster, Vector2>();
    private Dictionary<Monster, Vector2> velocities = new Dictionary<Monster, Vector2>();
    private int _sendTick = 0;
    private const int SendInterval = 1;

    public override void Init() { }

    public override void SetupHost()
    {
        StartCoroutine(MoveMonsters());
    }

    public override void SetupGuest() { }

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

    private IEnumerator<WaitForFixedUpdate> MoveMonsters()
    {
        while (true)
        {
            var req = new REQ_MONSTER_MOVE();

            foreach (var monster in monsters)
            {
                if (monster == null) continue;

                if (!lastDirections.ContainsKey(monster))
                {
                    lastDirections[monster] = Vector2.zero;
                    velocities[monster] = Vector2.zero;
                }

                Vector2 balanceForce = CalculateCohesionSeparation(monster);
                if (!monsterTargets.TryGetValue(monster, out var playerTarget) || playerTarget == null)
                    continue;

                Vector2 toPlayer = (playerTarget.position - monster.transform.position).normalized;

                if (balanceForce.magnitude < 0.1f)
                    balanceForce = toPlayer;

                Vector2 moveDirection = balanceForce * (1 - cohesionSeparationBalance) + toPlayer * cohesionSeparationBalance;

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

            if (req.MoveInfo.Count > 0 && ++_sendTick >= SendInterval)
            {
                _sendTick = 0;
                Managers.Instance.Network.Udp.Send(PacketId.MSG_REQ_MONSTER_MOVE, req);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private Vector2 CalculateCohesionSeparation(Monster monster)
    {
        Vector2 cohesionForce = Vector2.zero;
        Vector2 separationForce = Vector2.zero;
        int cohesionCount = 0;
        int separationCount = 0;

        foreach (var other in monsters)
        {
            if (other == monster || other == null) continue;

            float distance = Vector2.Distance(monster.transform.position, other.transform.position);

            if (distance < balanceRadius)
            {
                cohesionForce += (Vector2)other.transform.position;
                cohesionCount++;
            }

            if (distance < balanceRadius && distance > 0f)
            {
                separationForce += (Vector2)(monster.transform.position - other.transform.position) / distance;
                separationCount++;
            }
        }

        cohesionForce = cohesionCount > 0
            ? (cohesionForce / cohesionCount - (Vector2)monster.transform.position).normalized
            : Vector2.zero;

        separationForce = separationCount > 0
            ? separationForce.normalized
            : Vector2.zero;

        return Vector2.Lerp(cohesionForce, separationForce, cohesionSeparationBalance);
    }
}
