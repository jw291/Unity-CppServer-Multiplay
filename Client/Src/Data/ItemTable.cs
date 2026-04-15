using System.Collections.Generic;
using UnityEngine;

public class ItemTable
{
    private readonly Dictionary<int, ItemTableData> tableById = new();

    public void Load()
    {
        ItemTableData[] items = DataLoader.LoadTable<ItemTableData>("Data/ItemTable");
        foreach (ItemTableData data in items)
            tableById[data.itemId] = data;

        Debug.Log($"[ItemTable] Loaded: {items.Length} entries");
    }

    public ItemTableData Get(int itemId)
    {
        tableById.TryGetValue(itemId, out ItemTableData data);
        return data;
    }

    public IEnumerable<ItemTableData> GetAll()
    {
        return tableById.Values;
    }
}
