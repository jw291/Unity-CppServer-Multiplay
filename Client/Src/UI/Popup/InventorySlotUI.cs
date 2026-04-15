using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI quantityText;

    private uint slotIndex;

    public void Setup(uint slotIndex, InventorySlot slot)
    {
        this.slotIndex = slotIndex;

        var itemData = Managers.Instance.Data.ItemTable.Get((int)slot.ItemId);
        if (itemData != null)
        {
            itemIcon.sprite = itemData.Icon;
            itemIcon.enabled = true;
        }

        quantityText.text = slot.Quantity > 1 ? slot.Quantity.ToString() : "";
    }

    public void SetEmpty()
    {
        itemIcon.enabled = false;
        quantityText.text = "";
    }

    public void OnClickUse()
    {
        Managers.Instance.Inventory.RequestUseItem(slotIndex);
    }
}