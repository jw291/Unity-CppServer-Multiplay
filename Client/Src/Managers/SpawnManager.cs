using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private RectTransform spawnArea;
    public RectTransform SpawnArea => spawnArea;
    [SerializeField] private MonsterDropObject dropPrefab;

    private Dictionary<int, Monster> monsters = new();
    private Dictionary<int, Vector2> deadMonsterPositions = new();
    private Dictionary<uint, MonsterDropObject> activeDrops = new();

    public void Init()
    {
        var allMonsters = Managers.Instance.Data.MonsterTable.GetAll();
        foreach (var data in allMonsters)
            Managers.Instance.Pool.Prewarm(data.GetPoolType(), data.MonsterPrefab, 20, PoolLayer.Monster);
    }

    public void StartSpawn()
    {
        RequestMonsterSpawn();
    }

    private void RequestMonsterSpawn()
    {
        Vector3[] corners = new Vector3[4];
        spawnArea.GetWorldCorners(corners);

        REQ_MONSTER_SPAWN req = new REQ_MONSTER_SPAWN();
        req.Corners.Add(corners[0].x);
        req.Corners.Add(corners[2].x);
        req.Corners.Add(corners[0].y);
        req.Corners.Add(corners[2].y);
        Managers.Instance.Network.Tcp.Send(PacketId.MSG_REQ_MONSTER_SPAWN, req);
    }

    public void PACKET_RES_MONSTER_SPAWN(RES_MONSTER_SPAWN res)
    {
        foreach (var spawnInfo in res.SpawnInfo)
        {
            var monsterData = Managers.Instance.Data.MonsterTable.Get((int)spawnInfo.MonsterId);
            Monster monster = Managers.Instance.Pool.GetPoolObject(monsterData.GetPoolType(), monsterData.MonsterPrefab, PoolLayer.Monster);
            monster.SetData(monsterData);
            monster.SetMonsterSeq(spawnInfo.MonsterSeq);
            monster.transform.position = new Vector2(spawnInfo.PosX, spawnInfo.PosY);
            monsters[spawnInfo.MonsterSeq] = monster;
            Managers.Instance.Flock.AddMonster(monster);
        }
    }

    public void PACKET_RES_MONSTER_HIT(RES_MONSTER_HIT res)
    {
        if (monsters.TryGetValue(res.MonsterSeq, out var monster))
            monster.UpdateHp(res.RemainHp);
    }

    public void PACKET_RES_MONSTER_DEAD(RES_MONSTER_DEAD res)
    {
        if (!monsters.TryGetValue(res.MonsterSeq, out var monster))
            return;

        deadMonsterPositions[res.MonsterSeq] = monster.transform.position;
        monsters.Remove(res.MonsterSeq);
        monster.Die();
    }

    public void PACKET_RES_MONSTER_MOVE(RES_MONSTER_MOVE res)
    {
        foreach (var info in res.MoveInfo)
        {
            if (monsters.TryGetValue(info.MonsterSeq, out var monster))
                monster.MoveFromServer(new Vector2(info.PosX, info.PosY), new Vector2(info.DirX, info.DirY));
        }
    }

    public void PACKET_RES_PICKUP_DROP(RES_PICKUP_DROP res)
    {
        if (!activeDrops.TryGetValue(res.DropId, out var obj))
            return;

        activeDrops.Remove(res.DropId);
        Managers.Instance.Pool.Push(PoolType.MonsterDrop, obj);
    }

    public void PACKET_RES_MONSTER_DROP(RES_MONSTER_DROP res)
    {
        if (dropPrefab == null)
        {
            Debug.LogError("[SpawnManager] dropPrefab이 할당되지 않았습니다. SpawnManager 컴포넌트에서 Drop Prefab을 연결해주세요.");
            return;
        }

        Vector2 pos;
        if (deadMonsterPositions.TryGetValue(res.MonsterSeq, out var savedPos))
        {
            pos = savedPos;
            deadMonsterPositions.Remove(res.MonsterSeq);
        }
        else
        {
            pos = new Vector2(res.PosX, res.PosY);
        }

        var obj = Managers.Instance.Pool.GetPoolObject(PoolType.MonsterDrop, dropPrefab, PoolLayer.Monster);
        obj.transform.position = pos;
        obj.Setup(res.DropId, res.Gold);
        activeDrops[res.DropId] = obj;
    }
}
