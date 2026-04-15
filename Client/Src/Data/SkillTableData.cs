using UnityEngine;

[System.Serializable]
public class SkillTableData
{
    public int skillId;
    public string skillName;
    public string iconPath;
    public string prefabPath;
    public string poolType;
    public int[] damages;
    public float[] intervals;
    public float speed;

    public PoolType GetPoolType()
    {
        return System.Enum.TryParse(poolType, out PoolType result) ? result : default;
    }

    private Skill _skillPrefab;
    public Skill SkillPrefab
    {
        get
        {
            if (_skillPrefab == null)
                _skillPrefab = Resources.Load<Skill>(prefabPath);
            return _skillPrefab;
        }
    }

    public int MaxLevel => damages.Length;

    public int GetDamage(int level)
    {
        int index = Mathf.Clamp(level - 1, 0, damages.Length - 1);
        return damages[index];
    }

    public float GetIntervals(int level)
    {
        int index = Mathf.Clamp(level - 1, 0, damages.Length - 1);
        return intervals[index];
    }

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
