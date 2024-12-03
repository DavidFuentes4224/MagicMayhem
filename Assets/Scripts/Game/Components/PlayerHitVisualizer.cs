using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The player uses a different shader that does not have a `color` property.
/// Instead, we need to manually specify the colors of the shader we wish to change.
/// </summary>
[RequireComponent(typeof(HealthComponent))]
public class PlayerHitVisualizer : MonoBehaviour
{
	[SerializeField] Color _hitColor = Color.red;

	private void Awake()
	{
		_hc = GetComponent<HealthComponent>();
		_hc.damageTaken += HealthComponent_OnDamageTaken;
		SetShaderPropValues();
	}

	private void OnDestroy()
	{
		_hc.damageTaken -= HealthComponent_OnDamageTaken;
	}

	public void SetRenderers(Renderer[] renderers)
	{
		_renderers = renderers;

		// SetRenderers can be called prior to awake by the awake call on `Player.cs`.
		if (_shaderPropValues is null)
			SetShaderPropValues();

		// All renderers will share the same material, this means we only have to store the original colors of one renderer's material.
		// Then we can reference that original collection of colors when lerping back to the original.
		var sampleRenderer = _renderers.First();
		_originalColorByShaderProperty = new Dictionary<int, Color>();
		foreach (var propValue in _shaderPropValues)
		{
			_originalColorByShaderProperty[propValue] = sampleRenderer.material.GetColor(propValue);
		}
	}

	private void SetShaderPropValues()
	{
		_shaderPropValues = new List<int>();
		foreach (var propName in _shaderPropNames)
		{
			_shaderPropValues.Add(Shader.PropertyToID(propName));
		}
	}

	private void HealthComponent_OnDamageTaken(object sender, GenericEventArgs<float> e)
	{
		StopCoroutine(nameof(FadeColor));
		StartCoroutine(nameof(FadeColor));
	}

	private void SetAllColorsOnRenderer(Color color)
	{
		foreach(var renderer in _renderers)
		{
			foreach (var propValue in _shaderPropValues)
			{
				renderer.material.SetColor(propValue, color);
			}
		}
	}

	private void RevertAllRendererColors()
	{
		foreach (var renderer in _renderers)
		{
			foreach (var propvalue in _shaderPropValues)
			{
				renderer.material.SetColor(propvalue, _originalColorByShaderProperty[propvalue]);
			}
		}
	}

	IEnumerator FadeColor()
	{
		SetAllColorsOnRenderer(_hitColor);
		var t = 0.0f;

		while (t < _fadeTime)
		{
			foreach (var renderer in _renderers)
			{
				foreach (var propValue in _shaderPropValues)
				{
					var originalColor = _originalColorByShaderProperty[propValue];
					var currentColor = renderer.material.GetColor(propValue);
					var targetColor = Color.Lerp(currentColor, originalColor, t);
					renderer.material.SetColor(propValue, targetColor);
				}
			}
			t += Time.deltaTime;
			yield return null;
		}

		RevertAllRendererColors();
	}

	HealthComponent _hc;
	Renderer[] _renderers;

	string[] _shaderPropNames =
	{
		"_Color_Primary",
		"_Color_Secondary",
		"_Color_Hair",
		"_Color_Skin",
	};
	List<int> _shaderPropValues;
	Dictionary<int, Color> _originalColorByShaderProperty;
	float _fadeTime = 0.5f;
}
