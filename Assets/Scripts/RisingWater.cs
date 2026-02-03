using UnityEngine;

public class RisingWater : MonoBehaviour
{
    [Header("上升设置")]
    public float riseSpeed = 1f;
    public bool isRising = false;

    [Header("回档水位偏移（可调）")]
    public float checkpointOffsetY = -3f;   // 原来写死 -3f

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

    public void SaveWaterLevel()
    {
        checkpointWaterY = transform.position.y + checkpointOffsetY;
        Debug.Log("SaveWaterLevel: " + checkpointWaterY);
    }

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
