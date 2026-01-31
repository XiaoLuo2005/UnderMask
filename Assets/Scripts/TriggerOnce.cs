using UnityEngine;

/// <summary>
/// NPC单次触发脚本（触发后转为普通碰撞体，不再重复响应）
/// </summary>
public class TriggerOnce : MonoBehaviour
{
    [Header("NPC配置")]
    public NPCType type; // 选择NPC类型（A/B/C）
    public NPCManager npcManager; // 关联NPCManager脚本

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 仅响应玩家触发
        if (other.CompareTag("Player") && npcManager != null)
        {
            // 根据NPC类型，调用对应触发逻辑
            switch (type)
            {
                case NPCType.A:
                    npcManager.TriggerA();
                    break;
                case NPCType.B:
                    npcManager.TriggerB();
                    break;
                case NPCType.C:
                    npcManager.TriggerC();
                    break;
            }
        }
    }
}