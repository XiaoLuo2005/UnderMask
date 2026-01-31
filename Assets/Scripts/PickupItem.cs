using UnityEngine;

/// <summary>
/// 可拾取物品脚本，挂载到每个拾取物上
/// </summary>
public class PickupItem : MonoBehaviour
{
    // 拾取该物品时显示的提示文字，可在Inspector面板自定义
    public string pickupTipContent = "拾取了生命药水！";

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检测碰撞的对象是否是玩家（需给玩家对象打标签`Player`）
        if (other.CompareTag("Player"))
        {
            // 调用提示文字的显示方法
            PickupTip.Instance.ShowPickupTip(pickupTipContent);
            // 销毁拾取物品（也可写其他逻辑：添加道具、增加属性等）
            Destroy(gameObject);
            // 可选：播放拾取音效/特效
        }
    }
}