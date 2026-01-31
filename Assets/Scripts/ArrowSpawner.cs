using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    [Header("箭矢设置")]
    public GameObject arrowPrefab;
    public float spawnInterval = 2f;
    public float arrowSpeed = 5f;
    public float lifeTime = 5f;

    [Header("当前状态 (仅观察)")]
    public bool isInRange = false;    // 玩家是否在 ActiveZone 范围内
    public bool isPaused = false;     // 玩家是否在 PauseZone 范围内
    private float timer;

    void Update()
    {
        // 逻辑：只有当玩家进入激活区 (isInRange) 且没有进入暂停区 (isPaused) 时才发射
        if (isInRange && !isPaused)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                SpawnArrow();
                timer = 0;
            }
        }
        else
        {
            // 如果玩家离开或进入暂停区，重置计时器
            timer = 0;
        }
    }

    void SpawnArrow()
    {
        if (arrowPrefab == null) return;

        GameObject newArrow = Instantiate(arrowPrefab, transform.position, transform.rotation);
        Rigidbody2D rb = newArrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = transform.right * arrowSpeed;
        }
        Destroy(newArrow, lifeTime);
    }

    // 公共接口：供子物体脚本调用
    public void SetInRange(bool status) { isInRange = status; }
    public void SetPaused(bool status) { isPaused = status; }
}