using UnityEngine;
using TMPro;
using System.Collections; 

/// <summary>
/// 拾取物品提示文字控制脚本，挂载到TMP文本对象上
/// </summary>
public class PickupTip : MonoBehaviour
{
    // 唯一实例（单例，方便其他脚本快速调用显示方法）
    public static PickupTip Instance;
    // 文本组件引用
    private TextMeshProUGUI tipText;
    // 提示文字显示时长（秒），可在Inspector面板调整
    public float showTime = 5f;

    private void Awake()
    {
        // 单例初始化：确保场景中只有一个提示文本实例
        if (Instance == null)
        {
            Instance = this;
            // 让UI对象随场景切换不销毁（可选，根据你的游戏需求）
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        // 获取自身的TMP文本组件
        tipText = GetComponent<TextMeshProUGUI>();
        gameObject.SetActive(false); // 【新增】运行时强制隐藏文本，默认不显示
    }

    /// <summary>
    /// 外部调用的显示提示方法（传入要显示的文字）
    /// </summary>
    /// <param name="tipContent">拾取提示的文字内容</param>
    public void ShowPickupTip(string tipContent)
    {
        // 显示文本对象，设置文字内容
        gameObject.SetActive(true);
        tipText.text = tipContent;
        // 停止之前的延时协程（避免多次拾取时文字提前消失）
        StopCoroutine(HideTipAfterTime());
        // 开启协程，延时隐藏
        StartCoroutine(HideTipAfterTime());
    }

    /// <summary>
    /// 协程：延时指定时间后隐藏文本
    /// </summary>
    /// <returns></returns>
    private IEnumerator HideTipAfterTime()
    {
        // 等待showTime秒
        yield return new WaitForSeconds(showTime);
        // 隐藏文本对象
        gameObject.SetActive(false);
    }
}