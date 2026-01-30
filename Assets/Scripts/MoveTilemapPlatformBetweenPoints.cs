using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MoveTilemapPlatformBetweenPoints : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    Rigidbody2D rb;
    Vector2 a, b;
    bool goToB = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    void Start()
    {
        a = pointA.position;
        b = pointB.position;
        rb.position = a;
    }

    void FixedUpdate()
    {
        Vector2 target = goToB ? b : a;
        Vector2 next = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(next);

        if (Vector2.Distance(next, target) < 0.01f)
            goToB = !goToB;
    }
}
