using UnityEngine;

public class DamageIndicatorSpawner : MonoBehaviour
{
	[SerializeField] bool useSpawnOverride = false;
	[SerializeField] Transform _spawnLocation;
	[SerializeField] Color _color = Color.white;
	void Awake()
	{
		_hc = gameObject.GetComponentInParent<HealthComponent>(includeInactive: true);
		_hc.damageTaken += HealthComponent_onDamageTaken;
	}

	void OnDestroy()
	{
		_hc.damageTaken -= HealthComponent_onDamageTaken;
	}

	public void SetSpawnLocation(Transform transform) => _spawnLocation = transform;

	private void HealthComponent_onDamageTaken(object sender, GenericEventArgs<float> e)
	{
		var pos = useSpawnOverride ? _spawnLocation.position : gameObject.transform.position;
		DamageIndicatorPool.instance.SpawnIndicator(pos, e.value, _color);
	}

	HealthComponent _hc;
}
