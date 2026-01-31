using UnityEngine;

public class SpikeDestruction : MonoBehaviour
{
    [Header("音效设置")]
    public AudioClip destroySound; // 在 Inspector 中拖入你的消失音效文件

    void Update()
    {
        // 1. 监测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            // 将鼠标屏幕位置转换为世界坐标
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 2. 发射射线检测点击，限定在 Spikes 层级
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, LayerMask.GetMask("Spikes"));

            // 3. 判定是否点中了当前物体
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                OnAttemptDestroy();
            }
        }
    }

    private void OnAttemptDestroy()
    {
        // 4. 检查焦虑面具能力
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        bool hasAnxietyPower = false;

        foreach (GameObject player in players)
        {
            PlayerAbility ability = player.GetComponent<PlayerAbility>();
            if (ability != null && ability.canUseAnxietyPower)
            {
                hasAnxietyPower = true;
                break;
            }
        }

        // 5. 如果有能力则播放音效并销毁
        if (hasAnxietyPower)
        {
            // --- 新增：播放消失音效 ---
            if (destroySound != null)
            {
                AudioSource.PlayClipAtPoint(destroySound, transform.position);
            }

            Debug.Log("看穿了障碍的虚幻，障碍已消失！");
            Destroy(gameObject); // 执行销毁
        }
        else
        {
            Debug.Log("你感到很焦虑，这些障碍看起来无法逾越...");
        }
    }
}