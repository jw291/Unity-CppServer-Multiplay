using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private readonly Dictionary<uint, InventorySlot> slots = new();
    public IReadOnlyDictionary<uint, InventorySlot> Slots => slots;

    public event Action OnInventoryRefresh;
    public event Action<ItemTableData> OnBuffApplied;

    public void Init() { }

    public void RequestInventoryInfo()
    {
        Managers.Instance.Network.Tcp.Send(PacketId.MSG_REQ_INVENTORY_INFO, new REQ_INVENTORY_INFO());
    }

    public void RequestUseItem(uint slotIndex)
    {
        Managers.Instance.Network.Tcp.Send(PacketId.MSG_REQ_USE_ITEM,
            new REQ_USE_ITEM { SlotIndex = slotIndex });
    }

    public void PACKET_RES_INVENTORY_INFO(RES_INVENTORY_INFO res)
    {
        slots.Clear();
        foreach (var slot in res.Slots)
            slots[slot.SlotIndex] = slot;
        OnInventoryRefresh?.Invoke();
    }

    public void PACKET_RES_USE_ITEM(RES_USE_ITEM res)
    {
        if (!res.Success)
        {
            Debug.LogWarning("[InventoryManager] UseItem failed");
            return;
        }

        if (slots.TryGetValue(res.SlotIndex, out var slot))
        {
            if (slot.Quantity > 1)
            {
                slot.Quantity--;
            }
            else
            {
                slots.Remove(res.SlotIndex);
            }
        }
        OnInventoryRefresh?.Invoke();

        var itemData = Managers.Instance.Data.ItemTable.Get((int)res.ItemId);
        if (itemData == null) return;

        bool isBuff = ItemEffect.Apply(itemData);
        if (isBuff)
            OnBuffApplied?.Invoke(itemData);
    }
}
