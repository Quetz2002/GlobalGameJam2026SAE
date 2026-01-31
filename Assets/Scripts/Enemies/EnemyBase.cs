using UnityEngine;

public class EnemyBase : HealthBase
{
    [Header("Movement")]
    public float speed = 2f;
    public Transform leftPoint;
    public Transform rightPoint;

    protected bool movingRight = true;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        Patrol();
    }

    protected void Patrol()
    {
        float dir = movingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);

        if (movingRight && transform.position.x >= rightPoint.position.x)
            movingRight = false;

        if (!movingRight && transform.position.x <= leftPoint.position.x)
            movingRight = true;
    }

    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
    }
}
