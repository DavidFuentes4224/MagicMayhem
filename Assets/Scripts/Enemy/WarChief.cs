using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Dissolver))]
[RequireComponent(typeof(MeleeAttackComponent))]
public class WarChief : EnemyBase
{
	[Header("Attack")]
	[SerializeField] float _jumpRangeStartSq = 36.0f;
	[SerializeField] RadialDamageSource _jumpSource;
	[SerializeField] Transform _hammerHitPoint;
	[SerializeField] float _jumpCooldown = 15.0f;

	[Header("FX")] 
	[SerializeField] Renderer _renderer;


	protected override void DoAwake()
	{
		_animator = GetComponent<Animator>();

		_dissolver = GetComponent<Dissolver>();
		_dissolver.SetTargetRenderer(_renderer);

		_meleeAttackComponent = GetComponent<MeleeAttackComponent>();
	}

	private void FixedUpdate()
	{
		if (_navPoller.DistanceToPlayer >= _jumpRangeStartSq)
			_canJump = true;
		else if (_canJump && !_isJumping && _navPoller.DistanceToPlayer < _jumpRangeStartSq)
			JumpAttack();
		else if (!_isJumping && _meleeAttackComponent.canAttack)
			_meleeAttackComponent.MeleeAttack();
	}

	protected override IEnumerator SelfOnInit()
	{
		_canJump = true;
		yield return null;
	}

	protected override IEnumerator SelfOnKilled()
	{
		_canJump = true;
		_isJumping = false;
		_animator.ResetTrigger("Jump");
		yield return null;
	}

	public void JumpImpact()
	{
		Instantiate(_jumpSource, _hammerHitPoint.position, Quaternion.identity);
	}

	public void JumpEnd()
	{
		_isJumping = false;
	}

	void JumpAttack()
	{
		_animator.SetTrigger("Jump");
		_animator.SetTrigger("Attack");
		_canJump = false;
		_isJumping = true;
		StartCoroutine(nameof(JumpCooldown));
	}

	public void DoDamage()
	{
		_animator.SetBool("IsAttacking", false);
	}

	public void EndSwing()
	{
		_animator.SetLayerWeight(_swingLayerIndex, 0.0f);
	}

	IEnumerator JumpCooldown()
	{
		yield return new WaitForSeconds(_jumpCooldown);
		_canJump = true;
	}

	Animator _animator;
	Dissolver _dissolver;
	MeleeAttackComponent _meleeAttackComponent;

	bool _canJump;
	bool _isJumping;
	bool _canSwing = true;
	int _swingLayerIndex;
}
