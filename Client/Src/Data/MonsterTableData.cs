using UnityEngine;

[System.Serializable]
public class MonsterTableData
{
    public int    monsterId;
    public string poolType;
    public string prefabPath;
    public int    maxHp;
    public float  moveSpeed;
    public int    attack;
    public float  attackInterval;
    
    public PoolType GetPoolType()
    {
        return System.Enum.TryParse(poolType, out PoolType result) ? result : default;
    }
    private Monster _monsterPrefab;
    public Monster MonsterPrefab
    {
        get
        {
            if (_monsterPrefab == null)
                _monsterPrefab = Resources.Load<Monster>(prefabPath);
            return _monsterPrefab;
        }
    }
    
}
