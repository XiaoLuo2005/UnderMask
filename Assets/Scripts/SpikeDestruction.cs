using UnityEngine;

public class SpikeDestruction : MonoBehaviour
{
    [Header("音效设置")]
    public AudioClip destroySound;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // 注意：这里 LayerMask 必须要对应你地刺所在的层级
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, LayerMask.GetMask("Spikes"));

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                OnAttemptDestroy();
            }
        }
    }

    private void OnAttemptDestroy()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        bool hasAnxietyPower = false;

        foreach (GameObject player in players)
        {
            PlayerAbility ability = player.GetComponent<PlayerAbility>();
            if (ability != null && ability.canUseAnxietyPower)
            {
                hasAnxietyPower = true;
                break;
            }
        }

        if (hasAnxietyPower)
        {
            if (destroySound != null)
            {
                AudioSource.PlayClipAtPoint(destroySound, transform.position);
            }

            Debug.Log("看穿了障碍的虚幻，障碍已消失！");

            // --- 核心修改：改为隐藏而不是彻底销毁 ---
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("你感到很焦虑，这些障碍看起来无法逾越...");
        }
    }
}