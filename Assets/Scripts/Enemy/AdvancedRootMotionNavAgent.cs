using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NavPollerComponent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HealthComponent))]
public class AdvancedRootMotionNavAgent : RefreshableComponent
{
	public float sweetSpotRatio;
	public float targetMinDistance;
	public float targetMaxDistance;
	public bool inDistanceRange => _inRange;

	[Header("Debug")]
	[SerializeField] bool _showDebugLocations;

	void Awake()
	{
		_anim = GetComponent<Animator>();
		_agent = GetComponent<NavMeshAgent>();
		_navPoller = GetComponent<NavPollerComponent>();
		_hc = GetComponent<HealthComponent>();

		_anim.applyRootMotion = true;
		_agent.updatePosition = false;
		_agent.updateRotation = false;
	}

	void Update()
	{
		if (!_hc.IsAlive) return;

		var distance = Mathf.Sqrt(_navPoller.DistanceToPlayer);
		
		if (_movingToLocation)
		{
			UpdateAnimParamsFromSteering();
		}
		else if (distance < targetMinDistance || distance > targetMaxDistance)
		{
			MoveToSweetSpot();
			UpdateAnimParamsFromSteering();
		}
		else
		{
		}

		if (_agent.remainingDistance < _agent.radius)
		{
			_anim.SetBool("IsMoving", false);
			_movingToLocation = false;
		}

		_inRange = distance > targetMinDistance && distance < targetMaxDistance;
		FacePlayer();
	}

	void LateUpdate()
	{
		if (Vector3.Distance(transform.position, _agent.transform.position) > _agent.radius)
			transform.position = _agent.transform.position;
	}

	void OnAnimatorMove()
	{
		Vector3 position = _anim.rootPosition;
		position.y = _agent.nextPosition.y;
		transform.position = position;
		_agent.nextPosition = position;
	}

	public override void OnInit()
	{
		enabled = true;
	}

	public override void OnKilled()
	{
		enabled = false;
	}

	private void OnDrawGizmos()
	{
		if (!_showDebugLocations) return;

		DrawColoredSphere(_sweetSpot, 1.0f, Color.green);
		if (_agent)
			DrawColoredSphere(_agent.destination, 0.5f, Color.yellow);

		void DrawColoredSphere(Vector3 position, float radius, Color color)
		{
			Gizmos.color = color;
			Gizmos.DrawWireSphere(position, radius);
		}
	}

	public void MoveToLocation(Vector3 location)
	{
		_agent.destination = location;
		_movingToLocation = true;
	}

	/// <summary>
	/// Agent is outside of range. Set nav target to sweet spot between range boundaries.
	/// </summary>
	private void MoveToSweetSpot()
	{
		if (_navPoller.TargetPlayer is null)
		{
			_anim.SetBool("IsMoving", false);
			return;
		}

		var playerPos = _navPoller.TargetPlayer.GetAvatarPosition();
		var dir = (transform.position - playerPos).normalized;
		var distance = Mathf.Lerp(targetMinDistance, targetMaxDistance, sweetSpotRatio);
		var location = playerPos + (dir * distance);
		var validLocation = FindValidNavDestination(location);
		_sweetSpot = validLocation;
		_agent.destination = _sweetSpot;

		_anim.SetBool("IsMoving", true);
	}

	private void FacePlayer()
	{
		if (_navPoller.TargetPlayer is null) return;

		var lookDir = _navPoller.TargetPlayer.GetAvatarPosition() - transform.position;
		if (lookDir.sqrMagnitude > 0.1f)
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lookDir), 180f * Time.deltaTime);
	}

	private void UpdateAnimParamsFromSteering()
	{
		var dir = (_agent.steeringTarget - transform.position);
		var animDir = transform.InverseTransformDirection(dir);

		if (animDir.sqrMagnitude > 1)
			animDir = animDir.normalized;

		_anim.SetFloat("VelY", animDir.z);
		_anim.SetFloat("VelX", animDir.x);
		_anim.SetBool("IsMoving", true);
	}

	private Vector3 FindValidNavDestination(Vector3 sampleSpot)
	{
		Vector3 result;
		if (NavMesh.SamplePosition(sampleSpot, out var hit, 1.0f, _agent.areaMask))
			result = hit.position;
		else if (NavMesh.SamplePosition(sampleSpot, out var farHit, 5.0f, _agent.areaMask))
			result = farHit.position;
		else
			result = transform.position; // Unable to nav!

		return result;
	}

	Animator _anim;
	NavMeshAgent _agent;
	NavPollerComponent _navPoller;
	HealthComponent _hc;

	Vector3 _sweetSpot;
	bool _inRange;
	bool _movingToLocation;
}