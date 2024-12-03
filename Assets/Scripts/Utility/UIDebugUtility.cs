using TMPro;
using UnityEngine;

public sealed class UIDebugUtility : Singleton<UIDebugUtility>
{
	public TextMeshProUGUI trackedVelocity;
	public TextMeshProUGUI trackedHealth;
	public TextMeshProUGUI fps;

	protected override void DoAwake()
	{
		DontDestroyOnLoad(gameObject);
	}

	public void UpdateFps(float value)
	{
		if (fps is null) return;
			fps.text = $"FPS: {value}";
	}

	public void UpdateTrackedHealth(float value)
	{
		if (trackedHealth is null) return;

		trackedHealth.text = $"Health: {value}";
	}
	public void UpdateTrackedVelocity(Vector2 vel)
	{
		if (trackedVelocity is null) return;
		trackedVelocity.text = $"Velocity x:{vel.x}, z:{vel.y}";
	}

}
