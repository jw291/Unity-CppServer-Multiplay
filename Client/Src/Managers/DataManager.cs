using UnityEngine;

public class DataManager : MonoBehaviour
{
    public SkillTable SkillTable { get; private set; }
    public MonsterTable MonsterTable { get; private set; }
    public ItemTable ItemTable { get; private set; }
    public ShopTable ShopTable { get; private set; }
    public PlayerTable PlayerTable { get; private set; }

    public void Init()
    {
        SkillTable = new SkillTable();
        SkillTable.Load();

        MonsterTable = new MonsterTable();
        MonsterTable.Load();

        ItemTable = new ItemTable();
        ItemTable.Load();

        ShopTable = new ShopTable();
        ShopTable.Load();

        PlayerTable = new PlayerTable();
        PlayerTable.Load();
    }
}