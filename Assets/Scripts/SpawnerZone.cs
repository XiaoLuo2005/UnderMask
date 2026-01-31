using UnityEngine;

public class SpawnerZone : MonoBehaviour
{
    public enum ZoneType { Active, Pause }

    [Header("设置此子物体的功能类型")]
    public ZoneType type;
    public string playerTag = "Player";

    private ArrowSpawner mainSpawner;

    void Start()
    {
        // 获取父物体上的脚本组件
        mainSpawner = GetComponentInParent<ArrowSpawner>();
        if (mainSpawner == null)
        {
            Debug.LogError(gameObject.name + " 找不到父物体的 ArrowSpawner 脚本！");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            UpdateStatus(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            UpdateStatus(false);
        }
    }

    void UpdateStatus(bool inside)
    {
        if (mainSpawner == null) return;

        if (type == ZoneType.Active)
            mainSpawner.SetInRange(inside);
        else
            mainSpawner.SetPaused(inside);
    }
}