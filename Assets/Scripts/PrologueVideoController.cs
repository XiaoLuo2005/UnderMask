using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

/// <summary>
/// 序章开场视频控制器（仅首次播放，返回不重复播放）
/// </summary>
public class PrologueVideoController : MonoBehaviour
{
    [Header("序章视频组件配置")]
    public VideoPlayer prologueVideoPlayer; // 序章开场视频播放器
    public RawImage prologueVideoRawImage;  // 视频显示RawImage
    public Canvas prologueVideoCanvas;      // 视频全屏Canvas

    private void Start()
    {
        // 初始化：隐藏视频UI（解决你原有代码无法隐藏的问题）
        SetPrologueVideoUIActive(false);

        // 组件校验：自动获取未赋值的组件
        if (prologueVideoPlayer == null)
            prologueVideoPlayer = GetComponent<VideoPlayer>();

        // 需求4：仅首次进入序章时播放视频（通过EmotionGameSave记录状态）
        if (!EmotionGameSave.PrologueVideoPlayed)
        {
            PlayPrologueOpeningVideo();
        }
        else
        {
            // 已播放过视频，直接保持UI隐藏
            SetPrologueVideoUIActive(false);
        }

        // 订阅视频播放完成事件
        if (prologueVideoPlayer != null)
            prologueVideoPlayer.loopPointReached += OnPrologueVideoFinished;
    }

    /// <summary>
    /// 播放序章开场视频（仅首次调用）
    /// </summary>
    private void PlayPrologueOpeningVideo()
    {
        // 1. 显示视频UI
        SetPrologueVideoUIActive(true);

        // 2. 标记视频已播放（存入存档，返回不重复播放）
        EmotionGameSave.PrologueVideoPlayed = true;

        // 3. 开始播放视频
        if (prologueVideoPlayer != null)
            prologueVideoPlayer.Play();
    }

    /// <summary>
    /// 统一控制序章视频UI的显隐
    /// </summary>
    private void SetPrologueVideoUIActive(bool isActive)
    {
        if (prologueVideoCanvas != null)
            prologueVideoCanvas.gameObject.SetActive(isActive);

        if (prologueVideoRawImage != null)
            prologueVideoRawImage.gameObject.SetActive(isActive);
    }

    /// <summary>
    /// 序章视频播放完成后触发
    /// </summary>
    private void OnPrologueVideoFinished(VideoPlayer videoPlayer)
    {
        // 1. 停止视频播放
        videoPlayer.Stop();

        // 2. 隐藏视频UI（核心需求：播放完成后隐藏）
        SetPrologueVideoUIActive(false);
    }

    private void OnDestroy()
    {
        // 取消订阅视频事件，避免内存泄漏
        if (prologueVideoPlayer != null)
            prologueVideoPlayer.loopPointReached -= OnPrologueVideoFinished;
    }
}