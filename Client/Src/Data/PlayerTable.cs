using System.Collections.Generic;
using UnityEngine;

public class PlayerTable
{
    private readonly Dictionary<int, PlayerTableData> _tableById = new();

    public void Load()
    {
        PlayerTableData[] entries = DataLoader.LoadTable<PlayerTableData>("Data/PlayerTable");
        foreach (var data in entries)
            _tableById[data.playerId] = data;

        Debug.Log($"[PlayerTable] Loaded: {entries.Length} entries");
    }

    public PlayerTableData Get(int playerId)
    {
        _tableById.TryGetValue(playerId, out var data);
        return data;
    }
}
