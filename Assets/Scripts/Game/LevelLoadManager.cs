using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class LevelLoadManager : Singleton<LevelLoadManager>
{
	public event EventHandler<GenericEventArgs<string>> sceneLoaded;

	public bool sceneLoadedCompleted => _sceneLoadCompleted;
	public const string tutorialSceneName = "Tutorial Level";
	public const string gameSceneName = "Level Design";
	public const string loadingSceneName = "Loading";
	public const string menuSceneName = "Menu";

	protected override void DoAwake()
	{
		DontDestroyOnLoad(gameObject);
	}

	public void QueueScene(string sceneName) => _sceneToLoad = sceneName;

	public void LoadSceneAsync()
	{
		StopCoroutine(nameof(LoadSceneAsync));
		StartCoroutine(DoLoadSceneAsync());
	}

	private IEnumerator DoLoadSceneAsync()
	{
		_sceneLoadCompleted = false;
		_load = SceneManager.LoadSceneAsync(_sceneToLoad, LoadSceneMode.Single);
		_load.allowSceneActivation = false;
		while (!_load.isDone)
		{
			// Scenes will only load 90% async
			if (_load.progress >= 0.9f)
			{
				break;
			}
			yield return null;
		}
		// Uncomment line below to simulate long load times.
		//yield return new WaitForSeconds(10.0f);
		_sceneLoadCompleted = true;
		RaiseSceneLoaded(_sceneToLoad);
	}

	public void ActivateLoadedScene()
	{
		if (_load is null) return;

		_load.allowSceneActivation = true;
	}

	private void RaiseSceneLoaded(string sceneName)
	{
		sceneLoaded?.Invoke(instance, new GenericEventArgs<string>(sceneName));
	}

	AsyncOperation _load;
	string _sceneToLoad;
	bool _sceneLoadCompleted;
}
