using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
	public bool playerInShop = false;
	public bool inTutorialScene = false;
	public bool inWaveCooldown;
	public GameObject ShopKeeper;

	void Start()
	{
		if (WaveManager.instance is null || WaveManager.instance.useManager is false) return;

		if (WaveManager.instance.useManager == false)
			inWaveCooldown = true;

		WaveManager.instance.gameStarting += WaveManager_GameStarted;
		WaveManager.instance.waveStarted += WaveManager_WaveStarted;
		WaveManager.instance.waveFinished += WaveManager_WaveFinished;
	}

	private void OnDestroy()
	{
		WaveManager.instance.gameStarting -= WaveManager_GameStarted;
		WaveManager.instance.waveStarted -= WaveManager_WaveStarted;
		WaveManager.instance.waveFinished -= WaveManager_WaveFinished;
	}

	private void WaveManager_GameStarted(object sender, GameStartedEventArgs e)
	{
		inWaveCooldown = false;
	}

	private void WaveManager_WaveStarted(object sender, WaveStartedEventArgs e)
	{
		inWaveCooldown = false;
		playerInShop = false;
		foreach (Player player in PlayerManager.instance.players)
		{
			player.ClearPromptText();
		}
	}

	private void WaveManager_WaveFinished(object sender, WaveEndedEventArgs e)
	{
		inWaveCooldown = true;
	}

	// Trigger the ui based on if the player is inside the collider
	public virtual void OnTriggerEnter(Collider collision)
	{
		if ((collision.CompareTag("Player") && inWaveCooldown) || (collision.CompareTag("Player") && inTutorialScene))
		{
			playerInShop = true;
			var player = collision.gameObject.GetComponentInParent<Player>();
			if (player)
				player.SetPromptText(_openShopTextFormat, ActionToTextMapper.PlayerInputAction.OPENSTORE);
		}
	}

	public virtual void OnTriggerExit(Collider collision)
	{
		if ((collision.CompareTag("Player") && inWaveCooldown) || (collision.CompareTag("Player") && inTutorialScene))
		{
			playerInShop = false;
			var player = collision.gameObject.GetComponentInParent<Player>();
			if (player)
				player.ClearPromptText();
		}
	}

	string _openShopTextFormat = "Press {0} to open shop.";
}
