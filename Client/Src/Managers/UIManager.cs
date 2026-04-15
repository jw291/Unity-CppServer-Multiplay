using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<PopupBase> popups = new List<PopupBase>();

    private readonly Dictionary<Type, PopupBase> popupMap = new Dictionary<Type, PopupBase>();

    public void Init()
    {
        foreach (PopupBase popup in popups)
        {
            popupMap[popup.GetType()] = popup;
            popup.Close();
        }

        Open<LoginPopup>();
    }

    public T Open<T>() where T : PopupBase
    {
        if (popupMap.TryGetValue(typeof(T), out PopupBase popup))
        {
            popup.Open();
            return (T)popup;
        }

        Debug.LogWarning($"[UIManager] Popup not found: {typeof(T).Name}");
        return null;
    }

    public void Close<T>() where T : PopupBase
    {
        if (popupMap.TryGetValue(typeof(T), out PopupBase popup))
            popup.Close();
    }

    public T Get<T>() where T : PopupBase
    {
        popupMap.TryGetValue(typeof(T), out PopupBase popup);
        return popup as T;
    }
}
