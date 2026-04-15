using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHeadOnUI : MonoBehaviour
{
    [SerializeField] private Image hpBarFill;
    [SerializeField] private TextMeshProUGUI seqText;

    private int maxHp;

    public void SetData(int monsterSeq, int hp)
    {
        maxHp = hp;
        seqText.text = $"Seq : {monsterSeq.ToString()}";
        hpBarFill.fillAmount = 1f;
    }

    public void UpdateHp(int remainHp)
    {
        hpBarFill.fillAmount = maxHp > 0 ? (float)remainHp / maxHp : 0f;
    }
}
