using UnityEngine;

public class HUD : PopupBase
{
    public void OnClickSkillPopup()
    {
        Managers.Instance.UI.Open<SkillPopup>();
    }

    public void OnClickShopPopup()
    {
        Managers.Instance.UI.Open<ShopPopup>();
    }

    public void OnClickInventoryPopup()
    {
        Managers.Instance.UI.Open<InventoryPopup>();
    }
}
