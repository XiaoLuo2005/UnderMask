using UnityEngine;
using TMPro; // 若用UGUI的Text则替换为using UnityEngine.UI;

/// <summary>
/// 阀门交互脚本：玩家范围内显示E键提示，按E停止阀门动画
/// </summary>
public class ValveInteraction : MonoBehaviour
{
    [Header("交互设置")]
    [Tooltip("玩家触发交互的距离范围")]
    public float interactionRange = 2f;
    [Tooltip("阀门的动画组件（Animator）")]
    public Animator valveAnimator;
    [Tooltip("动画停止的参数名（建议设为Bool类型）")]
    public string stopAnimationParam = "Stop";
    [Header("阀门管理")]
    public HeartActivator heartActivator;
    [Header("Audio (音效)")]
    public AudioSource valveSource;

    [Header("UI提示设置")]
    [Tooltip("E键提示的UI文本（TMP_Text/Text）")]
    public TMP_Text interactTipText; // 若用UGUI Text则改为public Text interactTipText;

    // 缓存玩家对象和是否在交互范围内
    private Transform playerTransform;
    private bool isInRange = false;
    private bool isValveStopped = false; // 记录阀门是否已停止

    void Awake()
    {
        // 自动获取阀门的Animator（若未手动赋值）
        if (valveAnimator == null)
        {
            valveAnimator = GetComponent<Animator>();
            if (valveAnimator == null)
            {
                Debug.LogWarning($"[{gameObject.name}] 未找到Animator组件，请检查阀门是否挂载动画！");
            }
        }

        // 隐藏初始UI提示
        if (interactTipText != null)
        {
            interactTipText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] 未赋值交互提示UI，请在Inspector中绑定！");
        }

        // 自动获取玩家（标签需设为Player）
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("场景中未找到标签为Player的玩家对象，请检查！");
        }
    }

    void Update()
    {   
        if (!isValveStopped)
        {
            valveSource.Play();
        }
        if (playerTransform == null || valveAnimator == null) return;

        // 1. 检测玩家是否在交互范围内
        CheckPlayerInRange();

        // 2. 范围内按E键触发停止动画
        if (isInRange && Input.GetKeyDown(KeyCode.E) && !isValveStopped)
        {
            StopValveAnimation();
        }
    }

    /// <summary>
    /// 检测玩家是否在交互范围内，并更新UI提示
    /// </summary>
    void CheckPlayerInRange()
    {
        // 计算玩家与阀门的距离
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        isInRange = distanceToPlayer <= interactionRange;

        // 更新UI提示显示/隐藏
        if (interactTipText != null)
        {
            interactTipText.gameObject.SetActive(isInRange && !isValveStopped);
        }
    }

    /// <summary>
    /// 停止阀门动画的核心逻辑
    /// </summary>
    void StopValveAnimation()
    {
        isValveStopped = true;

        valveAnimator.SetBool(stopAnimationParam, true);
        Debug.Log($"[{gameObject.name}] 阀门动画已停止！");

        // 通知管理器
        if (heartActivator != null)
        {
            heartActivator.OnValveStopped();
        }

        if (interactTipText != null)
        {
            interactTipText.gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// 可选：重置阀门动画（如需重新开启）
    /// </summary>
    public void ResetValveAnimation()
    {
        isValveStopped = false;
        valveAnimator.SetBool(stopAnimationParam, false);
        Debug.Log($"[{gameObject.name}] 阀门动画已重置！");
    }

    // 场景视图绘制交互范围Gizmos，方便调试
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}