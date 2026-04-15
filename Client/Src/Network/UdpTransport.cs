using System;
using System.Net.Sockets;
using Google.Protobuf;
using UnityEngine;

public class UdpTransport : ITransport
{
    private UdpClient _client;
    private bool isConnected;

    public bool IsConnected => isConnected;

    public void Connect(string host, int port)
    {
        try
        {
            _client = new UdpClient();
            _client.Connect(host, port);
            isConnected = true;

            Debug.Log($"[UDP] Ready to {host}:{port}");
            _ = ReceiveLoop();
        }
        catch (Exception e)
        {
            Debug.LogError($"[UDP] Connect failed: {e.Message}");
        }
    }

    public void Send(PacketId msgId, IMessage message)
    {
        if (!isConnected) return;
        try
        {
            var body = message.ToByteArray();
            var packet = PacketHeader.Serialize((ushort)msgId, body);
            _ = _client.SendAsync(packet, packet.Length);
        }
        catch (Exception e)
        {
            Debug.LogError($"[UDP] Send error: {e.Message}");
        }
    }

    public void Disconnect()
    {
        isConnected = false;
        _client?.Close();
        Debug.Log("[UDP] Disconnected");
    }

    private async System.Threading.Tasks.Task ReceiveLoop()
    {
        try
        {
            while (isConnected)
            {
                var result = await _client.ReceiveAsync();
                var data = result.Buffer;

                if (data.Length < PacketHeader.SIZE) continue;

                var (msgId, size) = PacketHeader.Deserialize(data);
                var body = new byte[size];
                Array.Copy(data, PacketHeader.SIZE, body, 0, size);

                PacketHandler.Handle((PacketId)msgId, body);
            }
        }
        catch (Exception e)
        {
            if (isConnected)
                Debug.LogError($"[UDP] Receive error: {e.Message}");
        }
    }
}