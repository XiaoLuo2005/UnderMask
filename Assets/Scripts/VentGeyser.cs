using System.Collections;
using UnityEngine;

public class VentGeyser : MonoBehaviour
{
    [Header("喷气节奏")]
    public float offTime = 1.5f;   // 休息多久
    public float onTime  = 0.6f;   // 喷气多久
    public bool startOn = false;

    [Header("可视效果（可选）")]
    public GameObject visualOn;    // 喷气时显示的特效/子物体（粒子、动画等）

    public bool IsOn { get; private set; }

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
        if (visualOn != null) visualOn.SetActive(on);
    }
}
