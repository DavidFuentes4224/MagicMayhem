using System.Collections;
using UnityEngine;

[RequireComponent(typeof(NavPollerComponent))]
[RequireComponent(typeof(Animator))]
public class MeleeAttackComponent : RefreshableComponent
{
	[SerializeField] float _meleeRange;
	[SerializeField] float _meleeCooldownTime = 1.0f;
	[SerializeField] int _meleeAttackCount = 1;
	[SerializeField] Collider _weaponCollider;

	public bool canAttack => _canAttack;

	private void Awake()
	{
		_navPoller = GetComponent<NavPollerComponent>();
		_anim = GetComponent<Animator>();
		OnInit();
	}

	void FixedUpdate()
	{
		if (_navPoller.TargetPlayer is null) return;
		if (_inCooldown)
		{
			_canAttack = false;
			return;
		}

		var distanceToPlayer = _navPoller.DistanceToPlayer;
		_canAttack = distanceToPlayer <= _meleeRange;
	}
	public override void OnInit()
	{
		_inCooldown = false;
	}

	public override void OnKilled()
	{
		_anim.ResetTrigger("Attack");
	}

	public void MeleeAttack()
	{
		_anim.SetInteger("SwingNum", Random.Range(1, _meleeAttackCount + 1));
		_anim.SetTrigger("Attack");
		_inCooldown = true;
		StartCoroutine(nameof(MeleeCooldown));
	}

	public void EnableHitTrigger()
	{
		if (_weaponCollider is null) return;

		_weaponCollider.enabled = true;
	}

	public void DisableHitTrigger()
	{
		if (_weaponCollider is null) return;

		_weaponCollider.enabled = false;
	}

	IEnumerator MeleeCooldown()
	{
		yield return new WaitForSeconds(_meleeCooldownTime);
		_inCooldown = false;
	}

	NavPollerComponent _navPoller;
	Animator _anim;

	bool _canAttack;
	bool _inCooldown;
}
