using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    [Header("设置")]
    public GameObject arrowPrefab;
    public float spawnInterval = 2f;
    public float arrowSpeed = 5f;
    public float lifeTime = 5f;

    [Header("玩家检测设置")]
    public string playerTag = "Player"; // 玩家的标签
    private bool isPaused = false;      // 是否因为玩家靠近而暂停
    private float timer;

    void Update()
    {
        // 如果玩家不在附近，才进行计时和生成
        if (!isPaused)
        {
            timer += Time.deltaTime;

            if (timer >= spawnInterval)
            {
                SpawnArrow();
                timer = 0;
            }
        }
    }

    void SpawnArrow()
    {
        GameObject newArrow = Instantiate(arrowPrefab, transform.position, transform.rotation);
        Rigidbody2D rb = newArrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 如果你的版本报错 linearVelocity，请改回 velocity
            rb.velocity = transform.right * arrowSpeed;
        }
        Destroy(newArrow, lifeTime);
    }

    // --- 新增：感应区逻辑 ---

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 如果玩家进入并停留在感应区内
        if (collision.CompareTag(playerTag))
        {
            isPaused = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 如果玩家离开感应区
        if (collision.CompareTag(playerTag))
        {
            isPaused = false;
            // 可选：timer = 0; // 如果想让玩家离开后重新开始计时，取消此行注释
        }
    }
}