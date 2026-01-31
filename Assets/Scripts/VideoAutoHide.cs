using UnityEngine;
using UnityEngine.Video;

public class VideoAutoHide : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    void Start()
    {
        // 获取物体上的VideoPlayer组件
        videoPlayer = GetComponent<VideoPlayer>();

        // 订阅「视频播放完毕」事件（播放到末尾时触发）
        videoPlayer.loopPointReached += OnVideoPlayFinished;
    }

    /// <summary>
    /// 视频播放完成后调用
    /// </summary>
    /// <param name="vp">当前播放的VideoPlayer</param>
    void OnVideoPlayFinished(VideoPlayer vp)
    {
        // 核心逻辑：禁用当前视频物体（RawImage+VideoPlayer），直接消失
        // 禁用后会立刻隐藏视频，露出场景原本的内容
        gameObject.SetActive(false);

        // 可选：如果后续需要再次播放视频，可保留物体，仅隐藏RawImage（不销毁/禁用物体）
        // GetComponent<RawImage>().enabled = false;
        // videoPlayer.Stop();
    }
}