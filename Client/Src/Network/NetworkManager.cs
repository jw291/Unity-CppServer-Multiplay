using Google.Protobuf;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private TcpTransport _tcp = new();
    private UdpTransport _udp = new();

    public TcpTransport Tcp => _tcp;
    public UdpTransport Udp => _udp;

    // 외부에서 자주 쓰는 프로퍼티는 NetworkManager에서 바로 접근 가능
    public uint AccountId => _tcp.AccountId;
    public bool IsHost => _tcp.IsHost;
    public bool IsConnected => _tcp.IsConnected;

    public void SetAccountId(uint accountId) => _tcp.SetAccountId(accountId);
    public void SetIsHost(bool value) => _tcp.SetIsHost(value);

    private void OnDestroy()
    {
        _tcp.Disconnect();
        _udp.Disconnect();
    }

    public void Init()
    {
        PacketHandler.InitHandlers();
    }

    public void Connect(string host, int tcpPort, string username)
    {
        _tcp.Connect(host, tcpPort, username);
        _udp.Connect(host, tcpPort + 1); // UDP는 TCP포트 + 1 (7778)
    }
}