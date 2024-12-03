using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]
public sealed class NavPollerComponent : RefreshableComponent
{
	[SerializeField] DistancePollObject[] _distancePollInfos;
	[SerializeField] bool _usePoller = true;

	public bool assignNavDestination = true;
	public Player TargetPlayer => _targetPlayer;
	public float DistanceToPlayer => _distanceToPlayerSq;

	void Awake()
	{
		_agent = GetComponent<NavMeshAgent>();
		_distancePollInfos = _distancePollInfos.OrderBy(p => p.DistanceSqThreshold).ToArray();
		StartPolling();
	}

	void Update()
	{
		if (_usePoller is false) return;

		if (_startPoll)
		{
			_startPoll = false;
			ResetPolling();
		}
		UpdatePollInfo();
	}

	public override void OnInit()
	{
		enabled = true;
		StartPolling();
	}

	public override void OnKilled()
	{
		StopAllCoroutines();
		enabled = false;
		_agent.enabled = false;
		_targetPlayer = null;
	}

	public void StartPolling()
	{
		_startPoll = true;
		_distancePollInterval = 0;
	}

	public void ResetPolling()
	{
		StopCoroutine(nameof(EnemyPolling));
		StartCoroutine(nameof(EnemyPolling));
	}

	/// <summary>
	/// Grabs distance poll info accordeing to nearest player. In addition, sets target player.
	/// </summary>
	void UpdatePollInfo()
	{
		var newPollInterval = GetPollStateInfoFromDistance().PollInterval;
		if (_distancePollInterval != newPollInterval)
		{
			_distancePollInterval = newPollInterval;
			ResetPolling();
		}
	}

	/// <summary>
	/// Periodically update NavMeshAgent destination instead of every frame
	/// </summary>
	IEnumerator EnemyPolling()
	{
		while (true)
		{
			// There is a chance that the coroutine could start before players are intialized. Wait a frame before processing.
			if (_targetPlayer is null || !_agent.enabled)
			{
				yield return new WaitForEndOfFrame();
			}
			else
			{
				if (assignNavDestination)
				{
					_agent.SetDestination(_targetPlayer?.GetAvatarPosition() ?? Vector3.zero);
					_agent.enabled = true;
				}
				yield return new WaitForSeconds(_distancePollInterval);
			}
		}
	}

	DistancePollObject GetPollStateInfoFromDistance()
	{
		// Find nearest player avatar
		var players = FindObjectsOfType<Player>().Where(p => p.PlayerStats.IsAlive);
		var closestPlayer = players.FirstOrDefault();
		float closestDistanceSq = float.MaxValue;
		foreach (var player in players)
		{
			var distSq = (player.GetAvatarPosition() - transform.position).sqrMagnitude;
			if (distSq < closestDistanceSq)
			{
				closestPlayer = player;
				closestDistanceSq = distSq;
			}
		}

		// If there are no targets found, stop nav-ing.
		if (closestDistanceSq is float.MaxValue || closestPlayer is null)
			_agent.destination = transform.position;

		_distanceToPlayerSq = closestDistanceSq;
		_targetPlayer = closestPlayer;


		// Now that we have a target, update get frequency based on target distance sq
		DistancePollObject pollStateInfo = null;

		// Default to closest distance poll state
		foreach (var pollStat in _distancePollInfos)
		{
			if (closestDistanceSq > pollStat.DistanceSqThreshold) // Target is too far to use this poll info
			{
				continue;
			}
			else 
			{
				pollStateInfo = pollStat;
				break;
			}
		}

		// If we are outside the range of all polls, use the furthest one.
		return pollStateInfo ?? _distancePollInfos.Last();
	}

	Player _targetPlayer;
	NavMeshAgent _agent;
	float _distanceToPlayerSq;
	float _distancePollInterval = 0.0f;
	bool _startPoll;
}
