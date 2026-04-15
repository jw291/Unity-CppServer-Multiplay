using System.Collections.Generic;
using UnityEngine;

public class ShopTable
{
    private readonly Dictionary<int, ShopTableData> tableById = new();

    public void Load()
    {
        ShopTableData[] items = DataLoader.LoadTable<ShopTableData>("Data/ShopTable");
        foreach (ShopTableData data in items)
            tableById[data.shopItemId] = data;

        Debug.Log($"[ShopTable] Loaded: {items.Length} entries");
    }

    public ShopTableData Get(int shopItemId)
    {
        tableById.TryGetValue(shopItemId, out ShopTableData data);
        return data;
    }

    public IEnumerable<ShopTableData> GetAll()
    {
        return tableById.Values;
    }
}
