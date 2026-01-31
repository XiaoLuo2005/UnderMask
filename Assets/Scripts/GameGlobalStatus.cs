using UnityEngine;

/// <summary>
/// 全局游戏状态管理器（记录通关状态，跨场景保存）
/// </summary>
public static class GameGlobalStatus
{
    // 三个游戏的通关状态标记（默认未通关：false）
    public static bool IsAngryGamePassed { get; set; } = false;
    public static bool IsSadGamePassed { get; set; } = false;
    public static bool IsAnxietyGamePassed { get; set; } = false;

    /// <summary>
    /// 检查是否三个游戏全部通关
    /// </summary>
    /// <returns>全部通关返回true，否则返回false</returns>
    public static bool CheckAllGamesPassed()
    {
        return IsAngryGamePassed && IsSadGamePassed && IsAnxietyGamePassed;
    }

    /// <summary>
    /// 重置所有通关状态（可选，用于测试）
    /// </summary>
    public static void ResetAllPassStatus()
    {
        IsAngryGamePassed = false;
        IsSadGamePassed = false;
        IsAnxietyGamePassed = false;
        Debug.Log("已重置所有游戏的通关状态为未通关");
    }
}
