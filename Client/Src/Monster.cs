using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Monster : MonoBehaviour, IPoolableObject
{
    [SerializeField] protected MonsterHeadOnUI monsterHeadOnUI;

    public MonsterTableData Data => this.data;
    public int MonsterSeq { get; private set; }
    private MonsterTableData data;
    private bool facingRight = false;
    private bool isDying = false;
    private float attackCooldown;
    private Collider2D col;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int DieHash = Animator.StringToHash("Die");

    private void Awake()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        col = GetComponent<Collider2D>();
        col.isTrigger = true;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Activate()
    {
        isDying = false;
        col.enabled = true;
        facingRight = false;
        transform.rotation = Quaternion.identity;

        Color c = spriteRenderer.color;
        c.a = 1f;
        spriteRenderer.color = c;
        animator.Play("Idle");
    }

    public void SetData(MonsterTableData data)
    {
        this.data = data;
    }

    public void SetMonsterSeq(int seq)
    {
        MonsterSeq = seq;
        monsterHeadOnUI.SetData(seq, data.maxHp);
    }

    public void TakeDamage(int damage)
    {
        if (isDying)
            return;
        var req = new REQ_MONSTER_HIT();
        req.MonsterSeq = MonsterSeq;
        req.AccountId = Managers.Instance.Network.AccountId;
        req.Damage = damage;
        Managers.Instance.Network.Tcp.Send(PacketId.MSG_REQ_MONSTER_HIT, req);
    }

    public void UpdateHp(int remainHp)
    {
        monsterHeadOnUI.UpdateHp(remainHp);
    }

    public void Die()
    {
        if (isDying)
            return;
        isDying = true;
        col.enabled = false;
        animator.SetBool(IsMovingHash, false);
        UpdateHp(0);
        Managers.Instance.Flock.RemoveMonster(this);
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        animator.SetTrigger(DieHash);

        float elapsed = 0f;
        Color color = spriteRenderer.color;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed);
            spriteRenderer.color = color;
            yield return null;
        }

        Managers.Instance.Pool.Push(this.data.GetPoolType(), this);
    }

    private void Update()
    {
        if (attackCooldown > 0f)
            attackCooldown -= Time.deltaTime;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isDying)
            return;
        if (attackCooldown > 0f)
            return;
        if (!other.TryGetComponent<MyPlayer>(out _))
            return;

        attackCooldown = data.attackInterval;

        var req = new REQ_PLAYER_HIT
        {
            MonsterSeq = MonsterSeq,
            AccountId = Managers.Instance.Network.AccountId
        };
        Managers.Instance.Network.Tcp.Send(PacketId.MSG_REQ_PLAYER_HIT, req);
    }

    public void Move(Vector2 velocity)
    {
        if (isDying)
            return;

        animator.SetBool(IsMovingHash, velocity.sqrMagnitude > 0.01f);
        transform.Translate(velocity * Time.fixedDeltaTime, Space.World);
        HandleFlip(velocity.x);
    }

    public void MoveFromServer(Vector2 pos, Vector2 dir)
    {
        if (isDying)
            return;

        transform.position = Vector2.Lerp(transform.position, pos, 0.3f);
        animator.SetBool(IsMovingHash, dir.sqrMagnitude > 0.01f);
        HandleFlip(dir.x);
    }

    private void HandleFlip(float dirX)
    {
        if (dirX < 0f && facingRight)
        {
            facingRight = false;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            monsterHeadOnUI.transform.localRotation = Quaternion.identity;
        }
        else if (dirX > 0f && !facingRight)
        {
            facingRight = true;
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            monsterHeadOnUI.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }
}