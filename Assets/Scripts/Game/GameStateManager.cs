using System;
using UnityEngine;

public class GameStateManager : Singleton<GameStateManager>
{
	[SerializeField] Transform _spawnPoint;

	/// <summary>
	/// Value is bool representing if game was won.
	/// </summary>
	public event EventHandler<GenericEventArgs<bool>> gameEnded;

	// Start is called before the first frame update
	protected override void DoStart()
	{
		if (PlayerManager.instance is null) return; 
		var playerControllers = PlayerManager.instance.PlayerControllers;
		
		foreach (var controller in playerControllers)
		{
			// This could probably be refactored into a dedicated spawner class.
			var player = PlayerManager.instance.SpawnPlayer(controller);
			FollowCam.instance.targets.Add(player.GetAvatarTransform());
			player.ResetPlayer(_spawnPoint);
			InGameMenu.instance.ConfigureHud(player);
		}

		_alivePlayers = playerControllers.Count;
		SpawnDeadPlayers();
		// Hack for starting from scene other than menu
		if (_alivePlayers == 0)
			PlayerManager.instance.PlayerControllerJoined += PlayerManager_OnPlayerControllerJoined;

		WaveManager.instance.waveFinished += WaveManager_OnWaveFinished;
		PlayerManager.instance.EnableAllMovement();
	}

	private void WaveManager_OnWaveFinished(object sender, WaveEndedEventArgs e)
	{
		SpawnDeadPlayers();
		_alivePlayers = PlayerManager.instance.PlayerControllers.Count;
	}

	private void PlayerManager_OnPlayerControllerJoined(object sender, GenericEventArgs<PlayerController> e)
	{
		_alivePlayers++;
	}

	private void OnDestroy()
	{
		PlayerManager.instance.PlayerControllerJoined -= PlayerManager_OnPlayerControllerJoined;
		WaveManager.instance.waveFinished -= WaveManager_OnWaveFinished;
	}

	public void HandlePlayerDied(Player player)
	{
		_alivePlayers--;
		if (_alivePlayers == 0)
			RaiseGameLost();
		else
			FollowCam.instance.targets.Remove(player.GetAvatarTransform());
	}

	public void RaiseGameLost()
	{
		gameEnded?.Invoke(this, new GenericEventArgs<bool>(false));
	}

	public void RaiseGameWon()
	{
		gameEnded?.Invoke(this, new GenericEventArgs<bool>(true));
	}

	public void SetSpawnPoint(Transform newSpawnPoint)
	{
		_spawnPoint = newSpawnPoint;
	}

	private void SpawnDeadPlayers()
	{
		foreach (var player in PlayerManager.instance.players)
		{
			if (player.PlayerStats.IsAlive == false)
			{
				player.ResetPlayer(_spawnPoint);
				FollowCam.instance.targets.Add(player.GetAvatarTransform());
				player.PlayerStats.gold = Mathf.Max(player.PlayerStats.gold - 50, 0);
			}
		}
	}

	public void TeleportPlayersToNewLevel(Transform newLocation, float minX)
	{
		foreach (var player in PlayerManager.instance.players)
		{
			Transform thisPlayerTransform = player.GetAvatarTransform();
			if (player.PlayerStats.IsAlive == true && thisPlayerTransform.position.x < minX)
			{
				player.TeleportPlayer(newLocation);
			}
		}
	}

	int _alivePlayers;
}
