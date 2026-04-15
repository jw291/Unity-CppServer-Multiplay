using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    [SerializeField] private Image skillImage;
    [SerializeField] private TextMeshProUGUI skillLevel;

    private SkillTableData data;

    public void SetData(SkillTableData data)
    {
        this.data = data;
        RefreshUI();
    }

    public void RefreshUI()
    {
        RefreshSkillIcon();
        RefreshSkillLevel();
    }

    private void RefreshSkillIcon()
    {
        if (data == null)
            return;
        skillImage.sprite = data.Icon;
    }

    private void RefreshSkillLevel()
    {
        if (data == null)
            return;

        var ownedSkills = Managers.Instance.Skill.OwnedSkills;
        var level = ownedSkills.TryGetValue(data.skillId, out var skill) ? skill.SkillLevel : 0;
        skillLevel.text = $"Lv.{level}";
    }

    public void OnClickPlus()
    {
        if (data == null)
            return;
        Managers.Instance.Skill.RequestSkillAdd((uint)data.skillId);
    }

    public void OnClickMinus()
    {
        if (data == null)
            return;
        Managers.Instance.Skill.RequestSkillSubtract((uint)data.skillId);
    }
}