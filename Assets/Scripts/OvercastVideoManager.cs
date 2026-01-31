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

    // 新增：最终视频播放完成回调事件
    public System.Action OnFinalVideoPlayCompleted;

    // 目标场景名（视频播放完成后跳转）
    private string targetSceneName;

    private void Awake()
    {
        // 原有单例逻辑不变...
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CheckComponentReferences();
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (transitionCanvas != null)
            transitionCanvas.gameObject.SetActive(true);

        SetVideoCanvasVisible(false);

        if (transitionVideoPlayer != null)
            transitionVideoPlayer.loopPointReached += OnTransitionVideoFinished;
    }

    public void PlayTransitionVideo(VideoClip videoClip, string targetScene)
    {
        // 原有校验逻辑不变...
        if (Instance != this)
        {
            Instance.PlayTransitionVideo(videoClip, targetScene);
            return;
        }

        if (transitionVideoPlayer == null)
        {
            Debug.LogError("OvercastVideoManager: transitionVideoPlayer 未绑定！");
            if (!string.IsNullOrEmpty(targetScene)) SceneManager.LoadScene(targetScene);
            return;
        }
        if (videoClip == null)
        {
            Debug.LogError("OvercastVideoManager: 传入的视频片段为null！");
            if (!string.IsNullOrEmpty(targetScene)) SceneManager.LoadScene(targetScene);
            return;
        }

        // 配置视频播放器
        targetSceneName = targetScene;
        transitionVideoPlayer.clip = videoClip;
        transitionVideoPlayer.playOnAwake = false;
        transitionVideoPlayer.isLooping = false;
        transitionVideoPlayer.time = 0;

        // 显示UI并播放视频
        SetVideoCanvasVisible(true);
        transitionVideoPlayer.Play();
    }

    private void OnTransitionVideoFinished(VideoPlayer videoPlayer)
    {
        videoPlayer.Pause();
        SceneManager.sceneLoaded += OnTargetSceneLoaded;

        // 新增：如果是最终视频（目标场景为空），直接触发完成回调，不跳转场景
        if (string.IsNullOrEmpty(targetSceneName))
        {
            OnFinalVideoPlayCompleted?.Invoke();
            // 隐藏UI并重置状态
            SetVideoCanvasVisible(false);
            videoPlayer.Stop();
            videoPlayer.clip = null;
            videoPlayer.time = 0;
            SceneManager.sceneLoaded -= OnTargetSceneLoaded;
            return;
        }

        // 原有跳转场景逻辑（普通过场视频）
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }

    // 原有方法不变...
    private void OnTargetSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetVideoCanvasVisible(false);

        if (transitionVideoPlayer != null)
        {
            transitionVideoPlayer.Stop();
            transitionVideoPlayer.clip = null;
            transitionVideoPlayer.time = 0;
        }

        SceneManager.sceneLoaded -= OnTargetSceneLoaded;
    }

    private void SetVideoCanvasVisible(bool isVisible)
    {
        if (transitionCanvas != null)
            transitionCanvas.gameObject.SetActive(isVisible);

        if (videoRawImage != null)
            videoRawImage.gameObject.SetActive(isVisible);
    }

    private void CheckComponentReferences()
    {
        if (transitionVideoPlayer == null) Debug.LogError("transitionVideoPlayer 未绑定！");
        if (videoRawImage == null) Debug.LogError("videoRawImage 未绑定！");
        if (transitionCanvas == null) Debug.LogError("transitionCanvas 未绑定！");
    }

    private void OnDestroy()
    {
        if (transitionVideoPlayer != null)
            transitionVideoPlayer.loopPointReached -= OnTransitionVideoFinished;
        SceneManager.sceneLoaded -= OnTargetSceneLoaded;
    }
}