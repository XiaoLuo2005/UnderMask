using UnityEngine;

public class StreetLight : MonoBehaviour
{
    private bool isLit = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, LayerMask.GetMask("Light"));

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                OnAttemptLightUp();
            }
        }
    }

    private void OnAttemptLightUp()
    {
        // 获取场景中所有带有 "Player" 标签的物体
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            Debug.LogWarning("场景中没有找到任何带有 Player 标签的物体！");
            return;
        }

        bool anyPlayerHasPower = false;

        // 遍历所有玩家物体进行检查
        foreach (GameObject player in players)
        {
            PlayerAbility ability = player.GetComponent<PlayerAbility>();
            if (ability != null && ability.canUseAnxietyPower)
            {
                anyPlayerHasPower = true;
                break; // 只要有一个满足条件，就可以点灯了
            }
        }

        if (anyPlayerHasPower && !isLit)
        {
            LightUp();
        }
        else if (!anyPlayerHasPower)
        {
            Debug.Log("你感到很不安，似乎需要某个面具才能点亮这盏灯...");
        }
    }

    void LightUp()
    {
        isLit = true;

        // 开启名为 "Lit" 的子物体
        Transform litPart = transform.Find("Lit");
        if (litPart != null)
        {
            litPart.gameObject.SetActive(true);
        }

        // 开启名为 "FogMask" 的子物体
        Transform fogMask = transform.Find("FogMask");
        if (fogMask != null)
        {
            fogMask.gameObject.SetActive(true);
        }

        Debug.Log("路灯已点亮，迷雾退散！");
    }
}