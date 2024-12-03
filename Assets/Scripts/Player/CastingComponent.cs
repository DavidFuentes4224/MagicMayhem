using UnityEngine;

[RequireComponent(typeof(Animator), typeof(AbilitySlotsComponent), typeof(Mover))]
public class CastingComponent : MonoBehaviour
{
	[SerializeField] Transform _castingLocation;
	void Awake()
	{
		_animator = GetComponent<Animator>();
		_abilitySlotsComponent = GetComponent<AbilitySlotsComponent>();
		_abilitySlotsComponent.AbilityChanged += AbilitySlotsComponent_OnAbilityChanged;
		_mover = GetComponent<Mover>();
	}

	// Update is called once per frame
	void Update()
	{
		if (!_mover.isRolling && _isCastingSpell && _abilitySlotsComponent.CanCast())
		{
			switch (_currentAbility.abiltyType)
			{

				case AbiltyType.Continuous:
					break;
				case AbiltyType.RapidFire:
					CastRapidFire();
					break;
				default:
				case AbiltyType.SingleUse:
					CastSingleUse();
					break;
			}

			_abilitySlotsComponent.CastSpell();
		}
	}

	void OnDestroy()
	{
		_abilitySlotsComponent.AbilityChanged -= AbilitySlotsComponent_OnAbilityChanged;
	}

	public Vector3 GetCastingPosition() => _castingLocation.position;
	public Quaternion GetRandomCastingSpreadRotation(float angle)
	{
		if (angle <= 0)
			return _castingLocation.rotation;

		var randomY = Random.Range(-angle, angle);

		return Quaternion.LookRotation(_castingLocation.forward) * Quaternion.Euler(0, randomY, 0);
	}

	public void SetSelectedAbility(int slotNumber)
	{
		_abilitySlotsComponent.SetSelectedAbility(slotNumber);
	}

	public void SetSelectedAbilityByDirection(SelectAbilityDirection direction)
	{
		_abilitySlotsComponent.SetSelectedAbilityByDirection(direction);
	}

	public void UpdateCasting(bool isCastingSpell) => _isCastingSpell = isCastingSpell;

	void CastRapidFire()
	{
		_castRight = !_castRight;
		_animator.SetInteger("CastVariant", Random.Range(1, 4));
		_animator.SetTrigger($"{(_castRight ? "R" : "L")}Cast");
	}

	void CastSingleUse()
	{
		_animator.SetTrigger("SingleCast");
	}

	private void AbilitySlotsComponent_OnAbilityChanged(object sender, AbilitySlotsComponent.AbilityChangedEventArgs e)
	{
		if (e.abilityInfo is object)
			_currentAbility = e.abilityInfo;
	}

	bool _isCastingSpell;
	bool _castRight;
	Animator _animator;
	AbilitySlotsComponent _abilitySlotsComponent;
	AbilityInfo _currentAbility;
	Mover _mover;
}
