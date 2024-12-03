using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] GameObject _firstSelectedGameObject;

	[Header("UI References")]
	[SerializeField] Image _frame;
	[SerializeField] GameObject _pausePanel;
	[SerializeField] GameObject _winText;
	[SerializeField] GameObject _loseText;

	public void ConfigurePlayer(Player player)
	{
		_player = player;
		_frame.color = player.playerColor;
	}

	public void OpenWinMenu()
	{
		_loseText.SetActive(false);
		_winText.SetActive(true);
		OpenPauseMenu();
	}

	public void OpenLoseMenu()
	{
		_loseText.SetActive(true);
		_winText.SetActive(false);
		OpenPauseMenu();
	}

	public void OpenPauseMenu()
	{
		_player.owningController.DisableMovement();
		var eventSystem = PlayerManager.instance.GetEventSystemForController(_player.owningController);
		eventSystem.SetSelectedGameObject(_player.owningController.usingMK ? null : _firstSelectedGameObject);
		eventSystem.playerRoot = gameObject;
		_pausePanel.SetActive(true);
		Time.timeScale = 0;
	}

	public void ClosePauseMenu()
	{
		_pausePanel.SetActive(false);
		Time.timeScale = 1;
	}

	Player _player;
}
