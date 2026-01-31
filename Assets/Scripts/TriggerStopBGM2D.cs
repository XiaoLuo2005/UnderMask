using UnityEngine;

/// <summary>
/// 2D碰撞触发关闭BGM，挂在主动碰撞的物体上（比如玩家、子弹）
/// </summary>
public class TriggerStopBGM2D : MonoBehaviour
{
    [Header("触发关闭BGM的目标物体标签")]
    public string targetTag = "Player"; // 自定义标签，比如Finish、Door
    [Header("是否只触发一次（防止来回碰撞重复关闭）")]
    public bool triggerOnce = true;

    private bool hasTriggered = false; // 标记是否已触发，防止重复调用

    // 2D场景优先用触发碰撞（无物理反弹，适合触发事件）
    void OnTriggerEnter2D(Collider2D other)
    {
        // 判定条件：未触发过 + 碰撞的物体标签匹配
        if (!hasTriggered && other.CompareTag(targetTag))
        {
            // 获取BGM实例并调用关闭方法
            BGMPlayer bgm = BGMPlayer.GetInstance();
            if (bgm != null)
            {
                bgm.StopBGM();
                Debug.Log("碰撞触发，已关闭BGM！");
            }
            else
            {
                Debug.LogError("未找到BGMPlayer实例！请检查BGM物体是否挂载了BGMPlayer脚本");
            }

            // 若只触发一次，标记为已触发
            if (triggerOnce)
                hasTriggered = true;
        }
    }

}