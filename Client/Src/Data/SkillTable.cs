using System.Collections.Generic;
using UnityEngine;

public class SkillTable
{
    private readonly Dictionary<int, SkillTableData> tableById = new Dictionary<int, SkillTableData>();

    public void Load()
    {
        SkillTableData[] skills = DataLoader.LoadTable<SkillTableData>("Data/SkillTable");
        foreach (SkillTableData data in skills)
        {
            tableById[data.skillId] = data;
        }

        Debug.Log($"[SkillTable] Loaded: {skills.Length} entries");
    }

    public SkillTableData Get(int skillId)
    {
        tableById.TryGetValue(skillId, out SkillTableData data);
        return data;
    }

    public IEnumerable<SkillTableData> GetAll()
    {
        return tableById.Values;
    }
}
