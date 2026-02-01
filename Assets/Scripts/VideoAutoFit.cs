using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

/// <summary>
/// 视频自适应屏幕脚本（修复超大问题，确保视频在屏幕内完整显示）
/// 功能：挂载即生效，视频不超出屏幕、无拉伸、黑底填充多余区域
/// </summary>
[RequireComponent(typeof(VideoPlayer))]
public class VideoAutoFit : MonoBehaviour
{
    [Header("=== 仅需填充以下参数 ===")]
    [Tooltip("要播放的视频片段（直接拖拽即可）")]
    public VideoClip videoClip;

    [Header("=== 自适应配置（默认即可）===")]
    [Tooltip("视频背景色（多余区域填充，默认黑色）")]
    public Color bgColor = Color.black;

    [Tooltip("是否自动播放（挂载后进入场景即播放）")]
    public bool autoPlay = true;

    // 组件缓存
    private VideoPlayer _videoPlayer;
    private RawImage _videoRawImage;
    private Canvas _videoCanvas;
    private CanvasScaler _canvasScaler;
    private RenderTexture _renderTexture;

    // 视频原始比例
    private float _videoAspectRatio = 16f / 9f;

    #region 初始化 & 组件查找
    private void Awake()
    {
        FindComponents();
        InitAutoFitSettings();
        BindVideoClip();

        if (autoPlay && videoClip != null)
        {
            _videoPlayer.Play();
        }
    }

    private void FindComponents()
    {
        // 查找VideoPlayer
        _videoPlayer = GetComponent<VideoPlayer>();

        // 查找RawImage
        _videoRawImage = GetComponentInChildren<RawImage>(true);
        if (_videoRawImage == null)
        {
            GameObject rawImageObj = new GameObject("VideoRawImage", typeof(RawImage));
            rawImageObj.transform.SetParent(transform, false);
            _videoRawImage = rawImageObj.GetComponent<RawImage>();
            Debug.LogWarning("【视频自适应】未找到RawImage，已自动创建");
        }

        // 查找/创建Canvas（关键：RawImage必须在Canvas下，且Canvas独立配置）
        _videoCanvas = _videoRawImage.GetComponentInParent<Canvas>();
        if (_videoCanvas == null)
        {
            GameObject canvasObj = new GameObject("VideoCanvas", typeof(Canvas));
            canvasObj.transform.SetParent(transform, false);
            _videoRawImage.transform.SetParent(canvasObj.transform, false);
            _videoCanvas = canvasObj.GetComponent<Canvas>();
        }

        // 查找/添加CanvasScaler
        _canvasScaler = _videoCanvas.GetComponent<CanvasScaler>();
        if (_canvasScaler == null)
        {
            _canvasScaler = _videoCanvas.gameObject.AddComponent<CanvasScaler>();
        }

        // 初始化RenderTexture
        InitRenderTexture();
    }

    private void InitRenderTexture()
    {
        if (_videoPlayer.targetTexture != null)
        {
            _renderTexture = _videoPlayer.targetTexture;
            _videoRawImage.texture = _renderTexture;
            return;
        }

        // 关键：渲染纹理尺寸不超过1080P，避免视频过大
        _renderTexture = new RenderTexture(1920, 1080, 0);
        _videoPlayer.targetTexture = _renderTexture;
        _videoRawImage.texture = _renderTexture;
    }

    private void BindVideoClip()
    {
        if (videoClip != null)
        {
            _videoPlayer.clip = videoClip;
            // 计算视频原始比例（避免除零错误）
            _videoAspectRatio = videoClip.height > 0 ? (float)videoClip.width / (float)videoClip.height : 16f / 9f;
        }
    }
    #endregion

    #region 核心：修复自适应配置（限制视频不超出屏幕）
    private void InitAutoFitSettings()
    {
        // 1. Canvas核心配置：屏幕叠加，不超出屏幕
        _videoCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _videoCanvas.pixelPerfect = false;
        _videoCanvas.planeDistance = 100f;

        // 2. Canvas背景：黑底填充，不遮挡其他UI
        Image canvasBg = _videoCanvas.GetComponent<Image>();
        if (canvasBg == null)
        {
            canvasBg = _videoCanvas.gameObject.AddComponent<Image>();
        }
        canvasBg.color = bgColor;
        canvasBg.raycastTarget = false;

        // 3. CanvasScaler核心配置（关键：约束视频最大尺寸为屏幕尺寸）
        _canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        _canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height); // 参考当前屏幕尺寸，不超屏
        _canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink; // 关键：超出屏幕时收缩，不放大
        _canvasScaler.matchWidthOrHeight = 0.5f;

        // 4. RawImage配置（关键：初始尺寸适配屏幕，不超大）
        _videoRawImage.uvRect = new Rect(0, 0, 1, 1);
        _videoRawImage.raycastTarget = false;

        // 5. RawImage锚点与初始尺寸（关键：限制在屏幕内，不超出）
        RectTransform rawRect = _videoRawImage.rectTransform;
        // 锚点：铺满屏幕但不超出（0,0）到（1,1）
        rawRect.anchorMin = new Vector2(0, 0);
        rawRect.anchorMax = new Vector2(1, 1);
        rawRect.pivot = new Vector2(0.5f, 0.5f);
        rawRect.anchoredPosition = Vector2.zero;
        rawRect.offsetMin = Vector2.zero;
        rawRect.offsetMax = Vector2.zero;

        // 6. VideoPlayer配置：禁止循环，不自动播放
        _videoPlayer.playOnAwake = false;
        _videoPlayer.isLooping = false;
    }
    #endregion

    #region 实时自适应（确保视频始终在屏幕内，不超大）
    private void Update()
    {
        if (_videoPlayer.isPlaying && _videoRawImage.gameObject.activeSelf && _videoPlayer.clip != null)
        {
            FitVideoInScreen();
        }
    }

    /// <summary>
    /// 核心修复：确保视频始终在屏幕内，不超出、不超大
    /// </summary>
    private void FitVideoInScreen()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // 1. 计算视频应显示的尺寸（不超过屏幕尺寸，锁定比例）
        float targetWidth = screenWidth;
        float targetHeight = screenHeight;

        if (_videoAspectRatio > (screenWidth / screenHeight))
        {
            // 视频更宽 → 宽度适配屏幕，高度按比例收缩（不超屏）
            targetHeight = targetWidth / _videoAspectRatio;
        }
        else
        {
            // 视频更高 → 高度适配屏幕，宽度按比例收缩（不超屏）
            targetWidth = targetHeight * _videoAspectRatio;
        }

        // 2. 限制视频最大尺寸为屏幕尺寸（彻底杜绝超大）
        targetWidth = Mathf.Min(targetWidth, screenWidth);
        targetHeight = Mathf.Min(targetHeight, screenHeight);

        // 3. 应用尺寸到RawImage（居中显示，在屏幕内）
        RectTransform rawRect = _videoRawImage.rectTransform;
        // 重置偏移，避免超出屏幕
        rawRect.offsetMin = new Vector2((screenWidth - targetWidth) / 2, (screenHeight - targetHeight) / 2);
        rawRect.offsetMax = new Vector2(-(screenWidth - targetWidth) / 2, -(screenHeight - targetHeight) / 2);
    }
    #endregion

    #region 外部调用：播放/停止视频
    public void PlayVideo()
    {
        if (videoClip != null && !_videoPlayer.isPlaying)
        {
            BindVideoClip();
            _videoPlayer.Play();
        }
    }

    public void StopVideo()
    {
        if (_videoPlayer.isPlaying)
        {
            _videoPlayer.Stop();
        }
    }
    #endregion

    #region 清理资源
    private void OnDestroy()
    {
        if (_renderTexture != null && !_renderTexture.IsCreated()) return;
        _renderTexture?.Release();
    }

    // 屏幕尺寸变化时，重新适配
    private void OnScreenSizeChanged()
    {
        _canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
        if (_videoPlayer.isPlaying)
        {
            FitVideoInScreen();
        }
    }
    #endregion
}