using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    private static BGMPlayer instance;
    private AudioSource audioSource; // 新增：缓存音频源，避免重复获取

    void Awake()
    {
        // 保证全游戏只有一个 BGM
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // 修改：把局部变量改成成员变量，缓存起来
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    // 新增核心方法：外部可调用的关闭BGM方法
    public void StopBGM()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop(); // 直接关闭BGM
            // 可选：淡入淡出关闭（更丝滑），替换上面一行即可，取消下面注释
            // StartCoroutine(FadeOutBGM(0.5f));
        }
    }

    // 可选：BGM淡出关闭协程（参数是淡出时间，单位秒），需要的话取消注释
    /*
    private IEnumerator FadeOutBGM(float fadeTime)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        audioSource.Stop();
        audioSource.volume = startVolume; // 恢复音量，方便后续重新播放
    }
    */

    // 新增：给外部提供全局访问实例的方法（碰撞脚本要用到）
    public static BGMPlayer GetInstance()
    {
        return instance;
    }
}