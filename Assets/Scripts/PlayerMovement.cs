using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;

    [Header("音效设置（和参考脚本一致）")]
    public AudioSource walkAudioSource; // 走路音效AudioSource（建议设置为循环）

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveDirection;
    private bool isMoving; // 替代参考脚本的isGrounded，判断是否在移动

    void Start()
    {
        // 获取原有组件
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // 2D角色移动基础配置（原有逻辑不变）
        if (rb != null)
        {
            rb.gravityScale = 0; // 2D平面移动，关闭重力
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.freezeRotation = true;
        }

        // 初始暂停动画和走路音效
        if (anim != null)
        {
            anim.speed = 0f;
        }

        // 初始化走路音效（确保不自动播放）
        if (walkAudioSource != null)
        {
            walkAudioSource.playOnAwake = false;
            walkAudioSource.loop = true; // 强制设置为循环，和参考脚本一致
            walkAudioSource.Pause(); // 初始暂停
        }
    }

    void Update()
    {
        // 获取WASD输入（原有逻辑不变）
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(horizontalInput, verticalInput).normalized;

        // 判断是否在移动（和参考脚本isGrounded逻辑对齐，用于控制音效/动画）
        isMoving = moveDirection.magnitude > 0.1f;

        // 控制动画播放/暂停（原有逻辑不变）
        if (anim != null)
        {
            if (isMoving)
            {
                anim.speed = 1f;
                anim.SetFloat("MoveX", moveDirection.x);
                anim.SetFloat("MoveY", moveDirection.y);
            }
            else
            {
                anim.speed = 0f;
            }
        }

        // --- 核心：处理走路音效（完全沿用参考脚本的HandleWalkAudio逻辑）---
        HandleWalkAudio();
    }

    void FixedUpdate()
    {
        // 玩家移动逻辑（原有逻辑不变）
        if (rb != null && isMoving)
        {
            Vector2 targetPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);
        }
    }

    /// <summary>
    /// 处理走路音效（和参考脚本HandleWalkAudio完全一致）
    /// 移动时播放循环音效，停止时暂停音效
    /// </summary>
    void HandleWalkAudio()
    {
        // 空判断：避免未赋值AudioSource导致报错
        if (walkAudioSource == null) return;

        // 只有在移动时，才播放走路音效；停止移动则暂停
        if (isMoving)
        {
            if (!walkAudioSource.isPlaying)
            {
                walkAudioSource.Play(); // 开始播放循环走路音效
            }
        }
        else
        {
            if (walkAudioSource.isPlaying)
            {
                walkAudioSource.Pause(); // 停止移动时暂停音效（保留播放进度，下次移动无缝衔接）
                // 若想完全停止音效（下次重新从头播放），可替换为：walkAudioSource.Stop();
            }
        }
    }
}