using UnityEngine;

public class ShooterEnemy : EnemyBase
{
    [Header("Shoot")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootRange = 6f;
    public float shootCooldown = 1.5f;
    public float bulletSpeed = 8f;

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

        if (dist <= shootRange)
        {
            rb.linearVelocity = Vector2.zero; // se queda quieto
            Shoot();
        }
        else
        {
            base.Update(); // patrulla normal
        }
    }

    void Shoot()
    {
        timer -= Time.deltaTime;
        if (timer > 0) return;

        timer = shootCooldown;

        if (!bulletPrefab || !firePoint) return;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();

        float dir = player.position.x > transform.position.x ? 1 : -1;
        rbBullet.linearVelocity = Vector2.right * bulletSpeed * dir;
    }
}
