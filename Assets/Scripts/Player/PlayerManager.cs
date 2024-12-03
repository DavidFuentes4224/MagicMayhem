using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public sealed class PlayerManager : Singleton<PlayerManager>
{
	[Header("Settings")]
	public Player playerPrefab;
	public bool spawnPlayerOnConnect = true;
	[SerializeField] GameObject[] _playerInputSystems;

	public ReadOnlyCollection<PlayerController> PlayerControllers => _playersByOwningController.Keys.ToList().AsReadOnly();
	public ReadOnlyCollection<Player> players => _playersByOwningController.Values.ToList().AsReadOnly();

	public event EventHandler<GenericEventArgs<PlayerController>> PlayerControllerJoined;
	public event EventHandler<GenericEventArgs<int>> PlayerControllerRemoved;

	protected override void DoAwake()
	{
		_playersByOwningController = new Dictionary<PlayerController, Player>();
		_inputManager = GetComponent<PlayerInputManager>();
		_inputSystemByController = new Dictionary<PlayerController, GameObject>();
		DontDestroyOnLoad(gameObject);
	}

	public void RegisterPlayer(PlayerController controller)
	{
		_playersByOwningController.Add(controller, null);
		var matchingPlayerInputSystem = _playerInputSystems[controller.playerIndex];
		matchingPlayerInputSystem.SetActive(true);
		_inputSystemByController.Add(controller, matchingPlayerInputSystem);
		var inputModule = _inputSystemByController[controller].gameObject.GetComponent<InputSystemUIInputModule>();
		controller.playerInput.uiInputModule = inputModule;
		if (spawnPlayerOnConnect)
			SpawnPlayer(controller);

		PlayerControllerJoined?.Invoke(sender:this, new GenericEventArgs<PlayerController>(controller));
	}

	public void RemovePlayer(int index)
	{
		var matchingController = _playersByOwningController.Keys.FirstOrDefault(c => c.playerIndex == index);
		if (matchingController is null) return;

		matchingController.ReleaseControl();
		_playersByOwningController.Remove(matchingController);
		var matchingPlayerInputSystem = _inputSystemByController[matchingController];
		matchingPlayerInputSystem.SetActive(false);
		var eventSystem = GetEventSystemForController(matchingController);
		eventSystem.SetSelectedGameObject(null);
		eventSystem.playerRoot = null;

		Destroy(matchingController.gameObject);

		PlayerControllerRemoved?.Invoke(sender: this, new GenericEventArgs<int>(index));
	}

	public Player SpawnPlayer(PlayerController owningController)
	{
		EnsurePlayerIsRegistered(owningController);

		// Update this to spawn players at designated places around the map.
		// Use tag PlayerControllerClone to find the instantiations of player controller
        // when we need to restart game
		var player = Instantiate(playerPrefab);
		owningController.TakeControl(player);
		owningController.gameObject.tag = "PlayerControllerClone";
		_playersByOwningController[owningController] = player;
		return player;
	}

	public void OnPlayerLeft(PlayerInput input)
	{
		var controller = PlayerControllers.FirstOrDefault(c => c.playerInput == input);
		if (controller)
			OnPlayerLeft(controller);
	}

	public void OnPlayerLeft(PlayerController controller)
	{
		// If player isn't registered, nothing to do.
		if (!_playersByOwningController.TryGetValue(controller, out var existingPlayer))
			return;

		controller.ReleaseControl();
	}

	public void OnPlayerRejoined(PlayerController controller)
	{
		EnsurePlayerIsRegistered(controller);
		var existingPlayer = _playersByOwningController[controller];

		controller.TakeControl(existingPlayer);
	}

	public void SetJoiningEnabled(bool isEnabled)
	{
		if (isEnabled)
			_inputManager.EnableJoining();
		else
			_inputManager.DisableJoining();
	}

	public void DisableAllMovement()
	{
		foreach (var controller in PlayerControllers)
			controller.DisableMovement();
	}

	public void EnableAllMovement()
	{
		foreach (var controller in PlayerControllers)
			controller.EnableMovement();
	}

	public void ClearAllUIRoots()
	{
		foreach(var controller in PlayerControllers)
		{
			var eventSystem = GetEventSystemForController(controller);
			eventSystem.playerRoot = InGameMenu.instance.blankUI;
			eventSystem.SetSelectedGameObject(null);
		}
	}

	public void DisableAllInput() => SetAllInput(isEnabled: false);
	public void EnableAllInput() => SetAllInput(isEnabled: true);

	private void SetAllInput(bool isEnabled)
	{
		foreach (var controller in PlayerControllers)
			GetInputModuleForController(controller).enabled = isEnabled;
	}

	public void AddGold(int amount)
	{
		foreach (var player in _playersByOwningController.Values)
		{
			player.PlayerStats.gold += amount;
		}
	}

	public void ClearAllTextPrompts()
	{
		foreach (var player in _playersByOwningController.Values)
			player.ClearPromptText();
	}

	public InputSystemUIInputModule GetInputModuleForController(PlayerController controller) => _inputSystemByController[controller].gameObject.GetComponent<InputSystemUIInputModule>();
	public MultiplayerEventSystem GetEventSystemForController(PlayerController controller) => _inputSystemByController[controller].gameObject.GetComponent<MultiplayerEventSystem>();

	void EnsurePlayerIsRegistered(PlayerController controller)
	{
		if (!_playersByOwningController.TryGetValue(controller, out var existingPlayer))
			RegisterPlayer(controller);
	}

	Dictionary<PlayerController, Player> _playersByOwningController;
	PlayerInputManager _inputManager;
	Dictionary<PlayerController, GameObject> _inputSystemByController;
}