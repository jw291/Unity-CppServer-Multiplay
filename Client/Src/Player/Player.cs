using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    protected float moveSpeed;
    protected bool facingRight = true;

    private SpriteRenderer spriteRenderer;
    public Vector3 FirePosition => spriteRenderer != null
        ? spriteRenderer.bounds.center
        : transform.position;

    protected Animator animator;
    public uint accountId;
    public uint AccountId => accountId;

    protected int hp;
    protected int maxHp;
    protected float atkBonus;
    protected float speedBonus;
    protected float defenseBonus;

    public event Action<int, int> OnHpChanged;

    protected void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        animator = gameObject.GetComponent<Animator>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    public void SetPlayerData(PlayerTableData data)
    {
        maxHp = data.maxHp;
        hp = data.maxHp;
        moveSpeed = data.speed;
        defenseBonus = data.defense;
    }

    public void SetAccountId(uint accountId)
    {
        this.accountId = accountId;
    }

    public void Movement(Vector3 translation, Space relativeTo)
    {
        transform.Translate(translation, relativeTo);
    }

    public void Create(Transform parent)
    {
        transform.SetParent(parent);
        gameObject.SetActive(true);
    }

    public void Destroy()
    {
        gameObject.SetActive(false);
    }

    public void TakeDamage(int remainHp)
    {
        hp = Mathf.Clamp(remainHp, 0, maxHp);
        OnHpChanged?.Invoke(hp, maxHp);
        StartCoroutine(HitFlash());
    }

    private IEnumerator HitFlash()
    {
        if (spriteRenderer == null)
            yield break;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = Color.white;
    }

    protected void HandleFlip(Vector2 direction)
    {
        if (direction.x < 0f && facingRight)
        {
            facingRight = false;
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else if (direction.x > 0f && !facingRight)
        {
            facingRight = true;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    protected void HandleAnimation(Vector2 direction)
    {
        bool isMoving = direction.sqrMagnitude > 0.01f;
        animator.SetBool("IsMoving", isMoving);
    }

    public void HealHp(int amount)
    {
        hp = Mathf.Min(hp + amount, maxHp);
        OnHpChanged?.Invoke(hp, maxHp);
    }

    public void AddAtkBuff(int value, float duration)
    {
        StartCoroutine(AtkBuffCoroutine(value, duration));
    }

    public void AddSpeedBuff(int value, float duration)
    {
        StartCoroutine(SpeedBuffCoroutine(value, duration));
    }

    public void AddDefenseBuff(int value, float duration)
    {
        StartCoroutine(DefenseBuffCoroutine(value, duration));
    }

    private IEnumerator AtkBuffCoroutine(int value, float duration)
    {
        atkBonus += value;
        yield return new WaitForSeconds(duration);
        atkBonus -= value;
    }

    private IEnumerator SpeedBuffCoroutine(int value, float duration)
    {
        moveSpeed += value;
        speedBonus += value;
        yield return new WaitForSeconds(duration);
        moveSpeed -= value;
        speedBonus -= value;
    }

    private IEnumerator DefenseBuffCoroutine(int value, float duration)
    {
        defenseBonus += value;
        yield return new WaitForSeconds(duration);
        defenseBonus -= value;
    }
}
