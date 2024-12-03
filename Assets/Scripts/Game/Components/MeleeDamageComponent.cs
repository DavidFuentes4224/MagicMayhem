using UnityEngine;
using UnityExtensions;

public class MeleeDamageComponent : RefreshableComponent
{
	[SerializeField] LayerMask _hitMask;
	[SerializeField] float _damage;

	private void OnTriggerEnter(Collider other)
	{
		if (!_active) return;
		if (LayerMaskUtility.GameObjectIsInLayer(other.gameObject, _hitMask))
		{
			var hc = other.gameObject.FindComponent<HealthComponent>();
			if (hc != null)
			{
				hc.TakeDamage(_damage);
			}
		}
	}

	public override void OnInit() 
	{
		_active = true;
	}

	public override void OnKilled()
	{
		_active = false;
	}

	bool _active;
}
