using UnityEngine;
using TMPro;

public class TriggleText : MonoBehaviour
{
    [Header("提示文本设置")]
    [TextArea]
    public string hintText = "这里可以按 E 互动";

    [Header("UI 引用")]
    public TextMeshProUGUI hintTextUI; // 拖你的 TMP 文本

    [Header("行为设置")]
    public bool showOnce = false;      // 是否只显示一次
    public bool hideOnExit = true;     // 离开区域是否隐藏

    private bool hasShown = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (showOnce && hasShown) return;

        if (hintTextUI != null)
        {
            hintTextUI.gameObject.SetActive(true);
            hintTextUI.text = hintText;
            hasShown = true;
        }
        else
        {
            Debug.LogWarning("[AreaHintTrigger] 未绑定 hintTextUI");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!hideOnExit) return;

        if (hintTextUI != null)
        {
            hintTextUI.gameObject.SetActive(false);
        }
    }
}
