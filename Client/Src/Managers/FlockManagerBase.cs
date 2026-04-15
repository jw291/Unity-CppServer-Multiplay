using System.Collections.Generic;
using UnityEngine;

public abstract class FlockManagerBase : MonoBehaviour
{
    public float balanceRadius = 5f;

    [Range(0f, 1f)]
    public float cohesionSeparationBalance = 0.5f;

    protected List<Monster> monsters = new List<Monster>();
    protected Dictionary<Monster, Transform> monsterTargets = new Dictionary<Monster, Transform>();
    private int assignIndex = 0;

    public int MonsterCount => monsters.Count;

    public abstract void SetupHost();
    public abstract void SetupGuest();

    protected Transform AssignTarget(Monster monster)
    {
        var playerManager = Managers.Instance.Player;
        var players = new List<Transform>();

        if (playerManager.myPlayer != null)
            players.Add(playerManager.myPlayer.transform);

        foreach (var remote in playerManager.remotePlayers.Values)
        {
            if (remote != null)
                players.Add(remote.transform);
        }

        if (players.Count == 0)
            return null;

        var target = players[assignIndex % players.Count];
        assignIndex++;
        return target;
    }

    public abstract void Init();
    public abstract void AddMonster(Monster monster);
    public abstract void RemoveMonster(Monster monster);
}
