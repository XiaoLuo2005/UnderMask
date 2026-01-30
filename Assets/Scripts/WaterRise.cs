using UnityEngine;

public class WaterRise : MonoBehaviour
{
    [Tooltip("上升速度：米/秒")]
    public float riseSpeed = 0.2f;

    [Tooltip("最高水位（世界坐标Y），到达后停止")]
    public float maxHeight = 10f;

    [Tooltip("是否从当前高度开始")]
    public bool startFromCurrent = true;

    float startY;

    void Start()
    {
        startY = startFromCurrent ? transform.position.y : startY;
    }

    void Update()
    {
        var pos = transform.position;
        pos.y += riseSpeed * Time.deltaTime;
        pos.y = Mathf.Min(pos.y, maxHeight);
        transform.position = pos;
    }
}
