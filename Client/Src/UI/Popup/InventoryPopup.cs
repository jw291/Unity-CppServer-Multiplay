using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryPopup : PopupBase
{
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private Transform slotParent;
    [SerializeField] private int slotCount;
    [SerializeField] private TextMeshProUGUI currencyText;

    private readonly List<InventorySlotUI> _slots = new();

    public override void Open()
    {
        base.Open();
        CreateSlots();
        Managers.Instance.Inventory.OnInventoryRefresh += Refresh;
        Managers.Instance.Shop.OnGoldUpdated += RefreshGold;
        Managers.Instance.Inventory.RequestInventoryInfo();
        RefreshGold(Managers.Instance.Shop.Gold);
    }

    public override void Close()
    {
        Managers.Instance.Inventory.OnInventoryRefresh -= Refresh;
        Managers.Instance.Shop.OnGoldUpdated -= RefreshGold;
        base.Close();
    }

    private void CreateSlots()
    {
        for (int i = _slots.Count; i < slotCount; i++)
        {
            var slot = Instantiate(slotPrefab, slotParent);
            _slots.Add(slot);
            slot.gameObject.SetActive(true);
        }
    }

    private void Refresh()
    {
        var slots = Managers.Instance.Inventory.Slots;
        var items = new List<(uint key, InventorySlot slot)>();
        foreach (var pair in slots)
            items.Add((pair.Key, pair.Value));
        items.Sort((a, b) => a.key.CompareTo(b.key));

        for (int i = 0; i < _slots.Count; i++)
        {
            if (i < items.Count)
                _slots[i].Setup(items[i].key, items[i].slot);
            else
                _slots[i].SetEmpty();
        }
    }

    private void RefreshGold(uint gold)
    {
        if (currencyText != null)
            currencyText.text = $"Gold: {gold}";
    }

    public void OnClickClose() => Close();
}