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

    [Header("Audio (音效)")]
    public AudioSource walkAudioSource; // 建议设置为循环(Loop)
    public AudioSource jumpAudioSource; // 播放一次

    private Rigidbody2D rb;
    private float inputX;
    private bool isGrounded;
    private bool isTouchingWall;
    private int wallDir;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");

        // 地面检测
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // 跳跃逻辑
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            // --- 播放跳跃音效 ---
            if (jumpAudioSource != null)
            {
                jumpAudioSource.Play();
            }
        }

        // --- 处理步行音效 ---
        HandleWalkAudio();
    }

    void HandleWalkAudio()
    {
        if (walkAudioSource == null) return;

        // 只有在地面上、有输入、且速度不为0时播放
        bool isMovingOnGround = isGrounded && Mathf.Abs(inputX) > 0.1f && Mathf.Abs(rb.velocity.x) > 0.1f;

        if (isMovingOnGround)
        {
            if (!walkAudioSource.isPlaying)
            {
                walkAudioSource.Play();
            }
        }
        else
        {
            if (walkAudioSource.isPlaying)
            {
                walkAudioSource.Pause(); // 停止走动时暂停或停止音效
            }
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

    // (原有的 CheckWall 和 OnDrawGizmosSelected 保持不变...)
    void CheckWall()
    {
        isTouchingWall = false;
        wallDir = 0;
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, wallLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, wallLayer);
        if (hitRight.collider != null) { isTouchingWall = true; wallDir = 1; }
        else if (hitLeft.collider != null) { isTouchingWall = true; wallDir = -1; }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null) { Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); }
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallCheckDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * wallCheckDistance);
    }
}