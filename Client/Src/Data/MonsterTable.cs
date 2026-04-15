using System.Collections.Generic;
using UnityEngine;

public class MonsterTable
{
    private readonly Dictionary<int, MonsterTableData> tableById = new Dictionary<int, MonsterTableData>();

    public void Load()
    {
        MonsterTableData[] monsters = DataLoader.LoadTable<MonsterTableData>("Data/MonsterTable");
        foreach (MonsterTableData data in monsters)
        {
            tableById[data.monsterId] = data;
        }

        Debug.Log($"[MonsterTable] Loaded: {monsters.Length} entries");
    }

    public MonsterTableData Get(int skillId)
    {
        tableById.TryGetValue(skillId, out MonsterTableData data);
        return data;
    }

    public IEnumerable<MonsterTableData> GetAll() => tableById.Values;
}
