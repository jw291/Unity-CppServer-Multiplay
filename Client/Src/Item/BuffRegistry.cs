using System.Collections.Generic;

public static class BuffRegistry
{
    private static readonly Dictionary<ItemType, Buff> _map = new()
    {
        { ItemType.AttackBoost,  new AtkBoostBuff() },
        { ItemType.SpeedBoost,   new SpeedBoostBuff() },
        { ItemType.DefenseBoost, new DefenseBoostBuff() },
    };

    public static Buff Get(ItemType itemType)
    {
        _map.TryGetValue(itemType, out var buff);
        return buff;
    }
}
