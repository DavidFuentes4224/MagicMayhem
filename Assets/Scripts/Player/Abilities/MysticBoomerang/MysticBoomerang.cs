using UnityEngine;

[RequireComponent(typeof(AdvancedProjectileMotion))]
public class MysticBoomerang : Ability
{
    [SerializeField] Transform _meshTransform;
    [SerializeField] float _rotationSpeed = -900.0f;

	protected override void DoAwake()
	{
        _projectileMotion = GetComponent<AdvancedProjectileMotion>();
	}
	// Start is called before the first frame update
	protected override void DoStart()
    {
        _projectileMotion.StartMoving();
    }

    // Update is called once per frame
    protected override void DoUpdate()
    {
        _meshTransform.rotation = _meshTransform.rotation * Quaternion.Euler(0, _rotationSpeed * Time.deltaTime, 0);
    }

	AdvancedProjectileMotion _projectileMotion;
}
