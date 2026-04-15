using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    private uint gold;
    private readonly List<ShopItemInfo> _shopItems = new();

    public uint Gold => gold;
    public IReadOnlyList<ShopItemInfo> ShopItems => _shopItems;

    public event Action OnShopRefresh;
    public event Action<uint> OnGoldUpdated;

    public void Init() { }

    public void RequestShopInfo()
    {
        Managers.Instance.Network.Tcp.Send(PacketId.MSG_REQ_SHOP_INFO, new REQ_SHOP_INFO());
    }

    public void RequestBuyItem(uint shopItemId)
    {
        Managers.Instance.Network.Tcp.Send(PacketId.MSG_REQ_BUY_ITEM,
            new REQ_BUY_ITEM { ShopItemId = shopItemId });
    }

    public void PACKET_RES_SHOP_INFO(RES_SHOP_INFO res)
    {
        gold = res.Gold;
        _shopItems.Clear();
        _shopItems.AddRange(res.Items);
        OnShopRefresh?.Invoke();
    }

    public void PACKET_RES_BUY_ITEM(RES_BUY_ITEM res)
    {
        if (!res.Success)
        {
            Debug.LogWarning("[ShopManager] BuyItem failed");
            return;
        }
        gold = res.RemainGold;
        OnGoldUpdated?.Invoke(gold);
        Managers.Instance.Inventory.RequestInventoryInfo();
    }

    public void PACKET_RES_CURRENCY_UPDATE(RES_CURRENCY_UPDATE res)
    {
        gold = res.Gold;
        OnGoldUpdated?.Invoke(gold);
    }
}
