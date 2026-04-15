using UnityEngine;

public class GameManager : MonoBehaviour
{
    public void Init()
    {
    }

    public void OnClickGameStart()
    {
        GameStart();
    }

    private void GameStart()
    {
        var req = new REQ_GAME_START() { HostAccountId = Managers.Instance.Network.AccountId };
        Managers.Instance.Network.Tcp.Send(PacketId.MSG_REQ_GAME_START, req);
    }

    public void PACKET_RES_GAME_START(RES_GAME_START packet)
    {
        var isHost = packet.HostAccountId == Managers.Instance.Network.AccountId;
        Managers.Instance.Network.SetIsHost(isHost);

        if (isHost)
            SetupHost();
        else
            SetupGuest();
    }

    private void SetupHost()
    {
        Managers.Instance.Flock.SetupHost();
        Managers.Instance.Spawn.StartSpawn();
    }

    private void SetupGuest()
    {
        Managers.Instance.Flock.SetupGuest();
        PacketHandler.RegisterGuestHandlers();

        var myPlayer = Managers.Instance.Player.MyPlayer;
        if (myPlayer != null)
        {
            var autoMover = myPlayer.gameObject.AddComponent<TestAutoMover>();
            autoMover.MoveArea = Managers.Instance.Spawn.SpawnArea;
        }
    }
}
