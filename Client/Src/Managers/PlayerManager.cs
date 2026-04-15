using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform root;
    public Player myPlayer;
    public MyPlayer MyPlayer => myPlayer as MyPlayer;
    public Dictionary<uint, Player> remotePlayers = new();

    public event Action<int, int> OnMyPlayerHpChanged; // (currentHp, maxHp)

    public void Init() { }

    public void CreateMyPlayer()
    {
        var player = GameObject.Instantiate(playerPrefab, root);
        var myPlayer = player.GetOrAddComponent<MyPlayer>();
        myPlayer.SetAccountId(Managers.Instance.Network.AccountId);

        var tableData = Managers.Instance.Data.PlayerTable.Get(1);
        if (tableData != null)
            myPlayer.SetPlayerData(tableData);

        myPlayer.OnHpChanged += (hp, maxHp) => OnMyPlayerHpChanged?.Invoke(hp, maxHp);
        this.myPlayer = myPlayer;
    }

    public void CreateRemotePlayer(uint accountId)
    {
        var player = GameObject.Instantiate(playerPrefab, root);
        var remotePlayer = player.GetOrAddComponent<RemotePlayer>();
        remotePlayer.SetAccountId(accountId);
        remotePlayer.Create(root);
        remotePlayers.Add(accountId, remotePlayer);
    }

    public void DestroyPlayer(uint accountId)
    {
        if (Managers.Instance.Network.AccountId == accountId)
            myPlayer.Destroy();
        else
        {
            if (remotePlayers.TryGetValue(accountId, out var player))
            {
                player.Destroy();
                remotePlayers.Remove(accountId);
            }
        }
    }

    public void PACKET_RES_MOVE(RES_PLAYER_MOVE packet)
    {
        if (!remotePlayers.TryGetValue(packet.AccountId, out var player))
            return;

        Vector3 targetPos = new Vector3(packet.PosX, packet.PosY, 0f);
        Vector2 dir = new Vector2(packet.DirX, packet.DirY);
        ((RemotePlayer)player).SetMoveData(targetPos, dir, packet.MoveSpeed);
    }

    public void PACKET_RES_PLAYER_HIT(RES_PLAYER_HIT res)
    {
        if (res.AccountId == Managers.Instance.Network.AccountId)
            MyPlayer?.TakeDamage(res.RemainHp);
        else if (remotePlayers.TryGetValue(res.AccountId, out var player))
            player.TakeDamage(res.RemainHp);
    }
}
