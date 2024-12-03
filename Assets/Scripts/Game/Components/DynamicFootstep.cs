using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DynamicFootstep : MonoBehaviour
{
	[SerializeField] AudioClip[] _footSteps;
	[SerializeField] Transform _leftBone;
	[SerializeField] Transform _rightBone;
	[SerializeField] float _groundThreshold = 0.2f;

	[Header("Optional")]
	[SerializeField] float _minDistanceSq = 100.0f;

	private void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
		_navPoller = GetComponent<NavPollerComponent>();
	}

	private void Update()
	{
		if (_navPoller)
		{
			var distance = _navPoller.DistanceToPlayer;
			if (distance > _minDistanceSq) return;

			CheckFootsteps(distance);
		}
		else
		{
			CheckFootsteps();
		}
	}

	private void CheckFootsteps(float? distance = null)
	{
		if (Physics.Raycast(_leftBone.position, Vector3.down, out _, _groundThreshold))
		{
			if (_leftQueued)
				PlayFootStep(distance);
			_leftQueued = false;
		}
		else
		{
			_leftQueued = true;
		}

		if (Physics.Raycast(_rightBone.position, Vector3.down, out _, _groundThreshold))
		{
			if (_rightQueued)
				PlayFootStep(distance);
			_rightQueued = false;
		}
		else
		{
			_rightQueued = true;
		}
	}

	public void PlayFootStep(float? distance = null)
	{
		var step = _footSteps[Random.Range(0, _footSteps.Length)];
		var scale = distance.HasValue ? 1 - (distance.Value / _minDistanceSq) : 1.0f;
		_audioSource.PlayOneShot(step, scale);
	}

	AudioSource _audioSource;
	NavPollerComponent _navPoller;

	bool _leftQueued;
	bool _rightQueued;
}
