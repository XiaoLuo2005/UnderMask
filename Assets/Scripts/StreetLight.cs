using UnityEngine;

public class StreetLight : MonoBehaviour
{
    private bool isLit = false;
    private AudioSource audioSource; // 新增：用于存储音效组件

    void Awake()
    {
        // 初始化时获取自身的 AudioSource 组件
        audioSource = GetComponent<AudioSource>();
    }

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
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            Debug.LogWarning("场景中没有找到任何带有 Player 标签的物体！");
            return;
        }

        bool anyPlayerHasPower = false;

        foreach (GameObject player in players)
        {
            PlayerAbility ability = player.GetComponent<PlayerAbility>();
            if (ability != null && ability.canUseAnxietyPower)
            {
                anyPlayerHasPower = true;
                break;
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

        // 1. 播放点灯音效
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }

        // 2. 开启视觉效果
        Transform litPart = transform.Find("Lit");
        if (litPart != null)
        {
            litPart.gameObject.SetActive(true);
        }

        // 3. 开启迷雾遮罩
        Transform fogMask = transform.Find("FogMask");
        if (fogMask != null)
        {
            fogMask.gameObject.SetActive(true);
        }

        Debug.Log("路灯已点亮，迷雾退散！");
    }
}