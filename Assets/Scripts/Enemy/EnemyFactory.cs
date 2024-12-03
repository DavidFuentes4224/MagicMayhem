using System;
using System.Collections.Generic;
using UnityEngine;
using static EnemyContent;

public class EnemyFactory : MonoBehaviour
{
	[SerializeField] EnemyPool _goblinPool;
	[SerializeField] EnemyPool _golemPool;
	[SerializeField] EnemyPool _archerPool;
	[SerializeField] EnemyPool _warChiefPool;

	private void Awake()
	{
		if (_goblinPool is null) Debug.LogError("Goblin Pool is missing and should be assigned.");
		if (_golemPool is null) Debug.LogError("Golem Pool is missing and should be assigned.");
		if (_archerPool is null) Debug.LogError("Archer Pool is missing and should be assigned.");
		if (_warChiefPool is null) Debug.LogError("WarChief Pool is missing and should be assigned.");

		_spawnValuesByEnemyToSpawn = new Dictionary<EnemyToSpawn, (Func<EnemyBase> spawnEnemy, Action<EnemyBase> releaseAction)>();
	}

	private void Start()
	{
		_spawnValuesByEnemyToSpawn.Add(EnemyToSpawn.Goblin, (_goblinPool.pool.Get, _goblinPool.pool.Release));
		_spawnValuesByEnemyToSpawn.Add(EnemyToSpawn.WarChief, (_warChiefPool.pool.Get, _warChiefPool.pool.Release));
		_spawnValuesByEnemyToSpawn.Add(EnemyToSpawn.Golem, (_golemPool.pool.Get, _golemPool.pool.Release));
		_spawnValuesByEnemyToSpawn.Add(EnemyToSpawn.Archer, (_archerPool.pool.Get, _archerPool.pool.Release));
	}

	public void SpawnEnemy(EnemyToSpawn enemy, Vector3 location)
	{
		if (!_spawnValuesByEnemyToSpawn.TryGetValue(enemy, out var values))
		{
			Debug.LogError($"Unabled to spawn enemy {enemy}.");
			return;
		}

		(Func<EnemyBase> spawnEnemy, Action<EnemyBase> releaseAction) = values;

		var spawnedEnemy = spawnEnemy?.Invoke();
		if (spawnedEnemy is null) return;

		spawnedEnemy.enabled = true;
		spawnedEnemy.InitFromPool(location, releaseAction);
		spawnedEnemy.gameObject.SetActive(true);
		spawnedEnemy.transform.position = location;
	}

	Dictionary<EnemyToSpawn, (Func<EnemyBase> spawnEnemy, Action<EnemyBase> releaseAction)> _spawnValuesByEnemyToSpawn;
}
