using UnityEngine;

public class Projectile : MonoBehaviour, IPoolableObject
{
    protected int damage;

    public virtual void Launch(Vector2 direction, float speed, int damage = 0)
    {
        this.damage = damage;
    }

    public void Activate()
    {

    }
}
