using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class NPCGroup
{
    public GameObject fireAll;    // 一整片火焰（整个GameObject）
    public GameObject maskAll;    // 一整片面具
    public string sceneToLoad;    // 跳转场景名
}

public class NPCManager : MonoBehaviour
{
    [Header("===== A类整片配置 =====")]
    public NPCGroup groupA;

    [Header("===== B类整片配置 =====")]
    public NPCGroup groupB;

    [Header("===== C类整片配置 =====")]
    public NPCGroup groupC;

    private void Start()
    {
        // 进场景就恢复状态
        ApplyVisibility(groupA, GameSave.A_Used);
        ApplyVisibility(groupB, GameSave.B_Used);
        ApplyVisibility(groupC, GameSave.C_Used);
    }

    /// <summary>
    /// 关键改动：
    /// isUsed = true → 火焰消失，面具显示
    /// isUsed = false → 火焰出现，面具隐藏
    /// </summary>
    private void ApplyVisibility(NPCGroup group, bool isUsed)
    {
        if (group.fireAll != null)
            group.fireAll.SetActive(!isUsed); // 已触发 → 火焰消失

        if (group.maskAll != null)
            group.maskAll.SetActive(isUsed);  // 已触发 → 面具显示
    }

    // 触发A类（只一次）
    public void TriggerA()
    {
        if (GameSave.A_Used) return;

        GameSave.A_Used = true;
        ApplyVisibility(groupA, true); // 火焰消失、面具显示

        if (!string.IsNullOrEmpty(groupA.sceneToLoad))
            SceneManager.LoadScene(groupA.sceneToLoad);
    }

    // 触发B类
    public void TriggerB()
    {
        if (GameSave.B_Used) return;

        GameSave.B_Used = true;
        ApplyVisibility(groupB, true);

        if (!string.IsNullOrEmpty(groupB.sceneToLoad))
            SceneManager.LoadScene(groupB.sceneToLoad);
    }

    // 触发C类
    public void TriggerC()
    {
        if (GameSave.C_Used) return;

        GameSave.C_Used = true;
        ApplyVisibility(groupC, true);

        if (!string.IsNullOrEmpty(groupC.sceneToLoad))
            SceneManager.LoadScene(groupC.sceneToLoad);
    }
}