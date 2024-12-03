using UnityEngine;

[CreateAssetMenu(fileName ="RadialDamageSizeInfo", menuName ="Raidal Damage Info")]
public class RadialDamageInfo : ScriptableObject
{
	[Header("Size Manipulation")]
	[SerializeField] Vector3 _initialSize = Vector3.zero;
	[SerializeField] Vector3 _finalSize = Vector3.one;
	[SerializeField] float _expansionSpeed = 1.0f;
	[SerializeField] float _timeBeforeDestory = 0.0f;

	[Header("Damage")]
	[SerializeField] float _damage = 0.0f;
	[SerializeField] float _forceStrength = 100.0f;

	[Header("Tracking")]
	[SerializeField] LayerMask _objectsToTrack;

	public Vector3 initialSize => _initialSize;
	public Vector3 finalSize => _finalSize;
	public float expansionSpeed => _expansionSpeed;
	public float timeBeforeDestory => _timeBeforeDestory;

	public float damage => _damage;
	public float forceStrength => _forceStrength;
	
	public LayerMask objectsToTrack => _objectsToTrack;
}
