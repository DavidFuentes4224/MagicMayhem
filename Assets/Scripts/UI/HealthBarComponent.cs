using UnityEngine;
using UnityEngine.UI;

// Every health bar should use a damage indicator, but not every damage indicator requires a health bar.
[RequireComponent(typeof(DamageIndicatorSpawner))]
public class HealthBarComponent : RefreshableComponent
{
	[SerializeField] Slider _healthBar;
	void Awake()
	{
		_hc = gameObject.GetComponentInParent<HealthComponent>(includeInactive: true);
		_hc.damageTaken += HealthComponent_onDamageTaken;
	}

	void OnDestroy()
	{
		_hc.damageTaken -= HealthComponent_onDamageTaken;
	}

	public override void OnInit()
	{
		DoInit();
	}

	public override void OnKilled()
	{
		HideBar();
	}

	private void DoInit()
	{
		_healthBar.value = 1.0f;
		gameObject.SetActive(true);
	}

	private void HideBar()
	{
		gameObject.SetActive(false);
	}

	private void HealthComponent_onDamageTaken(object sender, GenericEventArgs<float> e)
	{
		_healthBar.value = _hc.health / _hc.maxHealth;
	}

	HealthComponent _hc;
}
