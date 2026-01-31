using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChasePlayer : MonoBehaviour
{
    [Header("追击参数")]
    public float chaseSpeed = 6f;
    [Range(0f, 1f)]
    public float airControlMultiplier = 0.4f;
    public float chaseRange = 8f;
    public float stopDistance = 0.02f;

    [Header("活动范围限制（X轴）")]
    public float minX = -10f;
    public float maxX = 10f;

    [Header("物理检测设置")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public float wallCheckDistance = 0.05f;
    public LayerMask wallLayer;

    [Header("引用设置")]
    public Transform player;
    public EnemyAttack enemyAttack;

    [Tooltip("核心：所有美术资源的父物体。翻转它会同步翻转武器、特效等。")]
    public Transform spriteRoot;

    [Header("美术偏好设置")]
    [Tooltip("如果你的原始图片/模型默认是朝左看的，请勾选此项。")]
    public bool isDefaultFacingLeft = false;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isTouchingWall;
    private int wallDir;
    private bool isChasing = false;

    // 核心标记：玩家是否在攻击范围内
    public bool isPlayerInAttackRange { get; set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (enemyAttack == null)
            enemyAttack = GetComponent<EnemyAttack>();

        // 自动查找机制：如果没有手动拖入 spriteRoot，尝试寻找名为 "Sprite" 的子物体
        if (spriteRoot == null)
        {
            Transform foundSprite = transform.Find("Sprite");
            if (foundSprite != null)
                spriteRoot = foundSprite;
            else
                spriteRoot = transform; // 兜底方案：实在没有就转动自身（不推荐，可能会影响某些物理射线）
        }
    }

    void Update()
    {
        if (player == null) return;

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // 只有玩家不在攻击范围时，才判断是否追击
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        isChasing = !isPlayerInAttackRange && (distanceToPlayer <= chaseRange && distanceToPlayer > stopDistance);
    }

    void FixedUpdate()
    {
        if (player == null) return;

        CheckWall();

        // 优先级逻辑：攻击范围内停止 > 追击 > 超出范围平滑停止
        if (isPlayerInAttackRange)
        {
            StopMovementCompletely();
            // 即使停止了，如果玩家绕后，敌人也应该看向玩家
            LookAtPlayer();
        }
        else if (isChasing)
        {
            ChasePlayerWithPlayerPhysics();
        }
        else
        {
            StopChaseSmoothly();
        }

        ClampPositionX();
    }

    void ChasePlayerWithPlayerPhysics()
    {
        float clampedTargetX = Mathf.Clamp(player.position.x, minX, maxX);
        float targetDirection = Mathf.Sign(clampedTargetX - transform.position.x);

        float targetSpeed = targetDirection * chaseSpeed;
        float control = isGrounded ? 1f : airControlMultiplier;

        float newX = Mathf.MoveTowards(
            rb.velocity.x,
            targetSpeed,
            chaseSpeed * control * Time.fixedDeltaTime * 10f
        );

        // 撞墙检测：如果在空中且撞墙，维持当前速度不被重置
        if (!isGrounded && isTouchingWall && targetDirection == wallDir)
            newX = rb.velocity.x;

        rb.velocity = new Vector2(newX, rb.velocity.y);

        // 调用翻转方法
        FlipTowardsPlayer(targetDirection);
    }

    // 专门用于停止时也要看向玩家的辅助方法
    void LookAtPlayer()
    {
        float dir = Mathf.Sign(player.position.x - transform.position.x);
        FlipTowardsPlayer(dir);
    }

    void StopChaseSmoothly()
    {
        float control = isGrounded ? 1f : airControlMultiplier;
        float newX = Mathf.MoveTowards(rb.velocity.x, 0f, chaseSpeed * control * Time.fixedDeltaTime * 15f);
        rb.velocity = new Vector2(newX, rb.velocity.y);
    }

    void StopMovementCompletely()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    void ClampPositionX()
    {
        Vector3 pos = transform.position;
        if (pos.x < minX)
        {
            pos.x = minX;
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else if (pos.x > maxX)
        {
            pos.x = maxX;
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        transform.position = pos;
    }

    void CheckWall()
    {
        isTouchingWall = false;
        wallDir = 0;

        if (Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, wallLayer))
        {
            isTouchingWall = true;
            wallDir = 1;
        }
        else if (Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, wallLayer))
        {
            isTouchingWall = true;
            wallDir = -1;
        }
    }

    /// <summary>
    /// 核心逻辑：基于 spriteRoot 的 localScale 进行翻转
    /// </summary>
    void FlipTowardsPlayer(float direction)
    {
        if (spriteRoot == null || direction == 0) return;

        Vector3 scale = spriteRoot.localScale;

        // 1. 获取基础朝向 (向右为正，向左为负)
        float targetScaleX = direction > 0 ? 1f : -1f;

        // 2. 如果美术资源默认是朝左画的，则反转逻辑
        if (isDefaultFacingLeft)
        {
            targetScaleX *= -1f;
        }

        // 3. 应用缩放（保持原有的缩放倍数，只改符号）
        scale.x = Mathf.Abs(scale.x) * targetScaleX;
        spriteRoot.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(new Vector3(minX, transform.position.y - 1, 0),
                        new Vector3(minX, transform.position.y + 1, 0));
        Gizmos.DrawLine(new Vector3(maxX, transform.position.y - 1, 0),
                        new Vector3(maxX, transform.position.y + 1, 0));
    }
}