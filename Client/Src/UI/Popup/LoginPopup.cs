using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginPopup : PopupBase
{
    [SerializeField] private TMP_InputField usernameInput;

    private void Awake()
    {
    }

    public void OnClickLogin()
    {
        string username = usernameInput.text.Trim();
        if (string.IsNullOrEmpty(username)) return;

        Managers.Instance.Network.Connect("127.0.0.1", 7777, username);
    }
}