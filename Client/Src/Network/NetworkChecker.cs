using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetworkChecker
{
    private const int checkDelay = 3;
    public static void Ping()
    {
        var req = new REQ_PING();
        _ = PingLoop();
        return;

        async Awaitable PingLoop()
        {
            while (Managers.Instance.Network.IsConnected)
            {
                req.TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                Managers.Instance.Network.Tcp.Send(PacketId.MSG_REQ_PING, req);
                await Awaitable.WaitForSecondsAsync(checkDelay);
            }
        }
    }
    
    public static void Pong(RES_PONG packet)
    {
        var nowTicks = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var diff = nowTicks - packet.TimeStamp;
        //Debug.Log($"[NetworkChecker] RTT: {diff}ms");
    }
    
}
