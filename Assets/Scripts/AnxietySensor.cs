using UnityEngine;

public class AnxietySensor : MonoBehaviour
{
    private AnxietyEnemy parentEnemy;

    void Start()
    {
        parentEnemy = GetComponentInParent<AnxietyEnemy>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parentEnemy.StartChasing(other.transform);
        }
    }
}