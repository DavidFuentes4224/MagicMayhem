using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
	[SerializeField] Color[] _playerColors;
	public int playerIndex => _playerInput.user.index;
	public PlayerInput playerInput => _playerInput;
	public Player player => _player;
	// Player is using mouse and keyboard as input.
	public bool usingMK => _playerInput.currentControlScheme == "MK";

	private bool playerInShop;
	private bool playerTalkingToNPC;

	void Awake()
	{
		_playerInput = GetComponent<PlayerInput>();
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		PlayerManager.instance.RegisterPlayer(this);
	}

	public void TakeControl(Player player)
	{
		_player = player;
		_player.Possess(this, _playerColors[playerIndex]);
	}

	public void ReleaseControl()
	{
		if (_player is object)
			_player.Release();
		_player = null;
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		var moveInput = context.ReadValue<Vector2>();
		if (_player)
			_player.MovePlayer(moveInput);
	}

	public void OnCastSpell(InputAction.CallbackContext context)
	{
		if (_player)
			_player.UpdateCasting(context.performed);

	}

	public void OnActivateTrap(InputAction.CallbackContext context)
	{
		if(context.performed && _player)
			_player.ActivateTrap(true);
	}

	public void OnToggleShopUI(InputAction.CallbackContext context)
	{
		if (_player && context.performed && _player.playerInShopRange)
		{
			_playerInput.SwitchCurrentActionMap("UI");
			_player.ToggleShopUI(true);
			playerInShop = true;
		}
	}

	public void OnToggleNPC(InputAction.CallbackContext context)
	{
		if (_player && context.performed && _player.playerInNPCRange)
		{
		//	_playerInput.SwitchCurrentActionMap("NPCInteract");
			_player.ToggleNPCUI(true);
			playerTalkingToNPC = true;
		}
	}

	public void OnCloseNPC(InputAction.CallbackContext context)
	{
		_playerInput.SwitchCurrentActionMap("Gameplay");
		_player.ToggleNPCUI(false);
		playerTalkingToNPC = false;
	}

	public void OnPauseGame(InputAction.CallbackContext context)
	{
		// HACK: If timescale is 0, game is already "paused".
		if (Time.timeScale == 0) return;

		if (_player && context.performed && InGameMenu.instance.RequestPause(this))
		{
			if (playerInShop)
			{
				_player.ToggleShopUI(false);
			}
		}
	}

	public void OnUnpauseGame(InputAction.CallbackContext context)
	{
		if (_player.GameOver())
		{
			return;
		}
		if (_player && context.performed)
			InGameMenu.instance.RequestUnpause(this);
	}

	public void OnSwapStoreTab(InputAction.CallbackContext context)
	{
		if (_player == null) return;
		if (context.performed)
			_player.SwapShopTab();
	}

	public void OnAim(InputAction.CallbackContext context)
	{
		if (_player)
		{
			var dir = Vector2.zero;
			if (_playerInput.currentControlScheme == c_gamepadScheme)
				dir = context.ReadValue<Vector2>();
			_player.SetAiming(context.performed, dir, useMouse: _playerInput.currentControlScheme == c_keyboardScheme);
		}
	}

	public void OnSelectAbility(int slotNumber)
	{
		if (_player)
			_player.SelectAbility(slotNumber);
	}

	public void OnSelectAbilityLeft(InputAction.CallbackContext context)
	{
		if (_player && context.performed)
			_player.SelectAbilityByDirection(SelectAbilityDirection.Left);
	}

	public void OnSelectAbilityRight(InputAction.CallbackContext context)
	{
		if (_player && context.performed)
			_player.SelectAbilityByDirection(SelectAbilityDirection.Right);
	}

	public void OnScrollAbilityLeft(InputAction.CallbackContext context)
	{
		if (_player && context.performed)
			_player.SelectAbilityByDirection(SelectAbilityDirection.Left);
	}

	public void OnScrollAbilityRight(InputAction.CallbackContext context)
	{
		if (_player && context.performed)
			_player.SelectAbilityByDirection(SelectAbilityDirection.Right);
	}

	public void OnRoll()
	{
		if (_player)
			_player.OnRoll();
	}

	public void OnCloseShopUI(InputAction.CallbackContext context)
	{
		_playerInput.SwitchCurrentActionMap("Gameplay");
		_player.ToggleShopUI(false);
		playerInShop = false;
	}
	
	public void ForceCloseActiveShopUI()
	{
		if (_player is null) return;

		_player.ToggleShopUI(false);
		_playerInput.SwitchCurrentActionMap("Gameplay");
		playerInShop = false;
	}

	public void ForceCloseNPC()
	{
		if (_player is null) return;

		_player.ToggleNPCUI(false);
		_playerInput.SwitchCurrentActionMap("Gameplay");
		playerInShop = false;
	}

	public void OnDeviceLost(PlayerInput lostPlayer)
	{
		PlayerManager.instance.OnPlayerLeft(this);
	}

	public void OnDeviceRegained(PlayerInput player)
	{
		PlayerManager.instance.OnPlayerRejoined(this);
	}

	public void DisableMovement()
	{
		_playerInput.SwitchCurrentActionMap("UI");
	}

	public void EnableMovement()
	{
		_playerInput.SwitchCurrentActionMap("Gameplay");
	}

	const string c_gamepadScheme = "Gamepad";
	const string c_keyboardScheme = "MK";

	Player _player;
	PlayerInput _playerInput;
}
