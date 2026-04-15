using UnityEngine;

public class RemotePlayer : Player
{
    private Vector3 targetPos;
    private Vector2 direction;
    private float remoteMoveSpeed;

    public void SetMoveData(Vector3 pos, Vector2 dir, float speed)
    {
        targetPos = pos;
        direction = dir;
        remoteMoveSpeed = speed;
    }

    private void Update()
    {
        targetPos += (Vector3)direction * (remoteMoveSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 15f);
        HandleFlip(direction);
        HandleAnimation(direction);
    }
}
