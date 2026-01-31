using UnityEngine;

public class RisingWater : MonoBehaviour
{
    [Header("上升设置")]
    public float riseSpeed = 1f;
    public bool isRising = false;

    private float checkpointWaterY;

    void Start()
    {
        checkpointWaterY = transform.position.y;
    }

    void Update()
    {
        if (isRising)
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
    }

    // checkpoint 触发时调用：记录水位
    public void SaveWaterLevel()
    {
        checkpointWaterY = transform.position.y - 3f;
        Debug.Log("SaveWaterLevel: " + checkpointWaterY);
    }

    // 死亡回档时调用：恢复到记录水位
    public void ResetWaterLevel()
    {
        transform.position = new Vector3(transform.position.x, checkpointWaterY, transform.position.z);
        Debug.Log("ResetWaterLevel: " + checkpointWaterY);
    }

    public void StartRising()
    {
        isRising = true;
        Debug.Log("Water StartRising");
    }
}
