using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingScene : MonoBehaviour
{
	private void Awake()
	{
		LevelLoadManager.instance.sceneLoaded += LevelLoadManager_OnSceneLoaded;
	}

	private void Start()
	{
		if (LevelLoadManager.instance.sceneLoadedCompleted)
			LevelLoadManager.instance.ActivateLoadedScene();

		StartCoroutine(nameof(DoLoad));
	}

	private IEnumerator DoLoad()
	{
		yield return new WaitForEndOfFrame();
		LevelLoadManager.instance.LoadSceneAsync();
		yield return null;
	}

	private void OnDestroy()
	{
		LevelLoadManager.instance.sceneLoaded -= LevelLoadManager_OnSceneLoaded;
	}

	private void LevelLoadManager_OnSceneLoaded(object sender, GenericEventArgs<string> e)
	{
		LevelLoadManager.instance.ActivateLoadedScene();
	}
}
