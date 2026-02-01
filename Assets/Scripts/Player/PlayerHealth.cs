using UnityEngine;

public class PlayerHealth :HealthBase
{
    [Header("Player Health Settings")]
    public float invincibilityDuration = 1.0f;
    private bool isInvincible = false;

    [Header("Armor Settings")]
    [SerializeField] private bool hasArmor = false;
    [SerializeField] private Sprite armoredSprite;

    public void EquipArmor()
    {
        hasArmor = true;
        if (armoredSprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = armoredSprite;
        }
    }

    public override void TakeDamage(int amount)
    {
        if (isInvincible) return;
        base.TakeDamage(amount);
       
        isInvincible = true;
        Invoke(nameof(ResetInvencible), invincibilityDuration);
    }

    void ResetInvencible()
    {
        isInvincible = false;
    }

    protected override void Die()
    {
        Debug.Log("Player Died");
        // Implement player death logic here (e.g., respawn, game over screen)
    }
}
