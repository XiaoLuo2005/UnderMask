using System.Collections; // 必须引入，用于支持协程
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("攻击设置")]
    public int damage = 3;
    public float attackCooldown = 1.5f; // 攻击间隔时间

    [Tooltip("核心设置：从播放动画到造成伤害的延迟时间（秒）。根据你的动画挥动速度调整")]
    public float damageDelay = 0.5f;

    [Header("引用")]
    public Animator animator;
    public Collider2D attackRange;
    public EnemyChasePlayer enemyChase;

    [Header("Audio (音效)")]
    public AudioSource attackSource;

    private float lastAttackTime;
    private PlayerHealth target;
    private bool isAttacking = false; // 标记位，防止重复启动协程

    void Awake()
    {
        if (enemyChase == null)
            enemyChase = GetComponent<EnemyChasePlayer>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var hp = other.GetComponentInParent<PlayerHealth>();
        if (hp != null)
        {
            target = hp;
            // 玩家进入攻击范围，通知追击脚本停止移动
            if (enemyChase != null)
                enemyChase.isPlayerInAttackRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var hp = other.GetComponentInParent<PlayerHealth>();
        if (hp != null && hp == target)
        {
            target = null;
            // 玩家离开攻击范围，通知追击脚本恢复移动
            if (enemyChase != null)
                enemyChase.isPlayerInAttackRange = false;
        }
    }

    void Update()
    {
        // 玩家不在攻击范围或正在攻击中，直接返回
        if (target == null || isAttacking) return;

        // 玩家在攻击范围内，且攻击冷却完成
        if (Time.time - lastAttackTime > attackCooldown)
        {
            // 使用协程来处理攻击序列
            StartCoroutine(AttackSequence());
        }
    }

    // 核心：攻击序列协程
    IEnumerator AttackSequence()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        // 1. 触发动画
        if (animator != null)
        {
            animator.SetTrigger("Attack");
            attackSource.Play();
            Debug.Log($"敌人发起攻击动画");
        }

        // 2. 等待设定的延迟时间（模拟动画挥舞到命中点的时间）
        yield return new WaitForSeconds(damageDelay);

        // 3. 执行伤害判定
        DealDamage();

        // 4. 标记攻击动作结束（允许下一次冷却检测）
        isAttacking = false;
    }

    // 实际扣血逻辑（现在由协程在特定时间点调用）
    public void DealDamage()
    {
        // 关键逻辑：在伤害触发瞬间，再次检查玩家是否还在触发器范围内（即target不为null）
        if (target != null)
        {
            target.TakeDamage(damage);
            Debug.Log($"<color=red>命中！</color> 玩家受到 {damage} 点伤害");
        }
        else
        {
            Debug.Log("<color=white>挥空了</color>：玩家在伤害判定前离开了范围");
        }
    }

    // 防止状态残留
    void OnDisable()
    {
        // 物体隐藏时停止所有正在进行的协程
        StopAllCoroutines();
        isAttacking = false;

        if (enemyChase != null)
            enemyChase.isPlayerInAttackRange = false;
        target = null;
    }
}