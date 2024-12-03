using System.Collections;
using UnityEngine;

public class AdvancedProjectileMotion : MonoBehaviour
{
	public bool useX;
	public AnimationCurve deltaX;
	public bool useY;
	public AnimationCurve deltaY;
	public bool useZ;
	public AnimationCurve deltaZ;
	public bool useWorldPos = true;

	public float timeTilCompletion = 1.0f;

	private void Awake()
	{
		_dmg = GetComponent<DamageIndicator>();
	}

	// Start is called before the first frame update
	void Start()
	{
		originalPos = transform.position;
	}

	public void StartMoving()
	{
		t = 0;
		originalPos = transform.position;
		StartCoroutine(nameof(DoMove));
	}

	IEnumerator DoMove()
	{
		while (t < timeTilCompletion)
		{
			t += Time.deltaTime;
			var timeLinePos = t / timeTilCompletion;
			if (useWorldPos)
			{
				Vector3 targetPos = originalPos;
				if (useX) targetPos.x += deltaX.Evaluate(timeLinePos);
				if (useY) targetPos.y += deltaY.Evaluate(timeLinePos);
				if (useZ) targetPos.z += deltaZ.Evaluate(timeLinePos);
				transform.position = targetPos;
			}
			else
			{
				Vector3 delta = Vector3.zero; ;
				if (useX) delta.x += deltaX.Evaluate(timeLinePos);
				if (useY) delta.y += deltaY.Evaluate(timeLinePos);
				if (useZ) delta.z += deltaZ.Evaluate(timeLinePos);
				
				transform.position =  originalPos + transform.TransformDirection(delta);
			}
			yield return null;
		}
		if (_dmg)
			NotificationCenter.instance.damageTextFinishedMoving?.Invoke(this, new GenericEventArgs<DamageIndicator>(_dmg));

		transform.position = originalPos;
	}

	DamageIndicator _dmg;
	float t;
	Vector3 originalPos;
}
