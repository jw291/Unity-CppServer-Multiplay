using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);
    [SerializeField] private RectTransform boundArea;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        var target = Managers.Instance?.Player?.myPlayer;
        if (!target)
            return;

        Vector3 desiredPosition = target.transform.position + offset;
        Vector3 smoothed = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        if (boundArea != null && cam != null)
            smoothed = Clamp(smoothed);

        transform.position = smoothed;
    }

    private Vector3 Clamp(Vector3 pos)
    {
        Vector3[] corners = new Vector3[4];
        boundArea.GetWorldCorners(corners);

        float minX = corners[0].x;
        float minY = corners[0].y;
        float maxX = corners[2].x;
        float maxY = corners[2].y;

        float halfHeight = cam.orthographicSize;
        float halfWidth = cam.orthographicSize * cam.aspect;

        float clampedX = Mathf.Clamp(pos.x, minX + halfWidth, maxX - halfWidth);
        float clampedY = Mathf.Clamp(pos.y, minY + halfHeight, maxY - halfHeight);

        return new Vector3(clampedX, clampedY, pos.z);
    }
}