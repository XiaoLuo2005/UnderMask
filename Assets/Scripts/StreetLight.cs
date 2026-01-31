using UnityEngine;

public class StreetLight : MonoBehaviour
{
    private bool isLit = false; // 逻辑变量：标记灯是否已点亮
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // 确保你的路灯 Collider 在 "Light" 这个 Layer 上
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, LayerMask.GetMask("Light"));

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                OnAttemptLightUp();
            }
        }
    }

    private void OnAttemptLightUp()
    {
        // 查找玩家并检查能力
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
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

        // 只有拥有能力且当前未点亮时才能点火
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

        // 1. 播放音效
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }

        // 2. 开启视觉子物体
        SetVisuals(true);
        Debug.Log(gameObject.name + " 已点亮！");
    }

    // --- 核心新增：供重生脚本调用的重置方法 ---
    public void ResetLight()
    {
        isLit = false; // 重置逻辑状态，否则重生后无法再次点击
        SetVisuals(false); // 重置视觉状态
    }

    // 封装视觉控制逻辑，方便维护
    private void SetVisuals(bool state)
    {
        Transform litPart = transform.Find("Lit");
        if (litPart != null) litPart.gameObject.SetActive(state);

        Transform fogMask = transform.Find("FogMask");
        if (fogMask != null) fogMask.gameObject.SetActive(state);
    }
}