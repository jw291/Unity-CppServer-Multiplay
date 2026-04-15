using System;
using System.Collections.Generic;
using Google.Protobuf;
using UnityEngine;

public static class PacketHandler
{
    private static readonly Dictionary<PacketId, Action<byte[]>> handlers = new();
    private static readonly Dictionary<PacketId, Action<byte[]>> listenerFireMap = new();

    public static void InitHandlers()
    {
        Regist<RES_ECHO>(PacketId.MSG_RES_ECHO, HandleEcho);
        Regist<RES_LOGIN>(PacketId.MSG_RES_LOGIN, HandleLogin);
        Regist<RES_PONG>(PacketId.MSG_RES_PONG, HandlePong);
        Regist<RES_NOTIFY_ENTER>(PacketId.MSG_RES_NOTFIY_ENTER, HandleEnter);
        Regist<RES_NOTIFY_LEAVE>(PacketId.MSG_RES_NOTFIY_LEAVE, HandleLeave);
        Regist<RES_PLAYER_MOVE>(PacketId.MSG_RES_MOVE, HandleMove);
        Regist<RES_SKILL_INFO>(PacketId.MSG_RES_SKILL_INFO, HandleSkillInfo);
        Regist<RES_SKILL_ADD>(PacketId.MSG_RES_SKILL_ADD, HandleSkillAdd);
        Regist<RES_SKILL_SUBTRACT>(PacketId.MSG_RES_SKILL_SUBTRACT, HandleSkillSubtract);
        Regist<RES_SKILL_FIRE>(PacketId.MSG_RES_SKILL_FIRE, HandleSkillFire);
        Regist<RES_MONSTER_SPAWN>(PacketId.MSG_RES_MONSTER_SPAWN, HandleMonsterSpawn);
        Regist<RES_MONSTER_HIT>(PacketId.MSG_RES_MONSTER_HIT, HandleMonsterHit);
        Regist<RES_MONSTER_DEAD>(PacketId.MSG_RES_MONSTER_DEAD, HandleMonsterDead);
        Regist<RES_GAME_START>(PacketId.MSG_RES_GAME_START, HandleGameStart);
        Regist<RES_NOTIFY_SKILL_START>(PacketId.MSG_RES_SKILL_START, HandleSkillStart);
        Regist<RES_SHOP_INFO>(PacketId.MSG_RES_SHOP_INFO, HandleShopInfo);
        Regist<RES_BUY_ITEM>(PacketId.MSG_RES_BUY_ITEM, HandleBuyItem);
        Regist<RES_INVENTORY_INFO>(PacketId.MSG_RES_INVENTORY_INFO, HandleInventoryInfo);
        Regist<RES_USE_ITEM>(PacketId.MSG_RES_USE_ITEM, HandleUseItem);
        Regist<RES_MONSTER_DROP>(PacketId.MSG_RES_MONSTER_DROP, HandleMonsterDrop);
        Regist<RES_CURRENCY_UPDATE>(PacketId.MSG_RES_CURRENCY_UPDATE, HandleCurrencyUpdate);
        Regist<RES_PLAYER_HIT>(PacketId.MSG_RES_PLAYER_HIT, HandlePlayerHit);
        Regist<RES_PICKUP_DROP>(PacketId.MSG_RES_PICKUP_DROP, HandlePickupDrop);
    }

    private static void Regist<T>(PacketId packetId, Action<byte[]> handler) where T : IMessage<T>, new()
    {
        handlers[packetId] = handler;
        listenerFireMap[packetId] = (body) => PacketListener<T>.Fire(body);
    }

    public static void Handle(PacketId msgId, byte[] body)
    {
        if (handlers.TryGetValue(msgId, out var handler))
            handler.Invoke(body);
        else
            Debug.LogWarning($"[PacketHandler] Unknown msgId: {msgId}");

        if (listenerFireMap.TryGetValue(msgId, out var fire))
            fire.Invoke(body);
    }

    private static void HandleLogin(byte[] body)
    {
        var res = RES_LOGIN.Parser.ParseFrom(body);
        if (!res.Success)
        {
            Debug.LogWarning("[PacketHandler] Login failed");
            return;
        }

        Managers.Instance.Network.SetAccountId(res.AccountId);
        Managers.Instance.Network.Udp.Send(PacketId.MSG_REQ_UDP_REGISTER,
            new REQ_UDP_REGISTER { AccountId = res.AccountId });
        Managers.Instance.Player.CreateMyPlayer();
        Managers.Instance.UI.Close<LoginPopup>();
        Managers.Instance.Skill.RequestSkillInfo();
        
        Debug.Log($"접속 : {res.Success}");
    }

    private static void HandleEnter(byte[] body)
    {
        var res = RES_NOTIFY_ENTER.Parser.ParseFrom(body);
        if (res.AccountId != Managers.Instance.Network.AccountId)
            Managers.Instance.Player.CreateRemotePlayer(res.AccountId);
        Debug.Log($"[HandleEnter] : {res.AccountId} 가 입장했습니다.");
    }

    private static void HandleEcho(byte[] body)
    {
        var res = RES_ECHO.Parser.ParseFrom(body);
        Debug.Log($"[HandleEchoRes] : {res.Text}");
    }

    private static void HandlePong(byte[] body)
    {
        var res = RES_PONG.Parser.ParseFrom(body);
        NetworkChecker.Pong(res);
    }

    private static void HandleLeave(byte[] body)
    {
        var res = RES_NOTIFY_LEAVE.Parser.ParseFrom(body);
        Managers.Instance.Player.DestroyPlayer(res.AccountId);
        Debug.Log($"[HandleLeave] : {res.AccountId} 가 퇴장했습니다.");
    }

    private static void HandleMove(byte[] body)
    {
        var res = RES_PLAYER_MOVE.Parser.ParseFrom(body);
        Managers.Instance.Player.PACKET_RES_MOVE(res);
    }

    private static void HandleSkillInfo(byte[] body)
    {
        var res = RES_SKILL_INFO.Parser.ParseFrom(body);
        Managers.Instance.Skill.PACKET_RES_SKILL_INFO(res);
    }

    private static void HandleSkillAdd(byte[] body)
    {
        var res = RES_SKILL_ADD.Parser.ParseFrom(body);
        Managers.Instance.Skill.PACKET_RES_SKILL_ADD(res);
    }

    private static void HandleSkillSubtract(byte[] body)
    {
        var res = RES_SKILL_SUBTRACT.Parser.ParseFrom(body);
        Managers.Instance.Skill.PACKET_RES_SKILL_SUBTRACT(res);
    }

    private static void HandleSkillFire(byte[] body)
    {
        var res = RES_SKILL_FIRE.Parser.ParseFrom(body);
        if (res.AccountId == Managers.Instance.Network.AccountId) 
            return;
        Managers.Instance.Skill.PACKET_RES_SKILL_FIRE(res);
    }

    private static void HandleMonsterSpawn(byte[] body)
    {
        var res = RES_MONSTER_SPAWN.Parser.ParseFrom(body);
        Managers.Instance.Spawn.PACKET_RES_MONSTER_SPAWN(res);
    }

    private static void HandleMonsterHit(byte[] body)
    {
        var res = RES_MONSTER_HIT.Parser.ParseFrom(body);
        Managers.Instance.Spawn.PACKET_RES_MONSTER_HIT(res);
    }

    private static void HandleMonsterDead(byte[] body)
    {
        var res = RES_MONSTER_DEAD.Parser.ParseFrom(body);
        Managers.Instance.Spawn.PACKET_RES_MONSTER_DEAD(res);
    }

    public static void RegisterGuestHandlers()
    {
        Regist<RES_MONSTER_MOVE>(PacketId.MSG_RES_MONSTER_MOVE, HandleMonsterMove);
    }

    private static void HandleMonsterMove(byte[] body)
    {
        var res = RES_MONSTER_MOVE.Parser.ParseFrom(body);
        Managers.Instance.Spawn.PACKET_RES_MONSTER_MOVE(res);
    }

    private static void HandleGameStart(byte[] body)
    {
        var res = RES_GAME_START.Parser.ParseFrom(body);
        Managers.Instance.Game.PACKET_RES_GAME_START(res);
    }

    private static void HandleSkillStart(byte[] body)
    {
        Managers.Instance.Skill.ExecuteSkills();
    }

    private static void HandleShopInfo(byte[] body)
    {
        var res = RES_SHOP_INFO.Parser.ParseFrom(body);
        Managers.Instance.Shop.PACKET_RES_SHOP_INFO(res);
    }

    private static void HandleBuyItem(byte[] body)
    {
        var res = RES_BUY_ITEM.Parser.ParseFrom(body);
        Managers.Instance.Shop.PACKET_RES_BUY_ITEM(res);
    }

    private static void HandleInventoryInfo(byte[] body)
    {
        var res = RES_INVENTORY_INFO.Parser.ParseFrom(body);
        Managers.Instance.Inventory.PACKET_RES_INVENTORY_INFO(res);
    }

    private static void HandleUseItem(byte[] body)
    {
        var res = RES_USE_ITEM.Parser.ParseFrom(body);
        Managers.Instance.Inventory.PACKET_RES_USE_ITEM(res);
    }

    private static void HandleMonsterDrop(byte[] body)
    {
        var res = RES_MONSTER_DROP.Parser.ParseFrom(body);
        Managers.Instance.Spawn.PACKET_RES_MONSTER_DROP(res);
    }

    private static void HandleCurrencyUpdate(byte[] body)
    {
        var res = RES_CURRENCY_UPDATE.Parser.ParseFrom(body);
        Managers.Instance.Shop.PACKET_RES_CURRENCY_UPDATE(res);
    }

    private static void HandlePlayerHit(byte[] body)
    {
        var res = RES_PLAYER_HIT.Parser.ParseFrom(body);
        Managers.Instance.Player.PACKET_RES_PLAYER_HIT(res);
    }

    private static void HandlePickupDrop(byte[] body)
    {
        var res = RES_PICKUP_DROP.Parser.ParseFrom(body);
        Managers.Instance.Spawn.PACKET_RES_PICKUP_DROP(res);
    }
}
