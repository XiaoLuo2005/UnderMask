using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("引用设置")]
    public Animator animator;
    public Transform attackPoint;      // 攻击中心点（空物体）
    public float attackRadius = 0.5f; // 攻击判定圆形的半径
    public LayerMask enemyLayer;      // 目标图层（需设置为 Enemy）

    [Header("攻击参数")]
    public int attackDamage = 1;      // 每次攻击伤害
    public bool attackUnlocked = false;
    public float attackCooldown = 0.5f;

    [Header("动画时间微调")]
    [Tooltip("点击鼠标后，延迟多久产生伤害（配合挥剑动作）")]
    public float hitDelay = 0.15f;
    [Tooltip("整个动作结束释放的延迟")]
    public float releaseDelay = 1.0f;

    private float lastAttackTime;
    private float lastFacingDir = 1f;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    public void UnlockAttack()
    {
        attackUnlocked = true;
        animator.SetBool("AttackState", true);
        Debug.Log("攻击形态开启");
    }

    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        if (inputX != 0) lastFacingDir = inputX;

        if (attackUnlocked && animator != null)
        {
            animator.SetBool("AttackRight", inputX > 0.1f);
            animator.SetBool("AttackLeft", inputX < -0.1f);
        }

        if (!attackUnlocked) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastAttackTime > attackCooldown)
            {
                Attack();
            }
        }
    }

    void Attack()
    {
        lastAttackTime = Time.time;

        if (lastFacingDir > 0)
        {
            animator.SetBool("AttackToRight", true);
            // 开启协程：处理伤害和动画收尾
            StartCoroutine(AttackRoutine("ReleaseRight", releaseDelay));
        }
        else
        {
            animator.SetBool("AttackToLeft", true);
            StartCoroutine(AttackRoutine("ReleaseLeft", releaseDelay));
        }
    }

    /// <summary>
    /// 核心协程：控制伤害时机和动画释放
    /// </summary>
    IEnumerator AttackRoutine(string releaseTrigger, float releaseTime)
    {
        // 1. 等待打击点延迟（让伤害出现在剑挥出的瞬间）
        yield return new WaitForSeconds(hitDelay);

        // 2. 执行伤害判定
        CheckDamage();

        // 3. 继续等待，直到释放动画
        yield return new WaitForSeconds(releaseTime - hitDelay);

        if (animator != null)
        {
            animator.SetBool("AttackToRight", false);
            animator.SetBool("AttackToLeft", false);
            animator.SetBool(releaseTrigger, true);
        }
    }

    void CheckDamage()
    {
        // 产生一个圆形的覆盖检测
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);
       
        foreach (Collider2D h in hits)
        {
            // 尝试从被击中的物体（或其父物体）上获取 EnemyHealth 脚本
            EnemyHealth eh = h.GetComponent<EnemyHealth>();
            if (eh == null) eh = h.GetComponentInParent<EnemyHealth>();

            if (eh != null)
            {
                eh.TakeDamage(attackDamage);
                Debug.Log("<color=cyan>玩家击中了: </color>" + h.name);
            }
        }
    }

    // 在 Scene 窗口画出红色的判定圆，方便调试
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}