using UnityEngine;

public class TestAutoMover : MonoBehaviour
{
    [SerializeField] private RectTransform moveArea;
    public RectTransform MoveArea { get => moveArea; set => moveArea = value; }
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float changeInterval = 3f;

    private MyPlayer player;
    private Vector2 targetPos;
    private float timer;

    private void Start()
    {
        player = Managers.Instance.Player.MyPlayer;
        if (player == null)
        {
            enabled = false;
            return;
        }

        player.IsAutoMove = true;
        PickNewTarget();
    }

    private void Update()
    {
        if (player == null)
            return;

        timer += Time.deltaTime;
        if (timer >= changeInterval)
        {
            timer = 0f;
            PickNewTarget();
        }

        Vector2 dir = targetPos - (Vector2)player.transform.position;
        if (dir.magnitude > 0.1f)
            player.AutoMoveTo(dir);
        else
            player.AutoMoveTo(Vector2.zero);
    }

    private void PickNewTarget()
    {
        Vector3[] corners = new Vector3[4];
        moveArea.GetWorldCorners(corners);

        float minX = corners[0].x;
        float minY = corners[0].y;
        float maxX = corners[2].x;
        float maxY = corners[2].y;

        targetPos = new Vector2(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY)
        );
    }
}
