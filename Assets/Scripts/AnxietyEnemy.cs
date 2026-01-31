using UnityEngine;
using UnityEngine.UI;

public class AnxietyEnemy : MonoBehaviour
{
    [Header("血量与UI (Slider版)")]
    public int maxHealth = 6;
    public int currentHealth = 6;
    public Slider healthSlider;

    [Header("移动设置")]
    public float moveSpeed = 4f;
    public GameObject spawnerParent;
    public GameObject clearanceObstacle;

    [Header("额外逻辑")]
    // 在 Inspector 中拖入你想要初始隐藏、死后显示的物体（或是该 Prefab 的实例）
    public GameObject specialPrefab;

    private Vector3 startPosition;
    private bool isChasing = false;
    private Transform player;

    void Start()
    {
        startPosition = transform.position;
        InitHealthUI();

        if (specialPrefab != null)
        {
            specialPrefab.SetActive(false);
        }
    }

    void Update()
    {
        if (isChasing && player != null)
        {
            float targetX = Mathf.MoveTowards(transform.position.x, player.position.x, moveSpeed * Time.deltaTime);
            transform.position = new Vector2(targetX, transform.position.y);
        }
    }

    void InitHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
    }

    public void TakeDamage()
    {
        currentHealth--;
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0) Die();
    }

    public void StartChasing(Transform targetPlayer)
    {
        if (!isChasing)
        {
            isChasing = true;
            player = targetPlayer;
            if (spawnerParent != null) spawnerParent.SetActive(true);
        }
    }

    public void ResetEnemy()
    {
        currentHealth = maxHealth;
        transform.position = startPosition;
        isChasing = false;
        if (spawnerParent != null) spawnerParent.SetActive(false);
        if (healthSlider != null) healthSlider.value = maxHealth;

        if (specialPrefab != null) specialPrefab.SetActive(false);

        gameObject.SetActive(true);
    }

    void Die()
    {
        if (clearanceObstacle != null) clearanceObstacle.SetActive(false);

        if (specialPrefab != null)
        {
            specialPrefab.SetActive(true);
        }

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<PlayerRespawn>()?.Respawn();
        }
        else if (other.CompareTag("Spikes"))
        {
            TakeDamage();
            Destroy(other.gameObject);
        }
    }
}