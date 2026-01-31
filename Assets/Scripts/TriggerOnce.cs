using UnityEngine;

public class TriggerOnce : MonoBehaviour
{
    [Header("该触发区属于哪一类NPC")]
    public NPCType type; // 选择A、B、C

    // 玩家触碰触发区时调用
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 只响应标签为“Player”的对象（即我们的玩家）
        if (!other.CompareTag("Player")) return;

        // 找到场景中的总管理器
        NPCManager npcManager = FindObjectOfType<NPCManager>();

        // 根据触发区类型，执行对应NPC的触发逻辑
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