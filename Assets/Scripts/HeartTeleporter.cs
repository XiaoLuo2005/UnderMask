using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.Video;

public class HeartTeleporter : MonoBehaviour
{
    // 定义心的类型
    public enum HeartType { Angry, Sad, Anxiety }

    [Header("基础配置")]
    public HeartType type;
    public string targetScene = "StartC"; // 序章场景名

    [Header("返回序章过场视频")]
    public VideoClip returnVideoClip; // 每个游戏返回序章的专属视频

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyHeartEffect();
            MarkGameAsPassed(); // 触发心灯时，标记对应游戏通关
            Invoke("PlayReturnVideoAndQuit", 2f); // 延迟2秒，保证效果展示完整
        }
    }

    /// <summary>
    /// 心灯效果逻辑（可扩展）
    /// </summary>
    void ApplyHeartEffect()
    {
        switch (type)
        {
            case HeartType.Angry:
                Debug.Log("【心灯效果】触碰了愤怒之心：后续可添加攻击力提升等逻辑");
                break;
            case HeartType.Sad:
                Debug.Log("【心灯效果】触碰了悲伤之心：后续可添加移动速度减慢等逻辑");
                break;
            case HeartType.Anxiety:
                Debug.Log("【心灯效果】触碰了焦虑之心：后续可添加视角晃动等逻辑");
                break;
        }
    }

    /// <summary>
    /// 标记对应游戏为已通关（更新全局状态）
    /// </summary>
    void MarkGameAsPassed()
    {
        switch (type)
        {
            case HeartType.Angry:
                GameGlobalStatus.IsAngryGamePassed = true;
                Debug.Log("【通关记录】愤怒游戏已标记为通关");
                break;
            case HeartType.Sad:
                GameGlobalStatus.IsSadGamePassed = true;
                Debug.Log("【通关记录】悲伤游戏已标记为通关");
                break;
            case HeartType.Anxiety:
                GameGlobalStatus.IsAnxietyGamePassed = true;
                Debug.Log("【通关记录】焦虑游戏已标记为通关");
                break;
        }
    }

    /// <summary>
    /// 播放返回序章视频，完成后停止当前场景声音并跳转
    /// </summary>
    void PlayReturnVideoAndQuit()
    {
        // 容错：过场视频管理器不存在，直接跳转并停声音
        if (OvercastVideoManager.Instance == null)
        {
            Debug.LogError("【错误】OvercastVideoManager 实例不存在，无法播放返回视频，直接跳转序章");
            StopCurrentSceneAllAudio();
            SceneManager.LoadScene(targetScene);
            return;
        }

        // 容错：返回视频未赋值，直接跳转并停声音
        if (returnVideoClip == null)
        {
            Debug.LogError("【错误】返回序章视频未赋值，直接跳转序章");
            StopCurrentSceneAllAudio();
            SceneManager.LoadScene(targetScene);
            return;
        }

        // 调用全局管理器播放视频并跳转序章
        OvercastVideoManager.Instance.PlayTransitionVideo(returnVideoClip, targetScene);
        SceneManager.sceneLoaded += OnReturnToTargetScene;
    }

    /// <summary>
    /// 跳转序章后，停止当前小游戏场景的所有声音
    /// </summary>
    private void OnReturnToTargetScene(Scene scene, LoadSceneMode mode)
    {
        StopCurrentSceneAllAudio();
        SceneManager.sceneLoaded -= OnReturnToTargetScene; // 取消订阅，防止内存泄漏
    }

    /// <summary>
    /// 停止当前场景所有非视频相关的音频
    /// </summary>
    private void StopCurrentSceneAllAudio()
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>(true); // true 包含非激活对象
        foreach (AudioSource audioSource in allAudioSources)
        {
            // 跳过视频播放器的音频，避免中断过场视频声音
            if (audioSource.GetComponent<VideoPlayer>() != null)
            {
                continue;
            }
            audioSource.Stop();
            audioSource.loop = false; // 防止后续自动重播
        }
        Debug.Log($"【音频控制】已停止当前场景的 {allAudioSources.Length} 个音频源");
    }

    /// <summary>
    /// 物体销毁时清理订阅
    /// </summary>
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnReturnToTargetScene;
    }
}