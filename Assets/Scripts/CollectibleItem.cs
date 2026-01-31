using UnityEngine;

// 定义面具类型的枚举
public enum MaskType { None, Anger, Anxiety, Sadness }

public class CollectibleItem : MonoBehaviour
{
    [Header("面具设置")]
    public MaskType maskType = MaskType.None;

    [Header("基础设置")]
    public string targetTag = "Player";
    public bool playSound = true;
    public AudioClip pickupSound; // 确保在 Inspector 里拖入了音频

    [Tooltip("消失时长")]
    public float disappearDelay = 0f;

    public PlayerAttack playerAttack;

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
        if (!other.CompareTag(targetTag)) return;

        // 1) 播放音效（防 Camera.main 为空）
        if (playSound && pickupSound != null)
        {
            Vector3 playPos = (Camera.main != null) ? Camera.main.transform.position : transform.position;
            playPos.z = 0;
            AudioSource.PlayClipAtPoint(pickupSound, playPos);
        }

        // 2) 通知玩家能力解锁（更稳：从父物体找）
        PlayerAbility playerAbility = other.GetComponentInParent<PlayerAbility>();
        if (playerAbility != null)
            playerAbility.UnlockMask(maskType);

        // 3) 解锁攻击（防空引用）
        if (playerAttack != null)
            playerAttack.UnlockAttack();
        else
            Debug.LogWarning($"{name}: playerAttack 没有绑定（Inspector 里没拖），已跳过 UnlockAttack");

        // 4) 直接消失
        Destroy(gameObject);
    }

}