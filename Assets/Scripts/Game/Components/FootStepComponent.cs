using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootStepComponent : MonoBehaviour
{
    [SerializeField] AudioClip[] _footSteps;

	private void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
	}
	public void PlayFootStep()
    {
        var step = _footSteps[Random.Range(0,_footSteps.Length)];
		_audioSource.PlayOneShot(step);
    }

	AudioSource _audioSource;
}
