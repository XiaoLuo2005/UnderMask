using UnityEngine;

// 游戏运行期间的存档（场景切换保留，关闭游戏清空）
public static class GameSave
{
    // 每类NPC是否已触发（默认false=未触发，游戏重开自动还原）
    public static bool A_Used { get; set; } = false;
    public static bool B_Used { get; set; } = false;
    public static bool C_Used { get; set; } = false;

    // 视频是否已播放（本次游戏内仅播放一次）
    public static bool Video_Played { get; set; } = false;

    // 可选：重置所有状态（比如“新游戏”按钮可调用）
    public static void ResetAll()
    {
        A_Used = false;
        B_Used = false;
        C_Used = false;
        Video_Played = false;
    }
}