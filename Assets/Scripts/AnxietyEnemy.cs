using UnityEngine;
using UnityEngine.UI;

public class AnxietyEnemy : MonoBehaviour
{
    [Header("血量与UI (Slider版)")]
    public int maxHealth = 6;
    public int currentHealth = 6;
    public Slider healthSlider; // 在 Inspector 中拖入 Slider 组件

    [Header("移动设置")]
    public float moveSpeed = 4f;
    public GameObject spawnerParent;
    public GameObject clearanceObstacle;

    private Vector3 startPosition;
    private bool isChasing = false;
    private Transform player;

    void Start()
    {
        startPosition = transform.position;
        InitHealthUI();
    }

    void Update()
    {
        if (isChasing && player != null)
        {
            // 水平移动：保持自身的 Y 坐标不变
            float targetX = Mathf.MoveTowards(transform.position.x, player.position.x, moveSpeed * Time.deltaTime);
            transform.position = new Vector2(targetX, transform.position.y);
        }
    }

    // 初始化进度条：满值且当前值为最大值
    void InitHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth; // 初始为满
        }
    }

    public void TakeDamage()
    {
        currentHealth--;
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth; // 随血量减少而清空进度条
        }

        if (currentHealth <= 0) Die();
    }

    // 开启追击（供传感器脚本调用）
    public void StartChasing(Transform targetPlayer)
    {
        if (!isChasing)
        {
            isChasing = true;
            player = targetPlayer;
            if (spawnerParent != null) spawnerParent.SetActive(true); // 开启箭塔
        }
    }

    public void ResetEnemy()
    {
        currentHealth = maxHealth;
        transform.position = startPosition;
        isChasing = false;
        if (spawnerParent != null) spawnerParent.SetActive(false); // 停止发射
        if (healthSlider != null) healthSlider.value = maxHealth; // 重置进度条为满
        gameObject.SetActive(true);
    }

    void Die()
    {
        if (clearanceObstacle != null) clearanceObstacle.SetActive(false);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 怪物本体碰到玩家
        if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<PlayerRespawn>()?.Respawn();
        }
        // 被箭矢扣血
        else if (other.CompareTag("Spikes"))
        {
            TakeDamage();
            Destroy(other.gameObject);
        }
    }
}