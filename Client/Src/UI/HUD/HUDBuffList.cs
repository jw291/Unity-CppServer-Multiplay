using System.Collections.Generic;
using UnityEngine;

public class HUDBuffList : MonoBehaviour
{
    [SerializeField] private HUDBuffSlot slotPrefab;
    [SerializeField] private Transform container;

    private readonly Dictionary<string, HUDBuffSlot> activeSlots = new();

    private void Start()
    {
        Managers.Instance.Inventory.OnBuffApplied += AddBuff;
    }

    private void OnDestroy()
    {
        if (Managers.Instance != null && Managers.Instance.Inventory != null)
            Managers.Instance.Inventory.OnBuffApplied -= AddBuff;
    }

    private void AddBuff(ItemTableData item)
    {
        if (activeSlots.TryGetValue(item.effectType, out var existing) && existing.gameObject.activeSelf)
        {
            existing.SetData(item);
            return;
        }

        var slot = Instantiate(slotPrefab, container);
        slot.OnExpired += effectType => activeSlots.Remove(effectType);
        slot.SetData(item);
        activeSlots[item.effectType] = slot;
    }
}
