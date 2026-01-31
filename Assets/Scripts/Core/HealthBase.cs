
using UnityEngine;

public class HealthBase : MonoBehaviour
{
    public int maxHp = 3;
    protected int currentHp;

    protected virtual void Awake()
    {
        currentHp = maxHp;
    }

    public virtual void TakeDamage(int amount)
    {
        currentHp -= amount;
        if (currentHp <= 0)
            Die();
    }

    protected virtual void Die(){}
}
