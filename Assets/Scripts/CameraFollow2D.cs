using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;      // 人物
    public Vector3 offset = new Vector3(0, 1.5f, -10f);
    public float smoothTime = 0.1f;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + offset;
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );
    }
}
