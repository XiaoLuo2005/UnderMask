using UnityEngine;

public class HeartActivator : MonoBehaviour
{
    [Header("阀门数量设置")]
    public int totalValves = 3;

    [Header("要激活的爱心对象")]
    public GameObject heart;

    [Header("Audio (音效)")]
    public AudioSource heartSource;

    private int stoppedValveCount = 0;

    private void Awake()
    {
        if (heart != null)
        {
            heart.SetActive(false); // 初始隐藏爱心
        }
    }

    /// <summary>
    /// 被阀门调用：通知“我已经停止了”
    /// </summary>
    public void OnValveStopped()
    {
        stoppedValveCount++;

        Debug.Log($"阀门停止：{stoppedValveCount}/{totalValves}");

        if (stoppedValveCount >= totalValves)
        {
            ActivateHeart();
        }
    }

    private void ActivateHeart()
    {
        if (heart != null && !heart.activeSelf)
        {
            heart.SetActive(true);
            heartSource.Play();
            Debug.Log("❤️ 三个阀门已全部停止，爱心已激活！");
        }
    }
}
