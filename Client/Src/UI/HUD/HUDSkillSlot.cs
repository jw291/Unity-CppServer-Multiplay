using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDSkillSlot : MonoBehaviour
{
    [SerializeField] private Image skillImage;
    [SerializeField] private TextMeshProUGUI skillLevel;

    private SkillTableData data;
    private int level;

    public void SetData(SkillTableData data, int level)
    {
        this.data = data;
        this.level = level;
        RefreshUI();
    }

    public void Clear()
    {
        skillImage.gameObject.SetActive(false);
        skillLevel.gameObject.SetActive(false);
    }

    public void RefreshUI()
    {
        RefreshSkillIcon();
        RefreshSkillLevel();
    }

    private void RefreshSkillIcon()
    {
        if (data == null)
        {
            skillImage.gameObject.SetActive(false);
            return;
        }

        skillImage.gameObject.SetActive(true);
        skillImage.sprite = data.Icon;
    }

    private void RefreshSkillLevel()
    {
        if (data == null)
        {
            skillLevel.gameObject.SetActive(false);
            return;
        }

        skillLevel.gameObject.SetActive(true);
        skillLevel.text = $"Lv.{level}";
    }
}