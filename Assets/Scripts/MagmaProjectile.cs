using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagmaProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 2;
    public float autoDestroyTime = 2f;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rb.velocity = Vector2.down * speed;
        Destroy(gameObject, autoDestroyTime);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //// 命中玩家
        if (col.collider.CompareTag("Player"))
        {
            Debug.Log("岩浆攻击到玩家");
            var hp = col.collider.GetComponentInParent<PlayerHealth>();

            if (hp != null)
                hp.TakeDamage(damage);
        }

        // 命中任何实体就消失（地面/平台/墙）
        Destroy(gameObject);
    }
}

