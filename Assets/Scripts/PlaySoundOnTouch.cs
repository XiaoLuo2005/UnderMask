using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlaySoundOnTouch : MonoBehaviour
{
    [Header("音效设置")]
    public AudioClip clip;          // 拖你的 mp3
    public bool playOnce = true;     // 是否只播放一次
    public float volume = 1f;

    private AudioSource audioSource;
    private bool hasPlayed = false;

    void Awake()
    {
        // 自动添加 AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f; // 2D 音效
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 只响应玩家（更稳：不靠 tag 也可以）
        if (other.GetComponentInParent<PlayerRespawn>() == null)
            return;

        if (playOnce && hasPlayed)
            return;

        audioSource.PlayOneShot(clip);
        hasPlayed = true;
    }
}
