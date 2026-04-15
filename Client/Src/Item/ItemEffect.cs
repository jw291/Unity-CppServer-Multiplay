using UnityEngine;

public static class ItemEffect
{
    public static bool Apply(ItemTableData item)
    {
        var player = Managers.Instance.Player.MyPlayer;
        if (player == null) return false;

        var itemType = (ItemType)item.itemId;

        var buff = BuffRegistry.Get(itemType);
        if (buff != null)
        {
            buff.Apply(player, item.effectValue, item.duration);
            return true;
        }

        ApplyConsumable(player, itemType, item);
        return false;
    }

    private static void ApplyConsumable(MyPlayer player, ItemType itemType, ItemTableData item)
    {
        switch (itemType)
        {
            case ItemType.HpPotion:
                player.HealHp(item.effectValue);
                break;
            default:
                Debug.LogWarning($"[ItemEffect] Unknown itemType: {itemType}");
                break;
        }
    }
}
