using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("标签设置")]
    public string spikeTag = "Spikes";
    public string checkpointTag = "Checkpoint";
    public string monsterTag = "AnxietyEnemy";

    private Vector3 currentCheckpoint;

    void Start()
    {
        // 1. 初始重生点设为角色当前位置
        currentCheckpoint = transform.position;

        // 2. 强制隐藏场景中所有存档点的 Glow 子物体
        GameObject[] allCheckpoints = GameObject.FindGameObjectsWithTag(checkpointTag);
        foreach (GameObject cp in allCheckpoints)
        {
            Transform glowEffect = cp.transform.Find("Glow");
            if (glowEffect != null)
            {
                glowEffect.gameObject.SetActive(false); // 强制设为不可见
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 碰到地刺
        if (collision.CompareTag(spikeTag) || collision.CompareTag(monsterTag))
        {
            Respawn();
        }
        // 碰到存档点
        else if (collision.CompareTag(checkpointTag))
        {
            // 更新重生点为触发时的位置
            currentCheckpoint = transform.position;

            // 开启当前存档点的 Glow
            Transform glowEffect = collision.transform.Find("Glow");
            if (glowEffect != null)
            {
                glowEffect.gameObject.SetActive(true);
            }

            Debug.Log("存档成功！");
        }
    }

    public void Respawn()
    {
        transform.position = currentCheckpoint;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        AnxietyEnemy[] enemies = FindObjectsOfType<AnxietyEnemy>(true);
        foreach (AnxietyEnemy enemy in enemies)
        {
            enemy.ResetEnemy();
        }
    }
}