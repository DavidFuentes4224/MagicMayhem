using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
	public float maxHealth;
	public float health;
	public bool IsAlive => health > 0;
	[SerializeField] RagdollComponent _ragdollComponent;
	[SerializeField] LootDropComponent lootDropComponent;

    public event EventHandler onDeath;
	public event EventHandler<GenericEventArgs<float>> damageTaken;

	public virtual void Awake()
	{
		health = maxHealth;
	}

	public virtual void TakeDamage(float damage)
	{
		health -= damage;
		damageTaken?.Invoke(this, new GenericEventArgs<float>(damage));
		if (health <= 0)
		{
			health = 0;
			HandleDeath();
		}
	}

    public virtual void HandleDeath()
    {
        onDeath?.Invoke(this, null);
	}

	public virtual void Heal(float healAmount)
    {
        health += healAmount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
}
