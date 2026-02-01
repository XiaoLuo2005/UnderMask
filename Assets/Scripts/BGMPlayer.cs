using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 全局唯一BGM管理器（只挂载一次，解决多实例叠加问题）
/// 存储所有场景的BGM，切换场景自动切换BGM，无叠加
/// </summary>
public class BGMPlayer : MonoBehaviour
{
    // 全局唯一实例（旧代码兼容 + 新代码直接访问）
    public static BGMPlayer instance { get; private set; }
    public static BGMPlayer GetInstance() => instance;

    // 【核心】所有场景的BGM配置（在Inspector面板统一配置）
    [Header("=== 所有场景BGM配置列表 ===")]
    public SceneBgmConfig[] allSceneBgmConfigs;

    // 缓存音频源
    private AudioSource _audioSource;
    // 缓存当前场景的BGM配置
    private SceneBgmConfig _currentSceneBgmConfig;

    /// <summary>
    /// 单个场景的BGM配置类
    /// </summary>
    [System.Serializable]
    public class SceneBgmConfig
    {
        public string sceneName; // 场景名（和Build Settings一致）
        public AudioClip bgmClip; // 该场景的BGM
        [Range(0f, 1f)] public float volume = 0.5f; // 该场景的音量
    }

    void Awake()
    {
        // 全局单例：确保只有一个BGM管理器，彻底解决多实例叠加
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // 初始化音频源
        InitAudioSource();

        // 初始化当前场景BGM
        InitCurrentSceneBgm(SceneManager.GetActiveScene().name);
    }

    void OnEnable()
    {
        // 订阅场景加载事件（只订阅一次，全局生效）
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// 初始化音频源
    /// </summary>
    private void InitAudioSource()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        _audioSource.loop = true;
        _audioSource.playOnAwake = false;
    }

    /// <summary>
    /// 初始化当前场景的BGM配置
    /// </summary>
    private void InitCurrentSceneBgm(string sceneName)
    {
        // 从配置列表中找到当前场景的BGM
        _currentSceneBgmConfig = FindSceneBgmConfig(sceneName);

        if (_currentSceneBgmConfig == null || _currentSceneBgmConfig.bgmClip == null)
        {
            _audioSource.Stop();
            Debug.Log($"【BGM管理器】场景 {sceneName} 无有效BGM配置，停止播放。");
            return;
        }

        // 配置并播放当前场景BGM
        _audioSource.clip = _currentSceneBgmConfig.bgmClip;
        _audioSource.volume = _currentSceneBgmConfig.volume;
        _audioSource.Play();
        Debug.Log($"【BGM管理器】初始化场景 {sceneName} BGM，开始播放。");
    }

    /// <summary>
    /// 场景加载完成时：切换到对应场景BGM（先停旧的，再播新的）
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. 先停止当前正在播放的BGM（无残留）
        _audioSource.Stop();

        // 2. 初始化并播放新场景的BGM
        InitCurrentSceneBgm(scene.name);
    }

    /// <summary>
    /// 从配置列表中查找指定场景的BGM配置
    /// </summary>
    private SceneBgmConfig FindSceneBgmConfig(string sceneName)
    {
        foreach (var config in allSceneBgmConfigs)
        {
            if (config.sceneName == sceneName)
            {
                return config;
            }
        }
        Debug.LogWarning($"【BGM管理器】未找到场景 {sceneName} 的BGM配置，请检查配置列表。");
        return null;
    }

    // 外部手动控制：停止BGM（兼容你的旧脚本）
    public void StopBGM()
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
    }

    // 外部手动控制：播放当前场景BGM（兼容你的旧脚本）
    public void PlayBGM()
    {
        if (_currentSceneBgmConfig != null && _currentSceneBgmConfig.bgmClip != null && !_audioSource.isPlaying)
        {
            _audioSource.Play();
        }
    }
}