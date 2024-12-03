using System.Collections;
using UnityEngine;

public static class AnimatorUtility
{
    public static IEnumerator DoForceIntStateForFrame(ForceAnimStateSettings settings)
    {
        var old = settings.animator.GetInteger(settings.paramName);
		settings.animator.SetInteger(settings.paramName, settings.iValue.Value);
        yield return new WaitForFixedUpdate();
		settings.animator.SetInteger(settings.paramName, old);
    }

	public static IEnumerator DoForceBoolStateForFrame(ForceAnimStateSettings settings)
	{
		var old = settings.animator.GetBool(settings.paramName);
		settings.animator.SetBool(settings.paramName, settings.bValue.Value);
		yield return new WaitForFixedUpdate();
		settings.animator.SetBool(settings.paramName, old);
	}
}
