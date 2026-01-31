using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 过场视频管理器（场景切换前播放，多视频支持）
/// </summary>
public class OvercastVideoManager : MonoBehaviour
{
    // 单例实例，全局可调用
    public static OvercastVideoManager Instance;

    [Header("过场视频组件配置")]
    public VideoPlayer transitionVideoPlayer; // 过场视频播放器
    public RawImage videoRawImage;            // 视频显示RawImage
    public Canvas transitionCanvas;           // 过场视频全屏Canvas

    // 目标场景名（视频播放完成后跳转）
    private string targetSceneName;

    private void Awake()
    {
        // 单例模式：跨场景保留，全局唯一
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 强制激活Canvas，确保脚本可以控制显隐（可选加固）
        if (transitionCanvas != null)
            transitionCanvas.gameObject.SetActive(true);

        // 初始化：隐藏过场视频UI
        SetVideoCanvasVisible(false);

        // 订阅视频播放完成事件（修正方法名，匹配实际定义）
        if (transitionVideoPlayer != null)
            transitionVideoPlayer.loopPointReached += OnTransitionVideoFinished;
    }

    /// <summary>
    /// 外部调用：播放指定过场视频，完成后跳转目标场景
    /// </summary>
    /// <param name="videoClip">要播放的过场视频</param>
    /// <param name="targetScene">跳转的小游戏场景名</param>
    public void PlayTransitionVideo(VideoClip videoClip, string targetScene)
    {
        // 参数校验：配置不完整则直接跳转场景
        if (transitionVideoPlayer == null || videoClip == null || string.IsNullOrEmpty(targetScene))
        {
            Debug.LogWarning("过场视频配置不完整，直接跳转场景！");
            SceneManager.LoadScene(targetScene);
            return;
        }

        // 记录目标场景，配置视频播放器
        targetSceneName = targetScene;
        transitionVideoPlayer.clip = videoClip;
        transitionVideoPlayer.playOnAwake = false;
        transitionVideoPlayer.isLooping = false;

        // 显示过场视频UI，开始播放视频
        SetVideoCanvasVisible(true);
        transitionVideoPlayer.Play();
    }

    /// <summary>
    /// 过场视频播放完成后触发（不隐藏UI，直接跳转，场景加载完再隐藏）
    /// </summary>
    private void OnTransitionVideoFinished(VideoPlayer videoPlayer)
    {
        // 1. 仅暂停视频（保留最后一帧遮挡序章），不隐藏UI、不停止视频
        videoPlayer.Pause();

        // 2. 订阅场景加载完成事件（关键：场景加载完再隐藏UI）
        SceneManager.sceneLoaded += OnTargetSceneLoaded;

        // 3. 直接跳转场景（视频UI一直遮挡，不会暴露序章）
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }

    /// <summary>
    /// 目标场景加载完成后触发（此时再隐藏视频UI，彻底无闪屏）
    /// </summary>
    private void OnTargetSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. 隐藏过场视频UI（此时已经在小游戏场景，暴露也无所谓）
        SetVideoCanvasVisible(false);

        // 2. 停止视频，重置状态（为下一次播放做准备）
        if (transitionVideoPlayer != null)
        {
            transitionVideoPlayer.Stop();
        }

        // 3. 取消订阅场景事件（避免内存泄漏，防止重复触发）
        SceneManager.sceneLoaded -= OnTargetSceneLoaded;
    }

    /// <summary>
    /// 统一控制过场视频UI的显隐（解决你原有代码无法隐藏的问题）
    /// </summary>
    private void SetVideoCanvasVisible(bool isVisible)
    {
        if (transitionCanvas != null)
            transitionCanvas.gameObject.SetActive(isVisible);

        if (videoRawImage != null)
            videoRawImage.gameObject.SetActive(isVisible);
    }

    private void OnDestroy()
    {
        // 取消订阅视频事件，避免内存泄漏
        if (transitionVideoPlayer != null)
            transitionVideoPlayer.loopPointReached -= OnTransitionVideoFinished;

        // 兜底：取消场景加载事件订阅
        SceneManager.sceneLoaded -= OnTargetSceneLoaded;
    }
}