using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 6;
    public int currentHP;

    void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if (currentHP < 0) currentHP = 0;

        Debug.Log("玩家受伤，当前血量 = " + currentHP);

        if (currentHP == 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("玩家死亡");
        // 这里可以切场景 / 重生
    }
}
