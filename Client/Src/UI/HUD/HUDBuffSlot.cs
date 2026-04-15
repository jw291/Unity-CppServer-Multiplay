using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDBuffSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI timerText;

    private float remainTime;
    private bool active;

    public string EffectType { get; private set; }
    public event Action<string> OnExpired;

    public void SetData(ItemTableData item)
    {
        EffectType = item.effectType;
        icon.sprite = item.Icon;
        icon.enabled = icon.sprite != null;
        remainTime = item.duration;
        active = true;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!active)
            return;

        remainTime -= Time.deltaTime;
        timerText.text = Mathf.CeilToInt(remainTime).ToString();

        if (remainTime <= 0f)
        {
            active = false;
            gameObject.SetActive(false);
            OnExpired?.Invoke(EffectType);
        }
    }
}
