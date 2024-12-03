using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class InGameMenu : Singleton<InGameMenu>
{
	public GameObject loseText;
	public GameObject winText;
	public GameObject menuPanel;
	public bool gameOver { get; private set; } = false;
	/// <summary>
	/// Setting playerRoot to `null` on MultiplayerEventSystems seems to allow controll of the whole root.
	/// Not ideal. Instead, just set it to this blank object so they can't control anything.
	/// </summary>
	public GameObject blankUI;

	[Header("HUDs")]
	[SerializeField] PlayerHUD []_playerHUDs;
	[SerializeField] Shop[] _shopHUDs;
	[SerializeField] PauseMenu[] _pauseMenus;
	[SerializeField] NPCMenu _npcMenu;

	protected override void DoAwake()
	{
		// initialize with everything turned off
		loseText.SetActive(false);
		winText.SetActive(false);
		menuPanel.SetActive(false);
	}

	protected override void DoStart()
	{
		GameStateManager.instance.gameEnded += GameStateManager_OnGameEnded;
	}

	private void OnDestroy()
	{
		GameStateManager.instance.gameEnded -= GameStateManager_OnGameEnded;
	}

	private void GameStateManager_OnGameEnded(object sender, GenericEventArgs<bool> won)
	{
		PlayerManager.instance.DisableAllMovement();

		var hostController = PlayerManager.instance.PlayerControllers.First();
		hostController.playerInput.SwitchCurrentActionMap("UI");
		if (won.value)
			WinGameMenu();
		else
			LoseGameMenu();
	}

	public bool RequestPause(PlayerController callingPlayer)
	{
		bool succeeded = false;
		if (_playerPausing is null)
		{
			PlayerManager.instance.DisableAllMovement();
			var pauseMenu = _pauseMenus[callingPlayer.playerIndex];
			pauseMenu.OpenPauseMenu();
			_playerPausing = callingPlayer;
			succeeded = true;
		}
		return succeeded;
	}

	public void RequestUnpause(PlayerController callingPlayer)
	{
		if (_playerPausing?.playerIndex == callingPlayer.playerIndex)
		{
			var pauseMenu = _pauseMenus[callingPlayer.playerIndex];
			pauseMenu.ClosePauseMenu();
			PlayerManager.instance.EnableAllMovement();
			_playerPausing = null;
		}
	}

	public void RestartLevel()
	{
		Time.timeScale = 1;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void RestartGame()
	{
		// reload the menu scene and reset time
		LevelLoadManager.instance.QueueScene(LevelLoadManager.menuSceneName);

		SceneManager.LoadScene(LevelLoadManager.loadingSceneName, LoadSceneMode.Single);
		Time.timeScale = 1f;
	}

	public void LoseGameMenu()
	{
		gameOver = true;
		_pauseMenus[0].OpenLoseMenu();
	}

	public void WinGameMenu()
	{
		gameOver = true;
		_pauseMenus[0].OpenWinMenu();
	}

	public void QuitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		 Application.Quit();
#endif
	}

	public void ConfigureHud(Player player)
	{
		var index = player.GetPlayerIndex();

		if (index >= _playerHUDs.Length || index >= _shopHUDs.Length) return;

		var matchingHUD = _playerHUDs[index];
		var matchingShop = _shopHUDs[index];
		var matchingPauseMenu = _pauseMenus[index];
		if (matchingHUD is null || matchingShop is null || matchingPauseMenu is null || _npcMenu is null) return;

		matchingHUD.TrackPlayer(player);
		matchingHUD.gameObject.SetActive(true);
		
		matchingShop.ConfigurePlayer(player);
		player.SetShop(matchingShop);
		player.SetNPCMenu(_npcMenu);

		matchingPauseMenu.ConfigurePlayer(player);
	}

	PlayerController _playerPausing;
}
