using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHP = 3;
    private int hp;

    void Awake()
    {
        hp = maxHP;
    }

    // 该方法被 PlayerAttack.cs 里的 eh.TakeDamage(attackDamage) 调用
    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        Debug.Log($"敌人 {gameObject.name} 受击，剩余HP={hp}");

        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("敌人死亡");
        Destroy(gameObject);
    }
}