using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.Video;

public class HeartTeleporter : MonoBehaviour
{
    // 定义心的类型
    public enum HeartType { Angry, Sad, Anxiety }

    [Header("属性设置")]
    public HeartType type;
    public string targetScene = "StartC"; // 序章场景名

    [Header("返回过场视频配置")]
    public VideoClip returnVideoClip; // 返回序章的过场视频（需在Inspector赋值）

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyHeartEffect();
            // 新增：触发心灯时，标记对应游戏为已通关
            MarkGameAsPassed();
            // 延迟2秒后，执行播放返回视频的逻辑
            Invoke("PlayReturnVideoAndQuit", 2f);
        }
    }

    void ApplyHeartEffect()
    {
        // 原有逻辑不变...
        switch (type)
        {
            case HeartType.Angry:
                Debug.Log("触碰了愤怒之心：也许会增加攻击力？");
                break;
            case HeartType.Sad:
                Debug.Log("触碰了悲伤之心：也许会减慢移动速度？");
                break;
            case HeartType.Anxiety:
                Debug.Log("触碰了焦虑之心：视角开始晃动？");
                break;
        }
    }

    /// <summary>
    /// 新增：标记对应游戏为已通关
    /// </summary>
    void MarkGameAsPassed()
    {
        switch (type)
        {
            case HeartType.Angry:
                GameGlobalStatus.IsAngryGamePassed = true;
                Debug.Log("愤怒游戏已标记为通关");
                break;
            case HeartType.Sad:
                GameGlobalStatus.IsSadGamePassed = true;
                Debug.Log("悲伤游戏已标记为通关");
                break;
            case HeartType.Anxiety:
                GameGlobalStatus.IsAnxietyGamePassed = true;
                Debug.Log("焦虑游戏已标记为通关");
                break;
        }
    }

    /// <summary>
    /// 播放返回序章的过场视频（原有逻辑不变）
    /// </summary>
    void PlayReturnVideoAndQuit()
    {
        if (OvercastVideoManager.Instance == null)
        {
            Debug.LogError("OvercastVideoManager 实例不存在！无法播放返回序章的过场视频，直接跳转。");
            StopCurrentSceneAllAudio();
            SceneManager.LoadScene(targetScene);
            return;
        }

        if (returnVideoClip == null)
        {
            Debug.LogError("返回序章的过场视频未赋值！直接跳转序章。");
            StopCurrentSceneAllAudio();
            SceneManager.LoadScene(targetScene);
            return;
        }

        OvercastVideoManager.Instance.PlayTransitionVideo(returnVideoClip, targetScene);
        SceneManager.sceneLoaded += OnReturnToTargetScene;
    }

    /// <summary>
    /// 跳回序章后，停止原小游戏场景的所有声音（原有逻辑不变）
    /// </summary>
    private void OnReturnToTargetScene(Scene scene, LoadSceneMode mode)
    {
        StopCurrentSceneAllAudio();
        SceneManager.sceneLoaded -= OnReturnToTargetScene;
    }

    /// <summary>
    /// 停止当前场景的所有声音（原有逻辑不变）
    /// </summary>
    private void StopCurrentSceneAllAudio()
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>(true);
        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource.GetComponent<VideoPlayer>() != null)
            {
                continue;
            }
            audioSource.Stop();
            audioSource.loop = false;
        }
        Debug.Log($"已停止当前场景的 {allAudioSources.Length} 个音频源。");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnReturnToTargetScene;
    }
}