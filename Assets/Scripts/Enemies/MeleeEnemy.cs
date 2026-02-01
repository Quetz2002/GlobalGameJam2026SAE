using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    [Header("Melee")]
    public float chaseRange = 4f;
    public float attackRange = 1.2f;
    public int damage = 1;
    public float attackCooldown = 1f;

    float timer;
    Transform player;

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected override void Update()
    {
        if (!player)
        {
            base.Update();
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= chaseRange && dist > attackRange)
        {
            // Chase
            float dir = player.position.x > transform.position.x ? 1 : -1;
            rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
        }
        else if (dist <= attackRange)
        {
            Attack();
        }
        else
        {
            base.Update(); // vuelve a patrullar
        }
    }

    void Attack()
    {
        timer -= Time.deltaTime;
        if (timer > 0) return;

        timer = attackCooldown;

        // Aqu� luego puedes conectar da�o al player
        Debug.Log("Melee Enemy Attacks!");

        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ph)
        {
            ph.TakeDamage(damage);
        }

    }
}
