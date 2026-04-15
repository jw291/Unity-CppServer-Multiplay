using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Google.Protobuf;
using UnityEngine;

public class TcpTransport : ITransport
{
    private TcpClient client;
    private NetworkStream stream;
    private bool isConnected;

    public bool IsConnected => isConnected;
    public uint AccountId { get; private set; }
    public bool IsHost { get; private set; }

    public void SetAccountId(uint accountId) { AccountId = accountId; }
    public void SetIsHost(bool value) { IsHost = value; }

    public async void Connect(string host, int port, string username)
    {
        try
        {
            client = new TcpClient();
            await client.ConnectAsync(host, port);
            stream = client.GetStream();
            isConnected = true;

            RequestLogin(username);
            Debug.Log($"[TCP] Connected to {host}:{port}");
            _ = ReceiveLoop();
        }
        catch (Exception e)
        {
            Debug.LogError($"[TCP] Connect failed: {e.Message}");
        }
    }

    public void Send(PacketId msgId, IMessage message)
    {
        if (!isConnected) 
            return;
        try
        {
            var body = message.ToByteArray();
            var packet = PacketHeader.Serialize((ushort)msgId, body);
            stream.Write(packet, 0, packet.Length);
        }
        catch (Exception e)
        {
            Debug.LogError($"[TCP] Send error: {e.Message}");
            Disconnect();
        }
    }

    public void Disconnect()
    {
        isConnected = false;
        stream?.Close();
        client?.Close();
        Debug.Log("[TCP] Disconnected");
    }

    private async Task ReceiveLoop()
    {
        var headerBuf = new byte[PacketHeader.SIZE];
        try
        {
            while (isConnected)
            {
                await ReadExactlyAsync(headerBuf, PacketHeader.SIZE);
                var (msgId, size) = PacketHeader.Deserialize(headerBuf);

                var bodyBuf = new byte[size];
                await ReadExactlyAsync(bodyBuf, (int)size);

                PacketHandler.Handle((PacketId)msgId, bodyBuf);
            }
        }
        catch (Exception e)
        {
            if (isConnected)
                Debug.LogError($"[TCP] Receive error: {e.Message}");
            Disconnect();
        }
    }

    private async Task ReadExactlyAsync(byte[] buffer, int size)
    {
        int totalRead = 0;
        while (totalRead < size)
        {
            int read = await stream.ReadAsync(buffer, totalRead, size - totalRead);
            if (read == 0)
                throw new Exception("Connection closed by server");
            totalRead += read;
        }
    }

    private void RequestLogin(string username)
    {
        Send(PacketId.MSG_REQ_LOGIN, new REQ_LOGIN { Username = username });
    }
}