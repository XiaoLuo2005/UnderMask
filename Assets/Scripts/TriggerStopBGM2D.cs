using UnityEngine;

public class TriggerStopBGM2D : MonoBehaviour
{
    [Header("碰到谁才关闭BGM（一般是Player）")]
    public string targetTag = "Player";

    [Header("把BGM物体上的 AudioSource 拖到这里")]
    public AudioSource bgmSource;

    public bool triggerOnce = true;
    private bool hasTriggered = false;

    private void Reset()
    {
        // 防呆：如果同物体上有 AudioSource 就自动填（一般用不上）
        if (bgmSource == null) bgmSource = FindFirstObjectByType<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnce && hasTriggered) return;
        if (!other.CompareTag(targetTag)) return;

        if (bgmSource != null)
        {
            bgmSource.Stop();
            Debug.Log("碰撞触发，已关闭BGM！");
            hasTriggered = true;
        }
        else
        {
            Debug.LogError("bgmSource 没有绑定：请在 Inspector 把 BGM 物体上的 AudioSource 拖进来。");
        }
    }
}
