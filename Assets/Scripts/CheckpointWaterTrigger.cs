using UnityEngine;

public class CheckpointWaterTrigger : MonoBehaviour
{
    public RisingWater water;
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    private void Awake()
    {
        if (water == null)
            water = FindFirstObjectByType<RisingWater>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (water == null) return;
        if (triggerOnce && hasTriggered) return;

        hasTriggered = true;

        water.SaveWaterLevel();
        water.StartRising();

        Debug.Log($"CheckpointWaterTrigger triggered by {collision.name}");
    }
}
