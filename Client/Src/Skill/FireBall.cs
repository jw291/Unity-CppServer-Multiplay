using UnityEngine;

public class FireBall : Projectile
{
    private Vector2 direction;
    private float speed;

    private void Awake()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        GetComponent<Collider2D>().isTrigger = true;
    }

    public void Activate() { }

    public override void Launch(Vector2 dir, float spd, int dmg = 0)
    {
        base.Launch(dir, spd, dmg);
        direction = dir;
        speed = spd;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void FixedUpdate()
    {
        transform.Translate(direction * (speed * Time.deltaTime), Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Monster monster))
            monster.TakeDamage(damage);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Boundary _))
            ReturnToPool();
    }

    private void ReturnToPool()
    {
        Managers.Instance.Pool.Push(PoolType.FireBall, this);
    }
}
