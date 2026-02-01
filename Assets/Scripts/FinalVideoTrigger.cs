using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class FinalVideoTrigger : MonoBehaviour
{
    [Header("最终通关视频")]
    public VideoClip finalVideoClip;
    public float stayTimeOnStartC = 2f;

    private bool hasPlayedFinalVideo = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnStartCSceneLoaded;
    }

    private void OnStartCSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "StartC") return;
        CheckAllGamesPassedAndTriggerFinalVideo();
    }

    private void CheckAllGamesPassedAndTriggerFinalVideo()
    {
        bool allPassed = GameGlobalStatus.CheckAllGamesPassed();
        bool canPlay = allPassed && !hasPlayedFinalVideo && finalVideoClip != null;

        if (canPlay)
        {
            Debug.Log("全部通关，将在2秒后播放最终视频并静音BGM");
            hasPlayedFinalVideo = true;
            Invoke(nameof(PlayFinalVideo), stayTimeOnStartC);
        }
        else if (!allPassed)
        {
            Debug.Log("未全部通关，正常播放BGM");
        }
    }

    private void PlayFinalVideo()
    {
        // 【修改1：优先判断视频片段是否有效，避免空指针】
        if (finalVideoClip == null)
        {
            Debug.LogError("最终视频片段为空，直接退出游戏");
            QuitGame();
            return;
        }

        // 【修改2：标记播放最终视频 → 序章BGM脚本会检测到并停止/不播放】
        GameGlobalStatus.IsPlayingFinalVideo = true;

        // 【核心修改：处理视频管理器，杜绝场景跳转延迟】
        if (OvercastVideoManager.Instance == null)
        {
            Debug.LogWarning("OvercastVideoManager 实例不存在，直接退出游戏");
            QuitGame();
            return;
        }

        // 【修改3：取消场景跳转（传入空字符串时，管理器可能默认返回序章，这里直接禁用跳转逻辑）】
        // 播放视频，但不指定目标场景（避免管理器触发任何场景加载）
        OvercastVideoManager.Instance.PlayTransitionVideo(finalVideoClip, string.Empty);

        // 【修改4：确保回调只订阅一次，且立即执行退出，不等待额外逻辑】
        // 先取消旧订阅，避免重复执行
        OvercastVideoManager.Instance.OnFinalVideoPlayCompleted -= QuitGame;
        // 再订阅新回调，确保视频播放完立即触发退出
        OvercastVideoManager.Instance.OnFinalVideoPlayCompleted += QuitGame;

        Debug.Log("最终视频已开始播放，播放完毕将立即退出游戏");
    }

    private void QuitGame()
    {
        Debug.Log("最终视频播放完毕，立即退出游戏（不返回序章）");

        // 【修改5：立即取消回调订阅，避免内存泄漏，不执行任何额外逻辑】
        if (OvercastVideoManager.Instance != null)
        {
            OvercastVideoManager.Instance.OnFinalVideoPlayCompleted -= QuitGame;
            // 【可选：强制停止视频管理器的所有后续逻辑，杜绝场景跳转】
            OvercastVideoManager.Instance.StopAllCoroutines(); // 停止管理器的所有协程（如场景跳转延迟）
        }

        // 【修改6：立即退出，不等待任何帧更新】
#if UNITY_EDITOR
        // 编辑器内立即停止播放模式，无延迟
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 打包后立即退出应用，无延迟
        Application.Quit();
#endif
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnStartCSceneLoaded;

        if (OvercastVideoManager.Instance != null)
            OvercastVideoManager.Instance.OnFinalVideoPlayCompleted -= QuitGame;
    }
}