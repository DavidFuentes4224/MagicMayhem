using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	[SerializeField] string sceneToLoad;
	[SerializeField] Animator _cameraAnim;

	[Header("MenuSections")]
	[SerializeField] GameObject _startSection;
	[SerializeField] GameObject _lobbySection;

	[Header("UIReferences")]
	[SerializeField] GameObject _uiFirstSelected;

	[SerializeField] CreditsHandler creditsHandler;

	void Start()
	{
        PlayerManager.instance.SetJoiningEnabled(true);
		PlayerManager.instance.PlayerControllerJoined += PlayerManager_OnPlayerControllerJoined;
		PlayerManager.instance.PlayerControllerRemoved += PlayerManager_OnPlayerControllerRemoved;
		creditsHandler.CreditsEnded += CreditsHandler_OnCreditsEnded;

		LevelLoadManager.instance.sceneLoaded += LevelManager_OnSceneLoaded;
		_menuCharacters = GameObject.FindGameObjectsWithTag("MenuCharacter").Select(c => c.GetComponent<MenuCharacter>()).ToArray();

		// If we already have player controllers connected, this means that players are returning from the game.
		if (PlayerManager.instance.PlayerControllers.Count != 0)
		{
			// Rejoin all returning players.
			foreach(var controller in PlayerManager.instance.PlayerControllers)
			{
				PlayerManager_OnPlayerControllerJoined(this, new GenericEventArgs<PlayerController>(controller));
			}
			_lobbyHostIndex = PlayerManager.instance.PlayerControllers.First().playerIndex;
		}

		StartCoroutine(nameof(IncreaseMusicIntensity));
	}

	void OnDestroy()
	{
		PlayerManager.instance.PlayerControllerJoined -= PlayerManager_OnPlayerControllerJoined;
    }

	public void StartGame()
	{
        if (_isCreditsPlaying) return;

		var hasPlayed = PlayerPrefs.GetInt("RunsPlayed", 0) != 0;
		LevelLoadManager.instance.QueueScene(hasPlayed ? LevelLoadManager.gameSceneName : LevelLoadManager.tutorialSceneName);

		PlayerManager.instance.SetJoiningEnabled(false);
		SceneManager.LoadScene(LevelLoadManager.loadingSceneName);
	}

	public void StartTutorial()
	{
		if (_isCreditsPlaying) return;
		LevelLoadManager.instance.QueueScene(LevelLoadManager.tutorialSceneName);

        PlayerManager.instance.SetJoiningEnabled(false);
        SceneManager.LoadScene(LevelLoadManager.loadingSceneName);
	}

	public void ShowCredits()
	{
        _isCreditsPlaying = true;
        creditsHandler.OnStartCredits();
    }

	public void CreditsHandler_OnCreditsEnded(object sender, GenericEventArgs<bool> e)
	{
		_isCreditsPlaying = false;
    }

	public void QuitGame()
	{
        if (_isCreditsPlaying) return;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	void LevelManager_OnSceneLoaded(object sender, GenericEventArgs<string> args)
	{
		LevelLoadManager.instance.sceneLoaded -= LevelManager_OnSceneLoaded;
		LevelLoadManager.instance.ActivateLoadedScene();
	}

	void PlayerManager_OnPlayerControllerJoined(object sender, GenericEventArgs<PlayerController> e)
	{
		var playerController = e.value;

		var matchingCharacter = _menuCharacters.First(c => c.playerIndex == playerController.playerIndex);
		if (matchingCharacter is null)
		{
			Debug.LogError($"ERROR: Unable to find matching player character for player {playerController.playerIndex}.");
			return;
		}

		matchingCharacter.Join(playerController);

		if (!_hostLoaded)
		{
			_hostLoaded = true;
			_lobbyHostIndex = playerController.playerIndex;
			if (_cameraAnim)
				_cameraAnim.SetTrigger("ToCharacters");
			_startSection.SetActive(false);
			_lobbySection.SetActive(true);
			SetUIControlSchemeFromController(playerController);
		}
	}

	void PlayerManager_OnPlayerControllerRemoved(object sender, GenericEventArgs<int> e)
	{
		var playerIndex = e.value;

		if (PlayerManager.instance.PlayerControllers.Count == 0)
		{
			_hostLoaded = false;
			if (_cameraAnim)
				_cameraAnim.SetTrigger("ToBase");
			_startSection.SetActive(true);
			_lobbySection.SetActive(false);
		}
		else if (playerIndex == _lobbyHostIndex)
		{
			StartCoroutine(nameof(RefreshMenuCharacters));
		}
	}

	void SetUIControlSchemeFromController(PlayerController controller)
	{
		var multiplayerEventSystem = PlayerManager.instance.GetEventSystemForController(controller);
		multiplayerEventSystem.playerRoot = gameObject;
		multiplayerEventSystem.SetSelectedGameObject(controller.usingMK ? null : _uiFirstSelected);
	}

	IEnumerator IncreaseMusicIntensity()
	{
		yield return new WaitForSeconds(0.5f);
		SimpleAudioManager.Manager.instance.SetIntensity(0);
		yield return new WaitForSeconds(15.0f);
		SimpleAudioManager.Manager.instance.SetIntensity(1);
		yield return new WaitForSeconds(15.0f);
		SimpleAudioManager.Manager.instance.SetIntensity(2);
	}

	IEnumerator RefreshMenuCharacters()
	{
		yield return new WaitForEndOfFrame();
		SetUIControlSchemeFromController(PlayerManager.instance.PlayerControllers.First());
		var activeIndexs = PlayerManager.instance.PlayerControllers.Select(c => c.playerIndex).ToList();
		foreach (var controller in PlayerManager.instance.PlayerControllers)
		{
			var matchingCharacter = _menuCharacters[controller.playerIndex];
			matchingCharacter.Join(controller);
		}
		foreach (var character in _menuCharacters.Where(c => !activeIndexs.Contains(c.playerIndex)))
		{
			character.BackOut();
		}
	}

	MenuCharacter[] _menuCharacters;
	int _lobbyHostIndex;

	bool _hostLoaded = false;
	bool _isCreditsPlaying = false;
}
