using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    [Header("当前拥有的面具")]
    public MaskType currentActiveMask = MaskType.None;

    [Header("能力解锁状态")]
    public bool canUseAngerPower = false;   // 愤怒：比如攻击力翻倍
    public bool canUseAnxietyPower = false; // 焦虑：比如移速变快/能看到陷阱
    public bool canUseSadnessPower = false; // 悲伤：比如跳得更高/可以穿墙

    void Start()
    {
        GameObject[] lights = GameObject.FindGameObjectsWithTag("StreetLight");
        foreach (GameObject lt in lights)
        {
            Transform litPart = lt.transform.Find("Lit");
            if (litPart != null) litPart.gameObject.SetActive(false);

            Transform fogMask = lt.transform.Find("FogMask");
            if (fogMask != null) fogMask.gameObject.SetActive(false);
        }
    }
    public void UnlockMask(MaskType type)
    {
        currentActiveMask = type;

        switch (type)
        {
            case MaskType.Anger:
                canUseAngerPower = true;
                Debug.Log("解锁了【愤怒】面具：力量爆发！");
                break;
            case MaskType.Anxiety:
                canUseAnxietyPower = true;
                Debug.Log("解锁了【焦虑】面具");
                break;
            case MaskType.Sadness:
                canUseSadnessPower = true;
                Debug.Log("解锁了【悲伤】面具：身轻如燕！");
                break;
        }
    }

    void Update()
    {
        // 如何使用这些功能？示例：
        if (canUseAnxietyPower)
        {
            // 在这里放焦虑面具的功能，比如自动显示附近的隐藏路径
        }
    }
}