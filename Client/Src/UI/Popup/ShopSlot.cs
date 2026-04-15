using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI priceText;

    private ShopItemInfo info;

    public void SetData(ShopItemInfo info)
    {
        this.info = info;

        var itemData = Managers.Instance.Data.ItemTable.Get((int)info.ItemId);
        if (itemData != null)
        {
            itemNameText.text = itemData.itemName;
            itemIcon.sprite = itemData.Icon;
        }

        priceText.text = $"{info.Price}G";
    }

    public void OnClickBuy()
    {
        if (info == null) return;
        Managers.Instance.Shop.RequestBuyItem(info.ShopItemId);
    }
}