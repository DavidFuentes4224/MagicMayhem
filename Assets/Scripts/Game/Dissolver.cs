using UnityEngine;

// A majority of this code is taken from `SpawnEffect.cs` but tweaked to match our use case.
public class Dissolver : RefreshableComponent
{
	public float spawnEffectTime = 5.0f;
	public float pause = 0;
	public AnimationCurve fadeIn;

	[SerializeField] Material _dissolveMaterial;

	void Start()
	{
		shaderProperty = Shader.PropertyToID("_cutoff");
		ps = GetComponentInChildren<ParticleSystem>();

		var main = ps.main;
		main.duration = spawnEffectTime;
	}

	void Update()
	{
		if (!_isDissolving) return;

		timer += Time.deltaTime;

		_targetRenderer.material.SetFloat(shaderProperty, fadeIn.Evaluate(Mathf.InverseLerp(0, spawnEffectTime, timer)));
		if (timer >= spawnEffectTime)
			ResetEffect(false);
	}

	public override void OnInit()
	{
		ResetEffect(false);
	}

	public override void OnKilled()
	{
		StartDissolving();
	}

	public void SetTargetRenderer(Renderer targetRenderer)
	{
		_targetRenderer = targetRenderer;
		_originalMaterial = targetRenderer.material;
	}

	public void StartDissolving()
	{
		_isDissolving = true;
		_targetRenderer.material = _dissolveMaterial;
		ps.Play();
	}

	public void ResetEffect(bool deactivateGameObject = true)
	{
		_isDissolving = false;
		if (deactivateGameObject)
			gameObject.SetActive(false);
		if (_targetRenderer)
		{
			_targetRenderer.material.SetFloat(shaderProperty, 1.0f);
			if (_originalMaterial)
				_targetRenderer.material = _originalMaterial;
			timer = 0;
		}
		
		if (ps)
			ps.Stop();
	}

	ParticleSystem ps;

	bool _isDissolving = false;
	float timer = 0;
	int shaderProperty;
	Material _originalMaterial;
	Renderer _targetRenderer;
}
