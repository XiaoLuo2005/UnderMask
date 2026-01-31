using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("血量")]
    public int maxHP = 6;
    public int currentHP;

    [Header("重生点")]
    public Transform respawnPoint;

    [Header("可选：血条UI")]
    public PlayerHealthUI healthUI;

    [Header("引用设置")]
    public Animator animator;

    Rigidbody2D rb;

    void Awake()
    {
        currentHP = maxHP;
        rb = GetComponent<Rigidbody2D>();

        UpdateUI();
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if (currentHP < 0) currentHP = 0;

        Debug.Log("玩家受伤，当前血量 = " + currentHP);

        UpdateUI();

        if (currentHP == 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("玩家死亡 → 回到重生点");

        Respawn();
    }

    void Respawn()
    {
        if (respawnPoint == null)
        {
            Debug.LogError("没有设置重生点！");
            return;
        }

        // 移动到重生点
        transform.position = respawnPoint.position;

        // 清空速度
        if (rb != null)
            rb.velocity = Vector2.zero;

        // 恢复满血
        currentHP = maxHP;

        UpdateUI();
     
    }

    void UpdateUI()
    {
        if (healthUI != null)
            healthUI.Refresh(currentHP);
    }
}
