using UnityEngine;

public class MoveBetweenPoints : MonoBehaviour
{
    [Header("移动端点（从 Inspector 拖拽）")]
    public Transform leftPoint;
    public Transform rightPoint;

    public enum StartDirection
    {
        Left,
        Right
    }

    [Header("初始移动方向")]
    public StartDirection startDirection = StartDirection.Right;

    [Header("移动设置")]
    public float speed = 2f;

    private Transform target;

    void Start()
    {
        if (leftPoint == null || rightPoint == null) return;

        // 根据 Inspector 选择的方向决定初始目标
        target = (startDirection == StartDirection.Right) ? rightPoint : leftPoint;
    }

    void Update()
    {
        if (leftPoint == null || rightPoint == null || target == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            target = (target == leftPoint) ? rightPoint : leftPoint;
        }
    }
}
