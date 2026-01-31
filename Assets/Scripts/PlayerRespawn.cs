using UnityEngine;
using System.Collections.Generic;

public class PlayerRespawn : MonoBehaviour
{
    [Header("标签设置")]
    public string spikeTag = "Spikes";
    public string checkpointTag = "Checkpoint";
    public string monsterTag = "AnxietyEnemy";

    private Vector3 currentCheckpoint;
    private List<GameObject> allLevelSpikes = new List<GameObject>();

    void Start()
    {
        currentCheckpoint = transform.position;

        // 记录场景中初始的所有地刺引用
        GameObject[] spikes = GameObject.FindGameObjectsWithTag(spikeTag);
        foreach (GameObject s in spikes)
        {
            allLevelSpikes.Add(s);
        }

        // 游戏开始时默认关闭所有灯
        ResetAllLightsInScene();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(spikeTag) || collision.CompareTag(monsterTag))
        {
            Respawn();
        }
        else if (collision.CompareTag(checkpointTag))
        {
            // 存档逻辑
            currentCheckpoint = transform.position;

            // 注意：存档点开启的通常是它自己的 Glow 或标志
            Transform glowEffect = collision.transform.Find("Glow");
            if (glowEffect != null) glowEffect.gameObject.SetActive(true);

            Debug.Log("存档成功！");
        }
    }

    public void Respawn()
    {
        transform.position = currentCheckpoint;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;

        // 1. 重置敌人
        AnxietyEnemy[] enemies = FindObjectsOfType<AnxietyEnemy>(true);
        foreach (AnxietyEnemy enemy in enemies) enemy.ResetEnemy();

        // 2. 清理箭矢，保留地刺
        GameObject[] currentSpikes = GameObject.FindGameObjectsWithTag(spikeTag);
        foreach (GameObject s in currentSpikes)
        {
            if (!allLevelSpikes.Contains(s)) Destroy(s);
        }

        // 3. 恢复消失的地刺
        foreach (GameObject spike in allLevelSpikes)
        {
            if (spike != null) spike.SetActive(true);
        }

        // 4. 调用路灯重置
        ResetAllLightsInScene();
    }

    private void ResetAllLightsInScene()
    {
        // 关键：通过 FindObjectsOfType 找到所有挂载了 StreetLight 脚本的对象
        StreetLight[] lights = FindObjectsOfType<StreetLight>(true);
        foreach (StreetLight lt in lights)
        {
            lt.ResetLight(); // 调用路灯自己的重置函数
        }

        // 如果你的存档点 Glow 逻辑独立，也在这里关闭
        GameObject[] checkpoints = GameObject.FindGameObjectsWithTag(checkpointTag);
        foreach (GameObject cp in checkpoints)
        {
            Transform glow = cp.transform.Find("Glow");
            if (glow != null) glow.gameObject.SetActive(false);
        }
    }
}