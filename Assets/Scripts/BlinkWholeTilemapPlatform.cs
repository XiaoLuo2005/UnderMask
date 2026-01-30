using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class BlinkWholeTilemapPlatform : MonoBehaviour
{
    [Header("Timing")]
    public float visibleTime = 2f;
    public float invisibleTime = 2f;
    public bool startVisible = true;

    [Header("Optional Warning Blink")]
    public bool useWarningBlink = false;
    public float warnDuration = 0.5f;
    public float warnInterval = 0.1f;

    TilemapRenderer tileRenderer;
    CompositeCollider2D compositeCollider;
    TilemapCollider2D tilemapCollider;

    void Awake()
    {
        tileRenderer = GetComponent<TilemapRenderer>();
        compositeCollider = GetComponent<CompositeCollider2D>();
        tilemapCollider = GetComponent<TilemapCollider2D>();
    }

    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        bool visible = startVisible;

        while (true)
        {
            SetState(true);
            yield return new WaitForSeconds(visibleTime);

            // 预警闪烁（只闪“看不看得见”，不动碰撞，脚感更好）
            if (useWarningBlink)
            {
                float left = warnDuration;
                bool on = false;
                while (left > 0f)
                {
                    if (tileRenderer) tileRenderer.enabled = on;
                    on = !on;
                    yield return new WaitForSeconds(warnInterval);
                    left -= warnInterval;
                }
            }

            SetState(false);
            yield return new WaitForSeconds(invisibleTime);
        }
    }

    void SetState(bool on)
    {
    if (tileRenderer) tileRenderer.enabled = on;

    if (tilemapCollider) tilemapCollider.enabled = on;
    if (compositeCollider) compositeCollider.enabled = on;

    Debug.Log($"[{name}] SetState({on}) " +
              $"renderer={(tileRenderer ? tileRenderer.enabled : false)} " +
              $"tileCol={(tilemapCollider ? tilemapCollider.enabled : false)} " +
              $"comp={(compositeCollider ? compositeCollider.enabled : false)}");
    }

}
