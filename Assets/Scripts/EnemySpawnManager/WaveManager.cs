using System;
using System.Collections;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

[RequireComponent (typeof(WaveInfo))]
[RequireComponent(typeof(AudioSource))]
public sealed class WaveManager : Singleton<WaveManager>
{
	public bool useManager = true;
	public int timeBeforeGameStarts;
	public int startPlainsLevel;
	public int startGoblinLevel;

	[SerializeField] EnemyFactory _enemyFactory;
	[SerializeField] bool inTestingScene;
	[SerializeField] Transform[] _townSpawnLocations;
	[SerializeField] Transform[] _plainsSpawnLocations;
	[SerializeField] Transform[] _goblinSpawnLocations;

	[System.NonSerialized] public bool inPlaceholderScene = true;
	[System.NonSerialized] public bool gameStarted;
	[System.NonSerialized] public bool inWaveCooldown;
	[System.NonSerialized] public bool inGameStartCooldown;
	[System.NonSerialized] public bool isGameFinished;
	[System.NonSerialized] public bool shouldSkipWaveCooldown;
	[System.NonSerialized] public int currentWaves;
	[System.NonSerialized] public int groupKillCount;
	[System.NonSerialized] public int enemiesAlive;
	[System.NonSerialized] public int totalEnemiesPerWave;
	[System.NonSerialized] public float timeBetweenWaves;

	private int aliveEnemies;
	private int enemiesToSpawn;
	private WaveInfo waveInfo;
	private AudioSource _audioSource;
	private WaveInfo.WaveContent[] waves;

	public event EventHandler<GameStartedEventArgs> gameStarting;
	public event EventHandler<WaveStartedEventArgs> waveStarted;
	public event EventHandler<WaveEndedEventArgs> waveFinished;
	public event EventHandler<EnemyDiedEventArgs> enemyDied;
	public event EventHandler<GroupSpawnedEventArgs> groupSpawned;

	protected override void DoStart()
	{
		if (useManager is false) return;

		isGameFinished = false;
	}

	protected override void DoAwake()
	{
		waveInfo = GetComponent<WaveInfo>();
		_audioSource = GetComponent<AudioSource>();
		waves = waveInfo.GetWaveContents();
		_inGameMenu = FindObjectOfType<InGameMenu>();
		_waveCanvasSettings = FindObjectOfType<WaveCanvasSettings>();
		_npcTrigger = FindObjectOfType<NPCTrigger>();
	}

	public void StartGame()
	{
		gameStarting?.Invoke(this, new GameStartedEventArgs(timeBeforeGameStarts));
		DisableShopDuringWave();
	}
	
	public void SpawnWaves()
	{
		StartCoroutine(DoSpawnWaves());
	}

	IEnumerator DoSpawnWaves()
	{
		var waveNum = 0;
		var groupNum = 0;
		foreach (var wave in waves)
		{
			groupNum = 0;
			var enemyCount = wave.GetEnemyCount();
			enemiesAlive = enemyCount;
			aliveEnemies = 0;
			_audioSource.Play();
			waveStarted?.Invoke(this, new WaveStartedEventArgs(waveNum + 1, enemyCount));
			DisableShopDuringWave();
			foreach (var group in wave.groups)
			{
				aliveEnemies += group.GetEnemyCount();
				Transform location;
				if (waveNum < startPlainsLevel-1)
				{
					location = _townSpawnLocations[UnityRandom.Range(0, _townSpawnLocations.Length)];
				}
				else if(waveNum < startGoblinLevel-1)
				{
					location = _plainsSpawnLocations[UnityRandom.Range(0, _plainsSpawnLocations.Length)];
				}
				else
				{
					location = _goblinSpawnLocations[UnityRandom.Range(0, _goblinSpawnLocations.Length)];
				}
				foreach (var enemy in group.enemySpawn)
				{
					for (int i = 0; i < enemy.amountToSpawn; i++)
					{
						var finalLoc = location.position;
						_enemyFactory.SpawnEnemy(enemy.enemyToSpawn, finalLoc);
						yield return new WaitForSeconds(wave.timeBetweenEnemies);
					}
				}
				groupNum++;
				groupSpawned?.Invoke(this, new GroupSpawnedEventArgs(groupNum, wave.groups.Length, group.GetEnemyCount()));
				yield return new WaitForSecondsOrCondition(
					condition: () => groupNum == wave.groups.Length || aliveEnemies == 0,
					seconds: group.timeToNextGroup);
			}
			yield return new WaitUntil(() => enemiesAlive == 0);
			if (++waveNum != waves.Length)
			{
				waveFinished?.Invoke(this, new WaveEndedEventArgs(wave.timeToNextWave));
				EnableShopAfterWave();
				yield return new WaitForSecondsOrCondition(
					condition: () => shouldSkipWaveCooldown,
					seconds: wave.timeToNextWave);
				shouldSkipWaveCooldown = false;
			}
		}

		StartCoroutine(nameof(GameWin));
	}

	IEnumerator GameWin()
	{
		// Handle Win Menu
		yield return new WaitForSeconds(1.5f);
		GameStateManager.instance.RaiseGameWon();
	}

	public void ReportEnemyKilled()
	{
		enemiesAlive--;
		aliveEnemies--;
		enemyDied?.Invoke(this, new EnemyDiedEventArgs(enemiesAlive));
	}

	public void DisableShopDuringWave()
	{
		if (!inTestingScene)
		{
			inWaveCooldown = false;
			foreach (PlayerController playerController in PlayerManager.instance.PlayerControllers)
			{
				playerController.ForceCloseActiveShopUI();
				playerController.ForceCloseNPC();
			}
		}
	}

	void EnableShopAfterWave()
	{
		inWaveCooldown = true;
		foreach (PlayerController playerController in PlayerManager.instance.PlayerControllers)
		{
			playerController.playerInput.actions.FindAction("OpenShop").Enable();
		}
	}

	public void SkipShopPhase()
	{

		if (_waveCanvasSettings.gameCountdownTextCoroutine != null)
		{
			_waveCanvasSettings.CancelGameCountDown();
		}
		else
		{
			shouldSkipWaveCooldown = true;
		}
		_npcTrigger.playerTalkingToNPC = false;
		foreach (PlayerController playerController in PlayerManager.instance.PlayerControllers)
		{
			playerController.ForceCloseNPC();
		}
	}

	InGameMenu _inGameMenu;
	WaveCanvasSettings _waveCanvasSettings;
	NPCTrigger _npcTrigger;
}

public sealed class WaveStartedEventArgs : EventArgs
{
	public WaveStartedEventArgs(int num, int count)
	{
		waveNum = num;
		enemyCount = count;
	}
	public int waveNum { get; }
	public int enemyCount { get; }
}

public sealed class WaveEndedEventArgs : EventArgs
{
	public WaveEndedEventArgs(int time)
	{
		timeTillNextWave = time;
	}
	public int timeTillNextWave { get; }
}

public sealed class GameStartedEventArgs : EventArgs
{
	public GameStartedEventArgs(int count)
	{
		countDown = count;
	}
	public int countDown { get; }
}

public sealed class GroupSpawnedEventArgs : EventArgs
{
	public GroupSpawnedEventArgs(int current, int total, int enemies)
	{
		currentGroup = current;
		totalGroups = total;
		enemiesInGroup = enemies;
	}
	public int currentGroup { get; }
	public int totalGroups { get; }
	public int enemiesInGroup { get; }
}

public sealed class EnemyDiedEventArgs : EventArgs
{
	public EnemyDiedEventArgs(int count)
	{
		remainingEnemies = count;
	}
	public int remainingEnemies { get; }
}