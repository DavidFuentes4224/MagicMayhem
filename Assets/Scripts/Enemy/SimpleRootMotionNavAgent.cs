using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent (typeof(NavMeshAgent))]
[RequireComponent(typeof(NavPollerComponent))]
public class SimpleRootMotionNavAgent : RefreshableComponent
{
	[SerializeField] float _lookSpeed = 180.0f;
	[SerializeField] float _movementScale = 1.0f;
	[SerializeField] bool _canSprint = false;
	[SerializeField] [Range(0,1)] float _sprintThreshold = 0.35f;
	[SerializeField] LayerMask _visionLayerMask;

	[Header("Pathing")]
	[SerializeField] float _stoppingDistance = 1.0f;
	[SerializeField] float _cornerThreshold = 0.33f;
	[SerializeField] float _avoidanceWeight = 1.0f;
	[SerializeField] float _avoidanceRadius = 1.0f;


	private void Awake()
	{
		_anim = GetComponent<Animator>();
		_agent = GetComponent<NavMeshAgent>();
		_navPoller = GetComponent<NavPollerComponent>();

		_anim.applyRootMotion = true;
		_agent.updatePosition = false;
		_agent.updateRotation = false;
	}

	void Update()
	{
		CheckIfCloseEnoughToCornerPath();
		TryFacePlayer();
		UpdateAnimParamsFromSteering();

		if (_agent.remainingDistance < _stoppingDistance)
		{
			_anim.SetBool("IsMoving", false);
			_anim.SetBool("IsRunning", false);
		}
	}

	void OnAnimatorMove()
	{
		var rootMotionMovement = _anim.deltaPosition * _movementScale;
		_agent.nextPosition = transform.position + rootMotionMovement;
		
		transform.position = _agent.nextPosition;
	}

	private void OnDrawGizmos()
	{
		if (_navPoller?.TargetPlayer is null) return;

		var dir = (_agent.steeringTarget - transform.position);
		Debug.DrawLine(transform.position, transform.position + dir.normalized, Color.blue);
		Debug.DrawLine(transform.position, transform.position + transform.forward, Color.green);

		if (_agent == null || _agent.path == null || _agent.path.corners.Length < 2)
			return;

		Gizmos.color = Color.red;
		Vector3[] pathCorners = _agent.path.corners;
		for (int i = 0; i < pathCorners.Length - 1; i++)
		{
			Gizmos.DrawLine(pathCorners[i], pathCorners[i + 1]);
		}
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.position, _avoidanceRadius);
	}

	/// <summary>
	/// Since we are using root motion, our nav agent has a tough time getting to nav corners.
	/// Instead, we should check if the distance to the next corner is within a threshold and if so,
	/// just move to the next corner.
	/// </summary>
	private void CheckIfCloseEnoughToCornerPath()
	{
		if (_agent.pathPending || _agent.path.corners.Length < 2) return;

		// Check distance to the next corner
		float distanceToCorner = Vector3.Distance(transform.position, _agent.path.corners[1]);

		if (distanceToCorner <= _cornerThreshold && _agent.path.corners.Length >= 3)
		{
			// If we're close enough to the current corner, tell the agent to go straight to the next corner
			transform.position = Vector3.MoveTowards(transform.position, _agent.path.corners[1], 2 * Time.deltaTime);
		}
	}

	private void UpdateAnimParamsFromSteering()
	{
		var dir = (_agent.steeringTarget - transform.position);
		/// `GetAvoidanceDir` uses a sphere cast for every enemy. This causes our frame rate to reach horrible numbers. Comment out until a better approach is figured out.
		//var avoidDir = GetAvoidanceDir();
		//dir += avoidDir;
		var animDir = transform.InverseTransformDirection(dir);
		if (animDir.sqrMagnitude > 1)
			animDir = animDir.normalized;

		_anim.SetFloat("VelY", animDir.z);
		_anim.SetFloat("VelX", animDir.x);
		var isRunning = _canSprint && (animDir.z > 0.5f && Mathf.Abs(animDir.x) < _sprintThreshold);
		_anim.SetBool("IsRunning", isRunning);
		_anim.SetBool("IsMoving", true);
	}

	private Vector3 GetAvoidanceDir()
	{
		var hits = Physics.SphereCastAll(transform.position, _avoidanceRadius, transform.forward);
		var nearbyAgents = hits.Select(h => h.collider.GetComponent<EnemyBase>()).Where(x => x != null && x.gameObject != gameObject).ToList();
		if (nearbyAgents.Count == 0)
			return Vector3.zero;

		var dir = Vector3.zero;
		foreach(var agent in nearbyAgents)
		{
			var difference = transform.position - agent.transform.position;
			// Use the inverse of the difference magnitude so that closer agents get more weight.
			dir += difference.normalized * (1.0f / difference.sqrMagnitude);
		}
		var result = dir * _avoidanceWeight;
		Debug.DrawLine(transform.position, result);
		return result;
	}

	private void TryFacePlayer()
	{
		if (_navPoller.TargetPlayer is null) return;

		var lookDir = _navPoller.TargetPlayer.GetAvatarPosition() - transform.position;
		Debug.DrawRay(transform.position + Vector3.up, lookDir, Color.yellow);
		if (Physics.Raycast(transform.position + Vector3.up, lookDir, out var hit, _visionDistance, _visionLayerMask) 
			&& hit.transform.tag == "Player")
		{
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lookDir), _lookSpeed * Time.deltaTime);
		}
		else
		{
			var dir = (_agent.steeringTarget - transform.position);
			if (dir.sqrMagnitude > 0.1f)
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), _lookSpeed * Time.deltaTime);
		}
	}

	public override void OnInit()
	{
		enabled = true;
	}

	public override void OnKilled()
	{
		enabled = false;
	}

	const float _visionDistance = 20.0f;

	Animator _anim;
	NavMeshAgent _agent;
	NavPollerComponent _navPoller;
}
