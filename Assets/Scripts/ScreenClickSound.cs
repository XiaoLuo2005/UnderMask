using UnityEngine;

public class ScreenClickSound : MonoBehaviour
{
    private void Update()
    {
        // 检测鼠标左键点击（屏幕任意位置）
        if (Input.GetMouseButtonDown(0))
        {
            // 调用全局音频管理器的方法，播放点击音效
            ClickSoundManager.Instance?.PlayClickSound();
        }
    }
}