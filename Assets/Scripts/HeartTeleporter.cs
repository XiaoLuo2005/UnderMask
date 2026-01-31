using UnityEngine;
using UnityEngine.SceneManagement;

public class HeartTeleporter : MonoBehaviour
{
    // 定义心的类型
    public enum HeartType { Angry, Sad, Anxiety }

    [Header("属性设置")]
    public HeartType type;
    public string targetScene = "StartC";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyHeartEffect();
            Invoke("GoToStartC", 0.1f); // 稍微延迟一下，确保效果逻辑先执行
        }
    }

    void ApplyHeartEffect()
    {
        // 根据类型执行不同的逻辑（这里就是你后续加效果的地方）
        switch (type)
        {
            case HeartType.Angry:
                Debug.Log("触碰了愤怒之心：也许会增加攻击力？");
                // 在这里加 Angry 的逻辑
                break;
            case HeartType.Sad:
                Debug.Log("触碰了悲伤之心：也许会减慢移动速度？");
                // 在这里加 Sad 的逻辑
                break;
            case HeartType.Anxiety:
                Debug.Log("触碰了焦虑之心：视角开始晃动？");
                // 在这里加 Anxiety 的逻辑
                break;
        }
    }

    void GoToStartC()
    {
        SceneManager.LoadScene(targetScene);
    }
}