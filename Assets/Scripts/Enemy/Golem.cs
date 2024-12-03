using UnityEngine;

[RequireComponent(typeof(Dissolver))]
[RequireComponent(typeof(MeleeAttackComponent))]
public class Golem : EnemyBase
{
	[Header("FX")]
	[SerializeField] ParticleSystem _dustParticleSystem;
	[SerializeField] AudioClip _stompAudio;
	[SerializeField] Renderer _targetRenderer;

	[Header("Attack")]
	[SerializeField] RadialDamageSource _stompSource;

	protected override void DoAwake()
	{
		_dissolver = GetComponent<Dissolver>();
		if (_targetRenderer)
			_dissolver.SetTargetRenderer(_targetRenderer);
		
		_meleeAttackComponent = GetComponent<MeleeAttackComponent>();
	}

	private void FixedUpdate()
	{
		if (_meleeAttackComponent.canAttack)
			_meleeAttackComponent.MeleeAttack();
	}

	public void Stomp()
    {
		_dustParticleSystem.Play();
		_audioSource.PlayOneShot(_stompAudio);

		// Particle system is attacked to golem's foot
		var hitPoint = _dustParticleSystem.transform.position;
		Instantiate(_stompSource, hitPoint, Quaternion.identity);
	}

	Dissolver _dissolver;
	MeleeAttackComponent _meleeAttackComponent;
}
