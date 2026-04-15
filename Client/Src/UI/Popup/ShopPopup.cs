using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopPopup : PopupBase
{
    [SerializeField] private ShopSlot slotPrefab;
    [SerializeField] private Transform slotParent;
    [SerializeField] private TextMeshProUGUI goldText;

    private readonly List<ShopSlot> slots = new();

    public override void Open()
    {
        base.Open();
        Managers.Instance.Shop.OnShopRefresh += Refresh;
        Managers.Instance.Shop.OnGoldUpdated += RefreshGold;
        Managers.Instance.Shop.RequestShopInfo();
    }

    public override void Close()
    {
        Managers.Instance.Shop.OnShopRefresh -= Refresh;
        Managers.Instance.Shop.OnGoldUpdated -= RefreshGold;
        base.Close();
    }

    private void Refresh()
    {
        var items = Managers.Instance.Shop.ShopItems;

        int index = 0;
        foreach (var info in items)
        {
            ShopSlot slot;
            if (index < slots.Count)
                slot = slots[index];
            else
            {
                slot = Instantiate(slotPrefab, slotParent);
                slots.Add(slot);
            }

            slot.SetData(info);
            slot.gameObject.SetActive(true);
            index++;
        }

        for (int i = index; i < slots.Count; i++)
            slots[i].gameObject.SetActive(false);

        RefreshGold(Managers.Instance.Shop.Gold);
    }

    private void RefreshGold(uint gold)
    {
        goldText.text = $"Gold: {gold}G";
    }

    public void OnClickClose() => Close();
}