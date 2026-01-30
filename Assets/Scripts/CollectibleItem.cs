using UnityEngine;

// 定义面具类型的枚举
public enum MaskType { None, Anger, Anxiety, Sadness }

public class CollectibleItem : MonoBehaviour
{
    [Header("面具设置")]
    public MaskType maskType = MaskType.None; // 在编辑器里选择面具类型

    [Header("基础设置")]
    public string targetTag = "Player";
    public bool playSound = true;
    public float disappearDelay = 0f;
    public AudioClip pickupSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && playSound)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            // 核心修改：碰到玩家时，通知玩家开启对应面具能力
            PlayerAbility playerAbility = other.GetComponent<PlayerAbility>();
            if (playerAbility != null)
            {
                playerAbility.UnlockMask(maskType);
            }

            OnPlayerTouch();
        }
    }

    void OnPlayerTouch()
    {
        if (playSound && pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }

        if (disappearDelay > 0)
        {
            DisableVisuals();
            Destroy(gameObject, disappearDelay);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void DisableVisuals()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null) renderer.enabled = false;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
    }
}