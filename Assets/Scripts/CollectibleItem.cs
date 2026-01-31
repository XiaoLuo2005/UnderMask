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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            // 1. 碰到就直接播！
            if (playSound && pickupSound != null)
            {
                // 把声音播放位置设在相机正前方，保证 2D 效果最清晰
                Vector3 playPos = Camera.main.transform.position;
                playPos.z = 0;
                AudioSource.PlayClipAtPoint(pickupSound, playPos);
            }

            // 2. 通知玩家能力解锁
            PlayerAbility playerAbility = other.GetComponent<PlayerAbility>();
            if (playerAbility != null)
            {
                playerAbility.UnlockMask(maskType);
            }

            // 3. 直接消失
            Destroy(gameObject);
        }
    }
}