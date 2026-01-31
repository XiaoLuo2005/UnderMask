using UnityEngine;

public class SpikeDestruction : MonoBehaviour
{
    void Update()
    {
        // 1. 监测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            // 将鼠标屏幕位置转换为世界坐标
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 2. 发射射线检测点击。
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, LayerMask.GetMask("Spikes"));

            // 3. 判定是否点中了当前这个地刺实例
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                Debug.Log("1111");
                OnAttemptDestroy();
            }
        }
    }

    private void OnAttemptDestroy()
    {
        // 4. 获取所有玩家物体并检查焦虑面具能力
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        bool hasAnxietyPower = false;

        foreach (GameObject player in players)
        {
            PlayerAbility ability = player.GetComponent<PlayerAbility>();
            if (ability != null && ability.canUseAnxietyPower) // 改为焦虑面具判断
            {
                hasAnxietyPower = true;
                break;
            }
        }

        // 5. 如果有能力则销毁，否则提示
        if (hasAnxietyPower)
        {
            Debug.Log("看穿了障碍的虚幻，障碍已消失！");
            // 执行销毁逻辑
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("你感到很焦虑，这些障碍看起来无法逾越...");
        }
    }
}