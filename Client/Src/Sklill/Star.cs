using UnityEngine;

public class Star : Projectile
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
        direction = dir.normalized;
        speed = spd;
    }

    private void FixedUpdate()
    {
        transform.Translate(direction * (speed * Time.deltaTime), Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Monster monster))
            monster.TakeDamage(damage); // 관통하며 계속 튕김
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent(out Boundary _)) return;

        Vector2 pos = transform.position;
        Bounds bounds = other.bounds;

        if (pos.x <= bounds.min.x || pos.x >= bounds.max.x)
            direction.x = -direction.x;

        if (pos.y <= bounds.min.y || pos.y >= bounds.max.y)
            direction.y = -direction.y;

        // 경계 밖으로 나간 위치 보정
        transform.position = new Vector2(
            Mathf.Clamp(pos.x, bounds.min.x, bounds.max.x),
            Mathf.Clamp(pos.y, bounds.min.y, bounds.max.y)
        );
    }
}
