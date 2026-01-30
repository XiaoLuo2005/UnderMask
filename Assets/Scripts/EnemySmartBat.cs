using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2D敌人追击玩家的核心脚本（匹配玩家物理逻辑）
public class EnemyChasePlayer : MonoBehaviour
{
    [Header("追击参数")]
    public float chaseSpeed = 6f;          // 追击移动速度（和玩家moveSpeed对应）
    [Range(0f, 1f)]
    public float airControlMultiplier = 0.4f; // 空中操控系数（和玩家保持一致）
    public float chaseRange = 8f;          // 触发追击的距离（超出则停止追击）
    public float stopDistance = 0.02f;      // 追到玩家身边后停止的距离（避免贴脸）

    [Header("物理检测设置（和玩家一致）")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public float wallCheckDistance = 0.05f;
    public LayerMask wallLayer;

    [Header("引用设置")]
    public Transform player;               // 玩家的Transform（需在编辑器中赋值）
    private Rigidbody2D rb;                // 敌人自身的刚体组件

    // 物理状态（和玩家一致）
    private bool isGrounded;
    private bool isTouchingWall;
    private int wallDir; // -1 左墙，1 右墙
    private bool isChasing = false;

    void Awake()
    {
        // 获取敌人自身的Rigidbody2D组件（必须挂载，否则无法移动）
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("敌人缺少Rigidbody2D组件！请给敌人添加Rigidbody2D", this);
        }
    }

    void Update()
    {
        // 如果未指定玩家，直接返回（避免报错）
        if (player == null)
        {
            Debug.LogWarning("未指定玩家对象！请在编辑器中给EnemyChasePlayer脚本的player赋值", this);
            return;
        }

        // 地面检测（和玩家逻辑一致）
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // 计算敌人到玩家的距离
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 判断是否进入追击范围
        isChasing = (distanceToPlayer <= chaseRange && distanceToPlayer > stopDistance);
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // 墙体检测（和玩家逻辑一致）
        CheckWall();

        // 物理相关的移动逻辑放在FixedUpdate中（Unity最佳实践）
        if (isChasing)
        {
            ChasePlayerWithPlayerPhysics();
        }
        else
        {
            // 停止追击时，平滑减速（而不是直接清零，更符合物理）
            StopChaseSmoothly();
        }
    }

    // 核心追击逻辑：使用和玩家完全一致的物理规则追击
    void ChasePlayerWithPlayerPhysics()
    {
        // 计算敌人需要移动的方向（只有水平方向，和玩家移动逻辑一致）
        float targetDirection = Mathf.Sign(player.position.x - transform.position.x);

        // 计算目标速度（和玩家的targetSpeed逻辑一致）
        float targetSpeed = targetDirection * chaseSpeed;

        // 地面/空中操控系数（和玩家完全一致）
        float control = isGrounded ? 1f : airControlMultiplier;

        // 平滑插值到目标速度（核心物理逻辑，和玩家Mathf.MoveTowards一致）
        float newX = Mathf.MoveTowards(
            rb.velocity.x,
            targetSpeed,
            chaseSpeed * control * Time.fixedDeltaTime * 10f
        );

        // 防止贴墙横移（和玩家逻辑一致）
        if (!isGrounded && isTouchingWall && targetDirection == wallDir)
        {
            newX = rb.velocity.x;
        }

        // 应用速度（只修改X轴，Y轴由物理引擎自然控制）
        rb.velocity = new Vector2(newX, rb.velocity.y);

        // 让敌人朝向玩家（左右翻转）
        FlipTowardsPlayer(targetDirection);
    }

    // 平滑停止追击（避免突然停住的生硬感）
    void StopChaseSmoothly()
    {
        float stopControl = isGrounded ? 1f : airControlMultiplier;
        float newX = Mathf.MoveTowards(
            rb.velocity.x,
            0f,
            chaseSpeed * stopControl * Time.fixedDeltaTime * 15f // 15f比移动时的10f稍大，停止更快
        );
        rb.velocity = new Vector2(newX, rb.velocity.y);
    }

    // 墙体检测逻辑（完全复制玩家的CheckWall方法）
    void CheckWall()
    {
        isTouchingWall = false;
        wallDir = 0;

        RaycastHit2D hitRight = Physics2D.Raycast(
            transform.position,
            Vector2.right,
            wallCheckDistance,
            wallLayer
        );

        RaycastHit2D hitLeft = Physics2D.Raycast(
            transform.position,
            Vector2.left,
            wallCheckDistance,
            wallLayer
        );

        if (hitRight.collider != null)
        {
            isTouchingWall = true;
            wallDir = 1;
        }
        else if (hitLeft.collider != null)
        {
            isTouchingWall = true;
            wallDir = -1;
        }
    }

    // 让敌人的Sprite朝向玩家（简化版，基于方向值）
    void FlipTowardsPlayer(float direction)
    {
        if (direction > 0 && transform.localScale.x < 0)
        {
            // 玩家在右侧，敌人朝右
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (direction < 0 && transform.localScale.x > 0)
        {
            // 玩家在左侧，敌人朝左
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    // 编辑器Gizmos：绘制追击范围和物理检测区域（方便调试）
    void OnDrawGizmosSelected()
    {
        // 绘制追击范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        // 绘制地面检测
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // 绘制墙体检测
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallCheckDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * wallCheckDistance);
    }
}