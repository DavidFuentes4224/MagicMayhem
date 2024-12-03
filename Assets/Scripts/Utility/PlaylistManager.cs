using System.Collections;
using UnityEngine;
using static SimpleAudioManager.Manager;

public class PlaylistManager : Singleton<PlaylistManager>
{
	[SerializeField] int _mediumEnemyThreshold = 5;
	[SerializeField] int _largeEnemyThreshold = 15;

	public enum PlaylistKind
	{
		MENU,
		COMBAT,
		SHOP
	};

	protected override void DoStart()
	{
		base.DoStart();
		WaveManager.instance.enemyDied += WaveManager_OnEnemyDied;
		WaveManager.instance.groupSpawned += WaveManager_OnGroupSpawned;
		WaveManager.instance.waveStarted += WaveManager_OnWaveStarted;
		WaveManager.instance.waveFinished += WaveManager_OnWaveFinished;
		GameStateManager.instance.gameEnded += GameStateManager_OnGameEnded;
	}

	private void WaveManager_OnGroupSpawned(object sender, GroupSpawnedEventArgs e)
	{
		_enemiesOnScreen += e.enemiesInGroup;
	}

	private void OnDestroy()
	{
		WaveManager.instance.enemyDied -= WaveManager_OnEnemyDied;
		WaveManager.instance.groupSpawned -= WaveManager_OnGroupSpawned;
		WaveManager.instance.waveStarted -= WaveManager_OnWaveStarted;
		WaveManager.instance.waveFinished -= WaveManager_OnWaveFinished;
		GameStateManager.instance.gameEnded -= GameStateManager_OnGameEnded;
	}

	private void WaveManager_OnWaveFinished(object sender, WaveEndedEventArgs e)
	{
		_currentIntesity = 1;
		StartPlaylist(PlaylistKind.SHOP);
		StartCoroutine(nameof(ShuffleActivePlaylist));
	}

	private void WaveManager_OnWaveStarted(object sender, WaveStartedEventArgs e)
	{
		_totalEnemiesForWave = e.enemyCount;
		_enemiesOnScreen = 0;
		_currentIntesity = 0;
		StartPlaylist(PlaylistKind.COMBAT);
		SetIntensity();
		StartCoroutine(nameof(ShuffleActivePlaylist));
	}

	private void WaveManager_OnEnemyDied(object sender, EnemyDiedEventArgs e)
	{
		_enemiesOnScreen = e.remainingEnemies;
		SetIntensity();
	}

	private void StartPlaylist(PlaylistKind kind) => _currentPlaylistIndicies = kind switch
	{
		PlaylistKind.MENU => _menuSongIndices,
		PlaylistKind.COMBAT => _combatSongIndices,
		PlaylistKind.SHOP => _shopSongIndices,
		_ => null
	};

	private void SetIntensity()
	{
		int intensity = _enemiesOnScreen switch
		{
			_ when (_enemiesOnScreen <= _mediumEnemyThreshold) => 0,
			_ when (_enemiesOnScreen <= _largeEnemyThreshold) => 1,
			_ => 2
		};
		_currentIntesity = intensity;
		SimpleAudioManager.Manager.instance.SetIntensity(intensity);
	}

	private void GameStateManager_OnGameEnded(object sender, GenericEventArgs<bool> won)
	{
		PlaySongAtIntensity(won.value ? _winSongIndex : _loseSongIndex, intensity: 3);
	}

	IEnumerator ShuffleActivePlaylist()
	{
		do
		{
			var targetSong = _currentPlaylistIndicies[Random.Range(0, _currentPlaylistIndicies.Length)];
			if (targetSong != _currentlyPlayingSong)
			{
				PlaySongAtIntensity(targetSong, _currentIntesity);
			}
			yield return new WaitForSeconds(300);
		}
		while (true);
	}

	private void PlaySongAtIntensity(int index, int intensity)
	{
		PlaySongOptions options = new PlaySongOptions()
		{
			intensity = intensity,
			song = index,
			blendOutTime = 2.0f,
			blendInTime = 2.0f,
			startTime = 0,
		};

		SimpleAudioManager.Manager.instance.PlaySong(options);
	}

	int[] _currentPlaylistIndicies;

	int[] _menuSongIndices = { 0,5 };
	int[] _combatSongIndices = { 1,2,4 };
	int[] _shopSongIndices = { 3 };
	int _winSongIndex = 7;
	int _loseSongIndex = 6;

	int _currentlyPlayingSong = -1;
	int _totalEnemiesForWave;
	int _enemiesOnScreen;
	int _currentIntesity;
}
