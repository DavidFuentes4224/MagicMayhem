using UnityEngine;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour
{
	[SerializeField] EnemyBase _prefab;
	[SerializeField] int _defaultCapacity;
	[SerializeField] int _maxCapacity;
	[SerializeField] bool _collectionCheck;

	public ObjectPool<EnemyBase> pool => _pool;

	private void Awake()
	{
		_pool = new ObjectPool<EnemyBase>(
			createFunc: Create,
			actionOnGet: OnGet,
			actionOnRelease: OnRelease,
			actionOnDestroy: OnDestroyPool,
			_collectionCheck,
			_defaultCapacity,
			_maxCapacity);
	}

	private EnemyBase Create() => Instantiate(_prefab, Vector3.zero, Quaternion.identity, parent: null);
	private void OnGet(EnemyBase goblin) => goblin.gameObject.SetActive(true);
	private void OnRelease(EnemyBase goblin) => goblin.gameObject.SetActive(false);
	private void OnDestroyPool(EnemyBase goblin) => Destroy(goblin.gameObject);

	ObjectPool<EnemyBase> _pool;
}
