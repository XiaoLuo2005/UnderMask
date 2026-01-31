using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 6f;
    [Range(0f, 1f)]
    public float airControlMultiplier = 0.4f;

    [Header("Jump")]
    public float jumpForce = 12f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;

    [Header("Wall Check")]
    public float wallCheckDistance = 0.05f;
    public LayerMask wallLayer;

    [Header("Animation")]
    public Animator animator;

    private Rigidbody2D rb;
    private float inputX;
    private bool isGrounded;
    private bool isTouchingWall;
    private int wallDir;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");

        // 1. 地面检测
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // 2. 核心逻辑：根据输入更新动画布尔值
        UpdateAnimationBools();

        // 3. 跳跃
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// 根据移动方向直接控制动画机的 bool 开关
    /// </summary>
    void UpdateAnimationBools()
    {
        if (animator == null) return;

        // 向右走
        if (inputX > 0.1f)
        {
  
            animator.SetBool("Right", true);
            animator.SetBool("Left", false);
        }
        // 向左走
        else if (inputX < -0.1f)
        {
            animator.SetBool("Right", false);
            animator.SetBool("Left", true);
        }
        // 站立不动
        else
        {
            animator.SetBool("Right", false);
            animator.SetBool("Left", false);
        }
    }

    void FixedUpdate()
    {
        CheckWall();

        float targetSpeed = inputX * moveSpeed;
        float control = isGrounded ? 1f : airControlMultiplier;

        float newX = Mathf.MoveTowards(
            rb.velocity.x,
            targetSpeed,
            moveSpeed * control * Time.fixedDeltaTime * 10f
        );

        if (!isGrounded && isTouchingWall && inputX == wallDir)
        {
            newX = rb.velocity.x;
        }

        rb.velocity = new Vector2(newX, rb.velocity.y);
    }

    void CheckWall()
    {
        isTouchingWall = false;
        wallDir = 0;

        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, wallLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, wallLayer);

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

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}