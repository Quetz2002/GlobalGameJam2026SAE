
using UnityEngine;

public class PlayerMeleeCombo : MonoBehaviour
{
    public Transform meleePoint;
    public float radius = 0.6f;
    public LayerMask enemyLayer;

    int combo = 0;
    float timer;
    public float resetTime = 0.6f;

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0) combo = 0;

        if (Input.GetKeyDown(KeyCode.C))
            Attack();
    }

    void Attack()
    {
        timer = resetTime;
        combo = (combo + 1) % 3;

        Collider2D[] hits = Physics2D.OverlapCircleAll(meleePoint.position, radius, enemyLayer);

        foreach (var hit in hits)
        {
            EnemyBase e = hit.GetComponent<EnemyBase>();
            if (e) e.TakeDamage(1);
        }
    }
}
