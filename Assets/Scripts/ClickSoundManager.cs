using UnityEngine;

// 全局点击音效管理器（单例模式，全程复用）
public class ClickSoundManager : MonoBehaviour
{
    // 单例实例：全局唯一，方便其他脚本调用
    public static ClickSoundManager Instance;

    [Header("点击音效配置")]
    public AudioClip clickSound; // 拖拽你的点击音效MP3（Audio Clip）
    private AudioSource audioSource; // 音频播放器

    private void Awake()
    {
        // 单例模式：确保场景切换时，这个管理器不被销毁，且只有一个实例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 场景切换不销毁
        }
        else
        {
            Destroy(gameObject); // 销毁重复的实例
            return;
        }

        // 获取Audio Source组件（无需手动拖拽，自动获取）
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 播放点击音效（外部脚本可调用这个方法播放音效）
    /// </summary>
    public void PlayClickSound()
    {
        // 安全判断：音效文件和音频播放器都存在，才播放
        if (clickSound != null && audioSource != null)
        {
            // 播放一次点击音效（不循环）
            audioSource.PlayOneShot(clickSound);
        }
    }
}