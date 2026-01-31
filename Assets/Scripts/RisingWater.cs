using UnityEngine;

public class RisingWater : MonoBehaviour
{
    [Header("上升设置")]
    public float riseSpeed = 1f;   // 每秒上升多少
    public bool isRising = false;

    private float checkpointWaterY; // 存档点对应的水位

    void Start()
    {
        checkpointWaterY = transform.position.y;
    }

    void Update()
    {
        if (isRising)
        {
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
        }
    }

    /// <summary>
    /// 玩家到达 checkpoint 时调用
    /// </summary>
    public void SaveWaterLevel()
    {
        checkpointWaterY = transform.position.y;
    }

    /// <summary>
    /// 玩家死亡回档时调用
    /// </summary>
    public void ResetWaterLevel()
    {
        transform.position = new Vector3(
            transform.position.x,
            checkpointWaterY,
            transform.position.z
        );
    }

    /// <summary>
    /// checkpoint 激活后，开始上升
    /// </summary>
    public void StartRising()
    {
        isRising = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerRespawn respawn = collision.GetComponent<PlayerRespawn>();
            if (respawn != null)
            {
                respawn.Respawn();
            }
        }
    }

}
