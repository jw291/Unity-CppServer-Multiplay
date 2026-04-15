using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    [SerializeField] private Projectile projectile;
    protected SkillTableData tableData;
    protected Vector3 FirePosition => Managers.Instance.Player.MyPlayer.FirePosition;
    protected Monster NearestMonster { get; private set; }

    public Projectile Projectile => projectile;
    protected int level;
    public int SkillLevel => level;
    protected int Damage => tableData.GetDamage(level);
    protected float Speed => tableData.speed;
    protected float Interval => tableData.GetIntervals(level);
    private Coroutine fireLoop;

    public void SetTableData(SkillTableData tableData)
    {
        this.tableData = tableData;
    }

    public void SetLevel(int level)
    {
        this.level = level;
    }

    public void SetNearestMonster(Monster monster)
    {
        NearestMonster = monster;
    }

    public void Create()
    {
        gameObject.SetActive(true);
    }

    public void Execute()
    {
        if (fireLoop != null)
            StopCoroutine(fireLoop);
        fireLoop = StartCoroutine(FireLoop());
    }

    protected abstract System.Collections.IEnumerator FireLoop();
}
