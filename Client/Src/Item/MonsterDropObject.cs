using UnityEngine;

public class MonsterDropObject : MonoBehaviour, IPoolableObject
{
    private uint dropId;
    private uint gold;
    private bool pickedUp;

    public void Activate()
    {
        pickedUp = false;
    }

    public void Setup(uint dropId, uint gold)
    {
        this.dropId = dropId;
        this.gold = gold;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickedUp)
            return;
        if (!other.TryGetComponent<MyPlayer>(out _))
            return;

        pickedUp = true;
        Managers.Instance.Network.Tcp.Send(PacketId.MSG_REQ_PICKUP_DROP,
            new REQ_PICKUP_DROP { DropId = dropId });
    }
}
