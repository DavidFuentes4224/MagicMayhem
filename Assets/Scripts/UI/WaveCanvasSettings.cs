using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class WaveCanvasSettings : MonoBehaviour
{

	public TextMeshProUGUI currentWaveText;
	public TextMeshProUGUI waveCountdownText;
	public TextMeshProUGUI totalEnemiesPerWaveText;
	public TextMeshProUGUI winText;
	[SerializeField] Slider _enemyCountSlider;

	Coroutine countdownTextCoroutine;
	public Coroutine gameCountdownTextCoroutine;

	private void Awake()
	{
		_anim = GetComponent<Animator>();
	}

	void Start()
	{
		if (WaveManager.instance is null || WaveManager.instance.useManager is false) return;

		WaveManager.instance.gameStarting += WaveManager_GameStarted;
		WaveManager.instance.waveStarted += WaveManager_WaveStarted;
		WaveManager.instance.waveFinished += WaveManager_WaveFinished;
		WaveManager.instance.enemyDied += WaveManager_EnemyDied;
		WaveManager.instance.StartGame();
	}

	private void OnDestroy()
	{
		WaveManager.instance.gameStarting -= WaveManager_GameStarted;
		WaveManager.instance.waveStarted -= WaveManager_WaveStarted;
		WaveManager.instance.waveFinished -= WaveManager_WaveFinished;
		WaveManager.instance.enemyDied -= WaveManager_EnemyDied;
	}

	private void WaveManager_GameStarted(object sender, GameStartedEventArgs e)
	{
		gameCountdownTextCoroutine = StartCoroutine(StartGameCountDown(e.countDown));
	}

	private void WaveManager_WaveStarted(object sender, WaveStartedEventArgs e)
	{
		//Cancels countdown early if player ends the shop phase early
		if (countdownTextCoroutine != null)
		{
			StopCoroutine(countdownTextCoroutine);
			countdownTextCoroutine = null;
			waveCountdownText.gameObject.SetActive(false);
		}
		totalEnemiesPerWaveText.gameObject.SetActive(true);
		currentWaveText.gameObject.SetActive(true);

		currentMaxEnemyCount = e.enemyCount;
		UpdateEnemyText(currentMaxEnemyCount);
		currentWaveText.text = $"Wave {e.waveNum}";
		_anim.SetTrigger("ShowCanvas");
	}

	private void WaveManager_WaveFinished(object sender, WaveEndedEventArgs e)
	{
		_anim.SetTrigger("HideCanvas");
		countdownTextCoroutine = StartCoroutine(SetCountDownText(e.timeTillNextWave));
	}

	private void WaveManager_EnemyDied(object sender, EnemyDiedEventArgs e)
	{
		UpdateEnemyText(e.remainingEnemies);
	}

	private void UpdateEnemyText(int count)
	{
		totalEnemiesPerWaveText.text = $"Enemies {count}/{currentMaxEnemyCount}";
		_enemyCountSlider.value = (float)count / (float)currentMaxEnemyCount;
	}

	IEnumerator StartGameCountDown(int start)
	{
		yield return SetCountDownText(start);
		WaveManager.instance.SpawnWaves();
		gameCountdownTextCoroutine = null;
	}

	public void CancelGameCountDown()
	{
		StopCoroutine(gameCountdownTextCoroutine);
		WaveManager.instance.SpawnWaves();
		gameCountdownTextCoroutine = null;
		waveCountdownText.gameObject.SetActive(false);
	}

	IEnumerator SetCountDownText(int start)
	{
		var count = start;
		waveCountdownText.gameObject.SetActive(true);
		totalEnemiesPerWaveText.gameObject.SetActive(false);
		currentWaveText.gameObject.SetActive(false);
		while (count > 0)
		{
			waveCountdownText.text = count.ToString();
			yield return new WaitForSeconds(1);
			count--;
		}
		waveCountdownText.gameObject.SetActive(false);
		countdownTextCoroutine = null;
	}

	int currentMaxEnemyCount;
	Animator _anim;
}
