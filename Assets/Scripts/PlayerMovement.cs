using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("移动速度")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // 2D角色移动配置
        rb.gravityScale = 0;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.freezeRotation = true;

        // 初始暂停动画（显示第一帧）
        anim.speed = 0f;
    }

    void Update()
    {
        // 获取WASD输入
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(horizontalInput, verticalInput).normalized;

        // 控制动画播放/暂停
        if (moveDirection.magnitude > 0.1f)
        {
            anim.speed = 1f;
            // 传递方向参数给Animator
            anim.SetFloat("MoveX", moveDirection.x);
            anim.SetFloat("MoveY", moveDirection.y);
        }
        else
        {
            anim.speed = 0f;
        }
    }
    void FixedUpdate()
    {
        // 计算目标位置
        Vector2 targetPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
        // 使用 MovePosition 移动，会和碰撞体产生阻挡
        rb.MovePosition(targetPosition);
    }
}
