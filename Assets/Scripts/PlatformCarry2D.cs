using UnityEngine;

public class PlatformCarry2D : MonoBehaviour
{
    Transform platform;

    void Awake()
    {
        platform = transform.parent; // Tilemap
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var respawn = other.GetComponentInParent<PlayerRespawn>();
        if (respawn == null) return;

        respawn.transform.SetParent(platform, true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var respawn = other.GetComponentInParent<PlayerRespawn>();
        if (respawn == null) return;

        if (respawn.transform.parent == platform)
            respawn.transform.SetParent(null, true);
    }
}
