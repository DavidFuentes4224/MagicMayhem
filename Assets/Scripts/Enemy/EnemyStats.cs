using UnityEngine;

public class EnemyStats : HealthComponent
{
    public float dodgeChance;

    private void Update()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public override void TakeDamage(float damage)
    {
        var randomDodgeChance = Random.Range(0f, 100f);
        if (randomDodgeChance < dodgeChance)
        {
            return;
        }
        base.TakeDamage(damage);
    }
}
