using UnityEngine;

public class ForceAnimStateSettings
{
	public ForceAnimStateSettings(Animator animator, string paramName, int value)
	 : this(animator, paramName)
	{
		iValue = value;
	}

	public ForceAnimStateSettings(Animator animator, string paramName, bool value)
	 : this(animator, paramName)
	{
		bValue = value;
	}

	public ForceAnimStateSettings(Animator animator, string paramName, float value)
	 : this(animator, paramName)
	{
		fValue = value;
	}

	private ForceAnimStateSettings(Animator anim, string param)
	{
		animator = anim;
		paramName = param;
	}

	public bool? bValue { get; }
	public float? fValue { get; }
	public int? iValue { get; }

	public Animator animator { get; }
	public string paramName { get; }
}
