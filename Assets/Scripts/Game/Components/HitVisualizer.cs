using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class HitVisualizer : RefreshableComponent
{
	[SerializeField] Color _hitColor = Color.red;
	[SerializeField] Renderer[] _renderers;

	private void Awake()
	{
		_hc = GetComponent<HealthComponent>();
		_hc.damageTaken += HealthComponent_OnDamageTaken;
		_originalColorByRenderer = new Dictionary<Renderer, Color>();
		foreach (Renderer renderer in _renderers)
		{
			_originalColorByRenderer[renderer] = renderer.material.color;
		}
	}

	private void OnDestroy()
	{
		_hc.damageTaken -= HealthComponent_OnDamageTaken;
	}

	public void SetRenderers(Renderer[] renderers)
	{
		_renderers = renderers;
		foreach (Renderer renderer in _renderers)
		{
			_originalColorByRenderer[renderer] = renderer.material.color;
		}
	}

	public override void OnInit()
	{
		RevertAllRenderersColor();
	}

	public override void OnKilled() { } // NOOP

	private void HealthComponent_OnDamageTaken(object sender, GenericEventArgs<float> e)
	{
		StopCoroutine(nameof(FadeColor));
		StartCoroutine(nameof(FadeColor));
	}

	private void SetAllRenderersColor(Color color)
	{
		foreach(var renderer in _renderers)
		{
			renderer.material.color = color;
		}
	}

	private void RevertAllRenderersColor()
	{
		foreach(var renderer in _renderers)
		{
			renderer.material.color = _originalColorByRenderer[renderer];
		}
	}

	IEnumerator FadeColor()
	{
		SetAllRenderersColor(_hitColor);
		var t = 0.0f;

		while (t < _fadeTime)
		{
			foreach(var renderer in _renderers)
			{
				var originalColor = _originalColorByRenderer[renderer];
				renderer.material.color = Color.Lerp(renderer.material.color, originalColor, t);
			}
			t += Time.deltaTime;
			yield return null;
		}

		RevertAllRenderersColor();
	}

	HealthComponent _hc;

	Dictionary<Renderer, Color> _originalColorByRenderer;
	float _fadeTime = 0.5f;
}
