using UnityEngine;

public class MoveBetweenPoints : MonoBehaviour
{
    [Header("移动端点（从 Inspector 拖拽）")]
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("移动设置")]
    public float speed = 2f;

    private Transform target;

    void Start()
    {
        // 默认先朝右移动
        if (rightPoint != null)
            target = rightPoint;
    }

    void Update()
    {
        if (leftPoint == null || rightPoint == null) return;

        // 向目标点移动
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // 到达目标后切换方向
        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            target = (target == leftPoint) ? rightPoint : leftPoint;
        }
    }
}
