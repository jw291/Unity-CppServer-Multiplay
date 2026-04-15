using UnityEngine;

[System.Serializable]
public class ItemTableData
{
    public int itemId;
    public string itemName;
    public string iconPath;
    public string effectType;
    public int effectValue;
    public float duration;

    private Sprite _iconCache;

    public Sprite Icon
    {
        get
        {
            if (_iconCache == null)
                _iconCache = Resources.Load<Sprite>(iconPath);
            return _iconCache;
        }
    }
}
