using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavPollerComponent))]
[RequireComponent(typeof(AdvancedRootMotionNavAgent))]
[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(Dissolver))]
[RequireComponent(typeof(RagdollComponent))]
[RequireComponent(typeof(LootDropComponent))]
[RequireComponent (typeof(NavMeshAgent))]
public class Archer : EnemyBase
{
	[Header("Targeting")]
	[SerializeField] Transform _eyesPos;
	[SerializeField] LayerMask _targetMask;
	[SerializeField] int _maxSightTries = 5;
	[SerializeField] float _sightAdjustmentDeltaDeg = 15.0f;

	[Header("NavSettings")]
	[SerializeField] float _targetMinDistance;
	[SerializeField] float _targetMaxDistance;
	[SerializeField] float _sweetSpotRatio = 0.25f;

	[Header("BowSettings")]
	[SerializeField] float _maxLookAheadTime = 1.0f;
	[SerializeField] float _shootCooldown = 2.0f;
	[SerializeField] float _minTimeToDraw = 5.0f;
	[SerializeField] Transform _shootLocation;
	[SerializeField] GameObject _arrowPrefab;

	[Header("FX")]
	[SerializeField] Renderer _targetRenderer;

	protected override void DoAwake()
	{
		_animator = GetComponent<Animator>();
		_rmNavAgent = GetComponent<AdvancedRootMotionNavAgent>();
		_rmNavAgent.targetMinDistance = _targetMinDistance;
		_rmNavAgent.targetMaxDistance = _targetMaxDistance;
		_rmNavAgent.sweetSpotRatio = _sweetSpotRatio;

		_dissolver = GetComponent<Dissolver>();
		_dissolver.SetTargetRenderer(_targetRenderer);
		
		_readyToNock = true;
	}

	// Update is called once per frame
	protected override void DoUpdate()
	{
		if (_navPoller.TargetPlayer is null) return;

		if (_rmNavAgent.inDistanceRange && targetInSight()) // We have line of sight and within range.
		{
			if (!_arrowNocked && _readyToNock) // Arrow cooldown has passed and we don't already have an arrow
				DrawArrow();

			if (_arrowNocked && _canShoot) // Ready to fire
			{
				ShootArrow();
			}
		}
		else if (_rmNavAgent.inDistanceRange) // In range, but no in line of sight.
		{
			GetWithinSight();
		}
		else // Outside range, move toward player
		{
			if (_arrowNocked)
				StoreArrow();
		}
	}

	protected override IEnumerator SelfOnInit()
	{
		_readyToNock = true;
		_canShoot = true;
		yield return null;
	}

	// Called from draw animation
	public void ArrowReady()
	{
		_arrowNocked = true;
		StartCoroutine(nameof(WaitForTimeToDraw));
	}

	// Called from recoil animation
	public void Fire()
	{
		_canShoot = false;
		Instantiate(_arrowPrefab, _shootLocation.position, Quaternion.LookRotation(_aimDir));
		_arrowNocked = false;
		StartCoroutine(nameof(WaitForCooldown));
	}

	private void SetPredictiveTarget()
	{
		var target = _navPoller.TargetPlayer;
		if (target == null) return;

		// Calculate perdictive aiming
		var initialPos = target.GetAvatarPosition();
		var arrowSpeed = 40.0f;
		var targetVel = target.GetAvatarVelocity();

		// Quadratic forumla derived from kinematic equations for projectile interception using dot products.
		var a = Vector3.Dot(targetVel, targetVel) - (arrowSpeed * arrowSpeed);
		var b = 2 * (Vector3.Dot(initialPos, targetVel));
		var c = Vector3.Dot(initialPos, initialPos);

		var sqrt = Mathf.Sqrt(b * b - (4 * a * c));
		var t1 = (-b + sqrt) / (2 * a);
		var t2 = (-b - sqrt) / (2 * a);

		float t;
		if (t1 > 0 && t2 > 0)
			t = Mathf.Min(t1, t2);
		else if (t1 > 0)
			t = t1;
		else if (t2 > 0)
			t = t2;
		else
			t = 0.0f;

		var dir = (initialPos + (targetVel * Mathf.Min(t, _maxLookAheadTime))) - transform.position;
		_aimDir = dir;
	}

	private bool targetInSight()
	{
		var avatarPos = _navPoller.TargetPlayer.GetAvatarPosition();
		// adjust avatar pos so we don't aim at the floor.
		avatarPos.y = 0.5f;
		var dir = avatarPos - _eyesPos.position;
		if (Physics.Raycast(_eyesPos.position, dir, out var hit, _targetMaxDistance, _targetMask))
		{
			return hit.collider.gameObject.tag == "Player";
		}
		return false;
	}

	private void GetWithinSight()
	{
		var avatarPos = _navPoller.TargetPlayer.GetAvatarPosition();
		var rot = Quaternion.LookRotation(Vector3.forward, Vector3.up);
		
		for (int i = 0; i < _maxSightTries; i++)
		{
			var dir1 = rot * Quaternion.AngleAxis(i * _sightAdjustmentDeltaDeg, Vector3.up);
			var dir2 = rot * Quaternion.AngleAxis(i * _sightAdjustmentDeltaDeg, Vector3.up);

			// Check if line of sight is clear
			if (!Physics.Raycast(avatarPos, dir1.eulerAngles, out _, _targetMinDistance))
			{
				_rmNavAgent.MoveToLocation(avatarPos + (dir1.eulerAngles * _targetMinDistance));
				return;
			}
			else if (!Physics.Raycast(avatarPos, dir2.eulerAngles, out _, _targetMinDistance))
			{
				_rmNavAgent.MoveToLocation(avatarPos + (dir2.eulerAngles * _targetMinDistance));
				return;
			}
		}

	}

	private void ShootArrow()
	{
		SetPredictiveTarget();
		_animator.SetTrigger("ShootArrow");
	}

	private void DrawArrow()
	{
		_readyToNock = false;
		_animator.SetTrigger("DrawArrow");
	}

	private void StoreArrow()
	{
		_arrowNocked = false;
		_readyToNock = true;
		_animator.SetTrigger("StoreArrow");
	}

	IEnumerator WaitForTimeToDraw()
	{
		yield return new WaitForSeconds(_minTimeToDraw);
		_canShoot = true;
	}

	IEnumerator WaitForCooldown()
	{
		yield return new WaitForSeconds(_shootCooldown);
		_readyToNock = true;
	}

	Animator _animator;
	AdvancedRootMotionNavAgent _rmNavAgent;
	Dissolver _dissolver;


	Vector3 _aimDir;
	bool _canShoot;
	bool _readyToNock;
	bool _arrowNocked;
}
