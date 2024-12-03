using UnityEngine;

public sealed class ShopKeeper : Singleton<ShopKeeper>
{
	public Transform shopkeeperPosition;
	void Start()
	{
		if (WaveManager.instance is null || WaveManager.instance.useManager is false) return;

		transform.gameObject.SetActive(false);
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
		transform.gameObject.SetActive(false);
	}

	private void WaveManager_WaveStarted(object sender, WaveStartedEventArgs e)
	{
		transform.gameObject.SetActive(false);
	}

	private void WaveManager_WaveFinished(object sender, WaveEndedEventArgs e)
	{
		transform.gameObject.SetActive(true);
	}

}