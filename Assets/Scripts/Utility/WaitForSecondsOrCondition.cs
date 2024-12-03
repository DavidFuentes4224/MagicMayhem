using System;
using UnityEngine;

public sealed class WaitForSecondsOrCondition : CustomYieldInstruction
{
	public WaitForSecondsOrCondition(Func<bool> condition, float seconds)
	{
		_t = seconds;
		_condition = condition;
	}

	public override bool keepWaiting
	{
		get
		{
			var stopWaiting = _condition.Invoke();
			if (stopWaiting)
				return false;

			_t -= Time.deltaTime;
			if (_t < 0)
				return false;

			return true;
		}
	}
	float _t;
	Func<bool> _condition;
}
