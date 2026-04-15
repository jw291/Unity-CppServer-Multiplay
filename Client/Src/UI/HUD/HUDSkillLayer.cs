using System.Linq;
using UnityEngine;

public class HUDSkillLayer : MonoBehaviour
{
    [SerializeField] private HUDSkillSlot[] skillSlots;
    private SkillTable skillTable => Managers.Instance.Data.SkillTable;

    void Start()
    {
        Managers.Instance.Skill.OnSkillRefresh -= OnSkillRefresh;
        Managers.Instance.Skill.OnSkillRefresh += OnSkillRefresh;
    }

    void OnDestroy()
    {
        Managers.Instance.Skill.OnSkillRefresh -= OnSkillRefresh;
    }

    private void OnSkillRefresh()
    {
        var ownedSkills = Managers.Instance.Skill.OwnedSkills.ToList();
        int count = Mathf.Min(ownedSkills.Count, skillSlots.Length);

        for (int i = 0; i < count; i++)
        {
            var skillTableData = skillTable.Get((int)ownedSkills[i].Key);
            skillSlots[i].SetData(skillTableData, ownedSkills[i].Value.SkillLevel);
        }

        for (int i = count; i < skillSlots.Length; i++)
            skillSlots[i].Clear();
    }
}