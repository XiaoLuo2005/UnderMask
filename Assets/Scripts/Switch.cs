using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    // 拖拽场景中的开始按钮到这个槽位
    public Button startButton;
    // 拖拽场景中的退出按钮到这个槽位
    public Button quitButton;
    // 目标场景名称
    public string targetSceneName;

    void Start()
    {
        // 绑定开始按钮（场景跳转）
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClick);
        }
        // 绑定退出按钮（程序退出）
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitButtonClick);
        }
    }

    // 场景跳转核心方法
    private void OnStartButtonClick()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError("目标场景名称未填写！");
        }
    }

    // 退出程序核心方法
    private void OnQuitButtonClick()
    {
        // 两种环境区分处理：
        // 1. 在Unity编辑器中运行时，停止播放模式（方便测试）
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 2. 打包成exe/APK等后，退出整个程序
        Application.Quit();
#endif

        // 调试日志（仅编辑器中可见，确认方法被触发）
        Debug.Log("程序已退出/停止播放模式");
    }
}