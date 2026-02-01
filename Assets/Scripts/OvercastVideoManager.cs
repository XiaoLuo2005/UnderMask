using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OvercastVideoManager : MonoBehaviour
{
    public static OvercastVideoManager Instance;

    [Header("视频组件绑定")]
    public VideoPlayer transitionVideoPlayer;
    public RawImage videoRawImage;
    public Canvas transitionCanvas;

    public System.Action OnFinalVideoPlayCompleted;

    private string targetSceneName;

    private void Awake()
    {
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
        if (Instance != this)
        {
            Instance.PlayTransitionVideo(videoClip, targetScene);
            return;
        }

        if (transitionVideoPlayer == null)
        {
            Debug.LogError("视频管理器：transitionVideoPlayer未绑定");
            if (!string.IsNullOrEmpty(targetScene)) SceneManager.LoadScene(targetScene);
            return;
        }

        if (videoClip == null)
        {
            Debug.LogError("视频管理器：视频片段为空");
            if (!string.IsNullOrEmpty(targetScene)) SceneManager.LoadScene(targetScene);
            return;
        }

        targetSceneName = targetScene;
        transitionVideoPlayer.clip = videoClip;
        transitionVideoPlayer.playOnAwake = false;
        transitionVideoPlayer.isLooping = false;
        transitionVideoPlayer.time = 0;

        SetVideoCanvasVisible(true);
        transitionVideoPlayer.Play();
    }

    private void OnTransitionVideoFinished(VideoPlayer videoPlayer)
    {
        videoPlayer.Pause();
        SceneManager.sceneLoaded += OnTargetSceneLoaded;

        // 最终视频：标记状态，禁止序章BGM
        if (string.IsNullOrEmpty(targetSceneName))
        {
            GameGlobalStatus.IsPlayingFinalVideo = true;
            OnFinalVideoPlayCompleted?.Invoke();
            ResetVideoAndUI();
            SceneManager.sceneLoaded -= OnTargetSceneLoaded;
            return;
        }

        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }

    private void OnTargetSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetVideoAndUI();
        SceneManager.sceneLoaded -= OnTargetSceneLoaded;
    }

    private void ResetVideoAndUI()
    {
        SetVideoCanvasVisible(false);

        if (transitionVideoPlayer != null)
        {
            transitionVideoPlayer.Stop();
            transitionVideoPlayer.clip = null;
            transitionVideoPlayer.time = 0;
        }
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
        if (transitionVideoPlayer == null) Debug.LogError("transitionVideoPlayer未绑定");
        if (videoRawImage == null) Debug.LogError("videoRawImage未绑定");
        if (transitionCanvas == null) Debug.LogError("transitionCanvas未绑定");
    }

    private void OnDestroy()
    {
        if (transitionVideoPlayer != null)
            transitionVideoPlayer.loopPointReached -= OnTransitionVideoFinished;

        SceneManager.sceneLoaded -= OnTargetSceneLoaded;
        OnFinalVideoPlayCompleted = null;
    }
}