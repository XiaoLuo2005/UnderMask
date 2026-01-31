using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

/// <summary>
/// 序章场景全部通关判断脚本：触发最终视频并退出
/// </summary>
public class FinalVideoTrigger : MonoBehaviour
{
    [Header("最终视频配置")]
    public VideoClip finalVideoClip; // 最后一个通关视频（需在Inspector赋值）
    public float stayTimeOnStartC = 2f; // 序章停留时间（秒）

    private bool hasPlayedFinalVideo = false; // 标记是否已播放过最终视频（防止重复触发）

    private void OnEnable()
    {
        // 订阅序章场景加载完成事件（每次返回序章都触发判断）
        SceneManager.sceneLoaded += OnStartCSceneLoaded;
    }

    /// <summary>
    /// 序章场景加载完成后，判断是否全部通关
    /// </summary>
    private void OnStartCSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 仅在序章场景加载完成后执行判断
        if (scene.name != "StartC") return;

        Debug.Log("返回序章，开始检查是否全部游戏通关...");
        CheckAllGamesPassedAndTriggerFinalVideo();
    }

    /// <summary>
    /// 检查全部通关状态，触发最终视频流程
    /// </summary>
    private void CheckAllGamesPassedAndTriggerFinalVideo()
    {
        // 条件：1. 全部通关 2. 未播放过最终视频 3. 最终视频不为空
        if (GameGlobalStatus.CheckAllGamesPassed() && !hasPlayedFinalVideo && finalVideoClip != null)
        {
            Debug.Log("三个游戏全部通关！准备播放最终视频...");
            // 标记已触发，防止重复播放
            hasPlayedFinalVideo = true;
            // 停留2秒后，播放最终视频
            Invoke("PlayFinalVideo", stayTimeOnStartC);
        }
        else if (GameGlobalStatus.CheckAllGamesPassed() && hasPlayedFinalVideo)
        {
            Debug.Log("最终视频已播放过，无需重复触发");
        }
        else if (!GameGlobalStatus.CheckAllGamesPassed())
        {
            Debug.Log("尚未全部通关，继续等待完成剩余游戏");
        }
    }

    /// <summary>
    /// 播放最终视频，播放完退出游戏
    /// </summary>
    private void PlayFinalVideo()
    {
        // 校验全局过场视频管理器
        if (OvercastVideoManager.Instance == null)
        {
            Debug.LogError("OvercastVideoManager 实例不存在！无法播放最终视频，直接退出游戏。");
            QuitGame();
            return;
        }

        // 调用管理器播放最终视频，目标场景传空（无需跳转场景，播放完直接退出）
        OvercastVideoManager.Instance.PlayTransitionVideo(finalVideoClip, string.Empty);

        // 订阅视频播放完成后的回调（播放完退出游戏）
        // 这里借助 OvercastVideoManager 的视频完成逻辑，新增一个专属回调
        OvercastVideoManager.Instance.OnFinalVideoPlayCompleted += QuitGame;
    }

    /// <summary>
    /// 退出游戏（编辑器中打印日志，打包后正常退出）
    /// </summary>
    private void QuitGame()
    {
        Debug.Log("最终视频播放完成，退出游戏！");
        // 取消订阅回调
        OvercastVideoManager.Instance.OnFinalVideoPlayCompleted -= QuitGame;

        // 编辑器模式下停止播放，打包后退出应用
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDisable()
    {
        // 取消订阅场景加载事件
        SceneManager.sceneLoaded -= OnStartCSceneLoaded;
        // 取消订阅最终视频回调
        if (OvercastVideoManager.Instance != null)
        {
            OvercastVideoManager.Instance.OnFinalVideoPlayCompleted -= QuitGame;
        }
    }
}