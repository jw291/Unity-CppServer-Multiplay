using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillPopup : PopupBase
{
    [SerializeField] private SkillSlot skillSlotPrefab;
    [SerializeField] private Transform skillSlotParent;

    private readonly List<SkillSlot> slots = new List<SkillSlot>();
    private SkillTable skillTable => Managers.Instance.Data.SkillTable;

    public override void Open()
    {
        base.Open();
        CreateSlots();
        Managers.Instance.Skill.OnSkillRefresh += OnSkillRefresh;
        Managers.Instance.Skill.RequestSkillInfo();
    }

    public override void Close()
    {
        Managers.Instance.Skill.OnSkillRefresh -= OnSkillRefresh;
        base.Close();
    }

    private void CreateSlots()
    {
        int index = 0;
        foreach (SkillTableData data in skillTable.GetAll())
        {
            SkillSlot slot;
            if (index < slots.Count)
            {
                slot = slots[index];
            }
            else
            {
                slot = Instantiate(skillSlotPrefab, skillSlotParent);
                slots.Add(slot);
            }

            slot.SetData(data);
            slot.gameObject.SetActive(true);
            index++;
        }

        for (int i = index; i < slots.Count; i++)
            slots[i].gameObject.SetActive(false);
    }

    private void OnSkillRefresh()
    {
        foreach (SkillSlot slot in slots)
        {
            slot.RefreshUI();
        }
    }
}