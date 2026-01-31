using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Tooltip("触发消失的标签，默认为Player")]
    public string targetTag = "Player";

    [Tooltip("是否在触发时播放音效")]
    public bool playSound = true;

    [Tooltip("消失延迟时间（秒）")]
    public float disappearDelay = 0f;

    [Tooltip("拾取音效")]
    public AudioClip pickupSound;

    public PlayerAttack playerAttack;

    private AudioSource audioSource;

    void Start()
    {
        // 尝试获取或添加AudioSource组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && playSound)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // 使用OnTriggerEnter2D（需要将碰撞体设为触发器）
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            OnPlayerTouch();
        }
    }

    // 使用OnCollisionEnter2D（需要物理碰撞）
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("撞到了：" + collision.gameObject.name);
        if (collision.gameObject.CompareTag(targetTag))
        {
            OnPlayerTouch();
        }
    }

    void OnPlayerTouch()
    {
        // 播放音效
        if (playSound && pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }

        // 延迟销毁物体
        if (disappearDelay > 0)
        {
            // 先禁用渲染和碰撞，让物体看起来消失了
            DisableVisuals();
            Destroy(gameObject, disappearDelay);
        }
        else
        {
            Destroy(gameObject);
        }
        playerAttack.UnlockAttack();
    }

    void DisableVisuals()
    {
        // 禁用渲染器，让物体不可见
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null) renderer.enabled = false;

        // 禁用碰撞体，防止重复触发
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
    }
}