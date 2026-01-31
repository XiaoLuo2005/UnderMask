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
    public float edgeBuffer = 0.05f;   // ⭐ 边界缓冲，防止卡边抖动

    [Header("朝向控制")]
    public float flipDeadZone = 0.2f;  // ⭐ 翻转死区，防止方向抖动

    [Header("物理检测设置")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public float wallCheckDistance = 0.05f;
    public LayerMask wallLayer;

    [Header("引用设置")]
    public Transform player;
    public EnemyAttack enemyAttack;

    [Tooltip("美术资源父节点")]
    public Transform spriteRoot;

    [Header("美术偏好设置")]
    public bool isDefaultFacingLeft = false;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isTouchingWall;
    private int wallDir;
    private bool isChasing = false;

    public bool isPlayerInAttackRange { get; set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (enemyAttack == null)
            enemyAttack = GetComponent<EnemyAttack>();

        if (spriteRoot == null)
        {
            Transform found = transform.Find("Sprite");
            spriteRoot = found != null ? found : transform;
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

        float distance = Vector2.Distance(transform.position, player.position);
        isChasing = !isPlayerInAttackRange &&
                    distance <= chaseRange &&
                    distance > stopDistance;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        CheckWall();

        if (isPlayerInAttackRange)
        {
            StopMovementCompletely();
            LookAtPlayer(); // ⭐ 停止也看向玩家
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
        float targetX = Mathf.Clamp(player.position.x, minX, maxX);
        float dx = targetX - transform.position.x;

        float targetDirection = 0f;
        if (Mathf.Abs(dx) > flipDeadZone)
            targetDirection = Mathf.Sign(dx);

        float targetSpeed = targetDirection * chaseSpeed;
        float control = isGrounded ? 1f : airControlMultiplier;

        float newX = Mathf.MoveTowards(
            rb.velocity.x,
            targetSpeed,
            chaseSpeed * control * Time.fixedDeltaTime * 10f
        );

        if (!isGrounded && isTouchingWall && targetDirection == wallDir)
            newX = rb.velocity.x;

        rb.velocity = new Vector2(newX, rb.velocity.y);

        if (targetDirection != 0)
            FlipTowardsPlayer(targetDirection);
    }

    void LookAtPlayer()
    {
        float dx = player.position.x - transform.position.x;

        if (Mathf.Abs(dx) < flipDeadZone)
            return;

        FlipTowardsPlayer(Mathf.Sign(dx));
    }

    void StopChaseSmoothly()
    {
        float control = isGrounded ? 1f : airControlMultiplier;

        float newX = Mathf.MoveTowards(
            rb.velocity.x,
            0f,
            chaseSpeed * control * Time.fixedDeltaTime * 15f
        );

        rb.velocity = new Vector2(newX, rb.velocity.y);
    }

    void StopMovementCompletely()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    void ClampPositionX()
    {
        Vector3 pos = transform.position;

        if (pos.x < minX + edgeBuffer)
        {
            pos.x = minX;
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else if (pos.x > maxX - edgeBuffer)
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

    void FlipTowardsPlayer(float direction)
    {
        if (spriteRoot == null || direction == 0) return;

        Vector3 scale = spriteRoot.localScale;

        float targetScaleX = direction > 0 ? 1f : -1f;

        if (isDefaultFacingLeft)
            targetScaleX *= -1f;

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
        Gizmos.DrawLine(
            new Vector3(minX, transform.position.y - 1, 0),
            new Vector3(minX, transform.position.y + 1, 0));

        Gizmos.DrawLine(
            new Vector3(maxX, transform.position.y - 1, 0),
            new Vector3(maxX, transform.position.y + 1, 0));
    }
}
