using UnityEngine;
using UnityEngine.Pool;

public class DamageIndicatorPool : Singleton<DamageIndicatorPool>
{
	[SerializeField] DamageIndicator _prefab;
	[SerializeField] int _defaultCapacity;
	[SerializeField] int _maxCapacity;
	[SerializeField] bool _collectionCheck;
	public ObjectPool<DamageIndicator> pool => _pool;

	protected override void DoAwake()
	{
		_pool = new ObjectPool<DamageIndicator>(
			createFunc: OnCreate,
			actionOnGet: OnGet,
			actionOnRelease: OnRelease,
			actionOnDestroy: OnDestroyPool,
			_collectionCheck,
			_defaultCapacity,
			_maxCapacity);
	}

	protected override void DoStart()
	{
		NotificationCenter.instance.damageTextFinishedMoving += NotificationCenter_DamageTextFinishedMovementFinished;
	}

	private void OnDestroy()
	{
		NotificationCenter.instance.damageTextFinishedMoving -= NotificationCenter_DamageTextFinishedMovementFinished;
	}

	private void NotificationCenter_DamageTextFinishedMovementFinished(object sender, GenericEventArgs<DamageIndicator> e)
	{

		_pool.Release(e.value);
	}

	public void SpawnIndicator(Vector3 position, float amount, Color color)
	{
		var indicator = _pool.Get();
		indicator.transform.position = position + (Random.insideUnitSphere * 0.5f);
		indicator.SetTextProperties(amount, color);
	}

	private DamageIndicator OnCreate() => Instantiate(_prefab, Vector3.zero, Quaternion.identity, parent: null);
	private void OnGet(DamageIndicator goblin) => goblin.gameObject.SetActive(true);
	private void OnRelease(DamageIndicator goblin) => goblin.gameObject.SetActive(false);
	private void OnDestroyPool(DamageIndicator goblin) => Destroy(goblin.gameObject);

	ObjectPool<DamageIndicator> _pool;
}
