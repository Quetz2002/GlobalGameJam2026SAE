
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int hp = 3;
    public float speed = 2f;
    public Transform leftPoint;
    public Transform rightPoint;

    bool right = true;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        float dir = right ? 1 : -1;
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);

        if (right && transform.position.x >= rightPoint.position.x)
            right = false;

        if (!right && transform.position.x <= leftPoint.position.x)
            right = true;
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0) Destroy(gameObject);
    }
}
