using System.Collections;
using UnityEngine;

public class VentGeyser : MonoBehaviour
{
    [Header("喷气节奏")]
    public float offTime = 1.5f;
    public float onTime = 0.6f;
    public bool startOn = false;

    [Header("可视效果（可选）")]
    public GameObject visualOn;

    [Header("动画")]
    public Animator animator;   // 拖 Animator（或自动找）

    public bool IsOn { get; private set; }

    private void Awake()
    {
        // 防止忘记拖 Animator
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // 初始关闭动画（很重要）
        if (animator != null)
            animator.enabled = false;
    }

    private void Start()
    {
        StartCoroutine(Loop());
    }

    private IEnumerator Loop()
    {
        if (startOn)
        {
            SetOn(true);
            yield return new WaitForSeconds(onTime);
        }

        while (true)
        {
            SetOn(false);
            yield return new WaitForSeconds(offTime);

            SetOn(true);
            yield return new WaitForSeconds(onTime);
        }
    }

    private void SetOn(bool on)
    {
        IsOn = on;

        // 特效显示/隐藏
        if (visualOn != null)
            visualOn.SetActive(on);

        // ⭐ 关键：喷气时才播放动画
        if (animator != null)
            animator.enabled = on;
    }
}
