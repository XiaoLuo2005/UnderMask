using UnityEngine;

public class KillZone : MonoBehaviour
{
    public RisingWater water;

    [Header("KillZone类型")]
    public bool isWater = false;                 // 勾上表示这是水
    public MaskType immuneMask = MaskType.None;  // 哪个面具可以免疫水（None=不免疫）

    private void Start()
    {
        if (water == null)
            water = FindFirstObjectByType<RisingWater>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // 只对“水”做免疫判断，刺/其它陷阱不受影响
        if (isWater)
        {
            var ability = collision.GetComponentInParent<PlayerAbility>();
            if (ability == null) ability = collision.GetComponent<PlayerAbility>();

            // 条件：拥有指定面具（或你也可以换成某个 bool）
            if (ability != null && immuneMask != MaskType.None && ability.currentActiveMask == immuneMask)
            {
                // 拿到面具后：碰水不死（也不回档水位）
                Debug.Log("免疫水：已获得面具 " + immuneMask);
                return;
            }
        }

        // 找Respawn（兼容 Model / 父物体）
        var respawn = collision.GetComponentInParent<PlayerRespawn>();
        if (respawn == null) respawn = collision.GetComponent<PlayerRespawn>();

        if (water != null)
            water.ResetWaterLevel();

        if (respawn != null)
            respawn.Respawn();
    }
}
