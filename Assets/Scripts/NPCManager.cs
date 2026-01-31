using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

[System.Serializable]
public class NPCGroup
{
    // 火焰（碰完返回后消失）
    public GameObject fireAll;
    // 面具（碰完返回后显示）
    public GameObject maskAll;
    // 对应的小游戏场景名
    public string gameSceneName;
    // 对应的过场视频（场景切换前播放）
    public VideoClip transitionVideo;
}

// NPC类型枚举（A/B/C三类）
public enum NPCType { A, B, C }

public class NPCManager : MonoBehaviour
{
    [Header("A类NPC配置")]
    public NPCGroup groupA;

    [Header("B类NPC配置")]
    public NPCGroup groupB;

    [Header("C类NPC配置")]
    public NPCGroup groupC;

    [Header("序章配置")]
    public string prologueSceneName = "Prologue"; // 填写你的序章场景名

    private void Awake()
    {
        // 订阅「场景加载完成」事件，监听返回序章
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// 场景加载完成后触发（仅处理序章返回）
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 只有返回序章场景，才更新火焰/面具状态
        if (scene.name == prologueSceneName)
        {
            ApplyNPCState(groupA, EmotionGameSave.A_Used);
            ApplyNPCState(groupB, EmotionGameSave.B_Used);
            ApplyNPCState(groupC, EmotionGameSave.C_Used);
        }
    }

    /// <summary>
    /// 真正修改火焰/面具的显隐状态（核心：返回序章后才执行）
    /// </summary>
    private void ApplyNPCState(NPCGroup group, bool isGameFinished)
    {
        if (group.fireAll != null)
            group.fireAll.SetActive(!isGameFinished); // 完成游戏 → 火焰消失

        if (group.maskAll != null)
            group.maskAll.SetActive(isGameFinished);  // 完成游戏 → 面具显示
    }

    // A类NPC触发入口（给TriggerOnce调用）
    public void TriggerA()
    {
        TryTriggerNPC(groupA, ref EmotionGameSave.A_Used, NPCType.A);
    }

    // B类NPC触发入口（给TriggerOnce调用）
    public void TriggerB()
    {
        TryTriggerNPC(groupB, ref EmotionGameSave.B_Used, NPCType.B);
    }

    // C类NPC触发入口（给TriggerOnce调用）
    public void TriggerC()
    {
        TryTriggerNPC(groupC, ref EmotionGameSave.C_Used, NPCType.C);
    }

    /// <summary>
    /// 统一处理NPC触发逻辑（防重复、禁用触发器、播放过场视频）
    /// </summary>
    private void TryTriggerNPC(NPCGroup group, ref bool saveFlag, NPCType npcType)
    {
        // 需求3：已完成游戏的NPC，不再触发任何逻辑
        if (saveFlag) return;

        // 1. 标记该类NPC已完成（仅记录，不立刻改显隐）
        saveFlag = true;

        // 2. 需求3：禁用该类所有NPC的触发器，转为普通碰撞体
        DisableNPCTrigger(npcType);

        // 3. 需求2：播放过场视频，完成后跳转小游戏场景
        if (OvercastVideoManager.Instance != null && group.transitionVideo != null)
        {
            OvercastVideoManager.Instance.PlayTransitionVideo(group.transitionVideo, group.gameSceneName);
        }
        else
        {
            // 兜底：无视频管理器/视频时，直接跳转场景
            SceneManager.LoadScene(group.gameSceneName);
        }
    }

    /// <summary>
    /// 禁用指定类型NPC的所有触发器（转为普通碰撞体）
    /// </summary>
    private void DisableNPCTrigger(NPCType npcType)
    {
        TriggerOnce[] allNPCTriggers = FindObjectsOfType<TriggerOnce>();
        foreach (var trigger in allNPCTriggers)
        {
            if (trigger.type == npcType)
            {
                // 获取NPC的碰撞体
                Collider2D npcCollider = trigger.GetComponent<Collider2D>();
                if (npcCollider != null)
                {
                    npcCollider.isTrigger = false; // 取消触发器属性，转为普通碰撞体
                    // 可选：开启静态刚体，让NPC产生碰撞阻挡
                    Rigidbody2D npcRb = trigger.GetComponent<Rigidbody2D>();
                    if (npcRb != null)
                        npcRb.bodyType = RigidbodyType2D.Static;
                }

                // 彻底禁用触发脚本，防止后续意外触发
                trigger.enabled = false;
            }
        }
    }

    private void OnDestroy()
    {
        // 取消订阅场景事件，避免内存泄漏
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}