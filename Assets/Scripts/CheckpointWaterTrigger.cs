using UnityEngine;

public class CheckpointWaterTrigger : MonoBehaviour
{
    public RisingWater water;

    private void Start()
    {
        if (water == null)
            water = FindFirstObjectByType<RisingWater>(); // Unity 2023+ 推荐
            // 如果你版本旧，用：water = FindObjectOfType<RisingWater>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (water == null) return;

        water.SaveWaterLevel();
        water.StartRising();
    }
}
