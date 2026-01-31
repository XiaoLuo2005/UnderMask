using UnityEngine;

public class VentBoostZone : MonoBehaviour
{
    [Header("引用")]
    public VentGeyser vent;         // 拖父物体 Vent 上的 VentGeyser

    [Header("冲力设置")]
    public float boostVelocity = 18f;   // 冲上去的目标竖直速度（调大就更高）
    public float cooldown = 0.15f;      // 防止一帧内反复加速

    private float nextBoostTime = 0f;

    private void Reset()
    {
        // 尽量自动绑定（编辑器里也建议手动拖一下更稳）
        vent = GetComponentInParent<VentGeyser>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!vent || !vent.IsOn) return;
        if (!other.CompareTag("Player")) return;
        if (Time.time < nextBoostTime) return;

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null) return;

        // 关键：把竖直速度设置为一个更高的值（比跳跃大）
        Vector2 v = rb.velocity;
        if (v.y < boostVelocity)
        {
            v.y = boostVelocity;
            rb.velocity = v;
        }

        nextBoostTime = Time.time + cooldown;
    }
}
