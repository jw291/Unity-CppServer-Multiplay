using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDPlayerInfoLayer : MonoBehaviour
{
    [SerializeField] private Image playerHealthBar;
    [SerializeField] private TextMeshProUGUI hpText;

    private void Start()
    {
        Managers.Instance.Player.OnMyPlayerHpChanged += OnHpChanged;

        var mp = Managers.Instance.Player.MyPlayer;
        if (mp != null)
            mp.OnHpChanged += OnHpChanged;
    }

    private void OnDestroy()
    {
        if (Managers.Instance != null && Managers.Instance.Player != null)
            Managers.Instance.Player.OnMyPlayerHpChanged -= OnHpChanged;
    }

    private void OnHpChanged(int current, int max)
    {
        if (max <= 0) return;

        if (playerHealthBar != null)
            playerHealthBar.fillAmount = (float)current / max;

        if (hpText != null)
            hpText.text = $"{current} / {max}";
    }
}