using System;
using UnityEngine;

public class AbilitySlotsComponent : MonoBehaviour
{
	public AbilityInfo abilitySlot1;
	public AbilityInfo abilitySlot2;
	public AbilityInfo abilitySlot3;
	public AbilityInfo abilitySlot4;

	public float ability1Cooldown = 0;
	public float ability2Cooldown = 0;
	public float ability3Cooldown = 0;
	public float ability4Cooldown = 0;

	public int numberOfAbilities = 4;

	public event EventHandler<AbilityChangedEventArgs> AbilityChanged;
	public event EventHandler<AbilityChangedEventArgs> AbilitySlotUpdated;

	private void Awake()
	{
		_player = GetComponentInParent<Player>();
		_castingComponent = GetComponent<CastingComponent>();
		_audioSource = GetComponent<AudioSource>();
	}

	void Start()
	{
		SetSelectedAbility(1);
		RaiseAbilitySlotUpdated(abilitySlot1, 1);
		RaiseAbilitySlotUpdated(abilitySlot2, 2);
		RaiseAbilitySlotUpdated(abilitySlot3, 3);
		RaiseAbilitySlotUpdated(abilitySlot4, 4);
	}

	private void Update()
	{
		if (ability1Cooldown > 0) SetAbilityCooldown(1, ability1Cooldown -= Time.deltaTime);
		if (ability2Cooldown > 0) SetAbilityCooldown(2, ability2Cooldown -= Time.deltaTime);
		if (ability3Cooldown > 0) SetAbilityCooldown(3, ability3Cooldown -= Time.deltaTime);
		if (ability4Cooldown > 0) SetAbilityCooldown(4, ability4Cooldown -= Time.deltaTime);
	}

	public bool OwnsSpell(AbilityInfo ability) => abilitySlot1.IsEqual(ability)
		|| (abilitySlot2?.IsEqual(ability) ?? false)
		|| (abilitySlot3?.IsEqual(ability) ?? false)
		|| (abilitySlot4?.IsEqual(ability) ?? false);

	public bool CanCast() => GetAbilityCooldown(_selectedAbililtyNumber) <= 0;

	public void UpdateAbilitySlot(AbilityInfo newAbility, int slotNumber)
	{
		switch (slotNumber)
		{
			case 1:
				abilitySlot1 = newAbility;
				break;
			case 2:
				abilitySlot2 = newAbility;
				break;
			case 3:
				abilitySlot3 = newAbility;
				break;
			case 4:
				abilitySlot4 = newAbility;
				break;
		}
		RaiseAbilitySlotUpdated(newAbility, slotNumber);
	}

	public AbilityInfo GetAbility(int slotNumber)
	{
		return slotNumber switch
		{
			1 => abilitySlot1,
			2 => abilitySlot2,
			3 => abilitySlot3,
			4 => abilitySlot4,
			_ => throw new System.NotImplementedException(),
		};
	}

	public float GetAbilityCooldown(int slotNumber)
	{
		return slotNumber switch
		{
			1 => ability1Cooldown,
			2 => ability2Cooldown,
			3 => ability3Cooldown,
			4 => ability4Cooldown,
			_ => throw new System.NotImplementedException(),
		};
	}

	public void SetAbilityCooldown(int slotNumber, float newValue)
	{
		switch (slotNumber)
		{
			case 1:
				ability1Cooldown = newValue;
				break;
			case 2:
				ability2Cooldown = newValue;
				break;
			case 3:
				ability3Cooldown = newValue;
				break;
			case 4:
				ability4Cooldown = newValue;
				break;
		}

	}

	public bool GetAreAllAbilitySlotsFull()
	{
		return abilitySlot1 != null && abilitySlot2 != null && abilitySlot3 != null && abilitySlot4 != null;
	}

	public int GetNextUnusedAbilitySlotNumber()
	{
		if (abilitySlot1 == null)
		{
			return 1;
		}
		if (abilitySlot2 == null)
		{
			return 2;
		}
		if (abilitySlot3 == null)
		{
			return 3;
		}
		if (abilitySlot4 == null)
		{
			return 4;
		}
		return -1;
	}

	public void CastSpell()
	{
		var ability = GetAbility(_selectedAbililtyNumber);

		SetAbilityCooldown(_selectedAbililtyNumber, ability.cooldown);
		var pos = _castingComponent.GetCastingPosition();
		var spread = ability.projectileSpread;

		var rot = _castingComponent.GetRandomCastingSpreadRotation(spread);
		var abilityPrefab = Instantiate(ability.abilityPrefab, pos, rot);
		var abilityComponent = abilityPrefab.GetComponent<Ability>();
		if (abilityComponent != null)
		{
			abilityComponent.SetPlayer(_player);
		}

		if (ability.castingSound is object)
		{
			_audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.2f);
			_audioSource.PlayOneShot(ability.castingSound);

		}
	}

	public void SetSelectedAbility(int slotNumber)
	{
		var ability = GetAbility(slotNumber);
		if (ability != null)
		{
			_selectedAbililtyNumber = slotNumber;
			RaiseAbilityChanged(slotNumber);
		}
	}

	public void SetSelectedAbilityByDirection(SelectAbilityDirection direction)
	{
		var newSelectedAbilityNumber = _selectedAbililtyNumber;
		switch (direction)
		{
			case SelectAbilityDirection.Left:
				newSelectedAbilityNumber -= 1;
				if (newSelectedAbilityNumber <= 0)
				{
					if (GetAreAllAbilitySlotsFull())
					{
						newSelectedAbilityNumber = 4;
					}
					else
					{
						newSelectedAbilityNumber = GetNextUnusedAbilitySlotNumber() - 1;
					}
				}
				break;
			case SelectAbilityDirection.Right:
				newSelectedAbilityNumber += 1;
				if (GetAreAllAbilitySlotsFull())
				{
					if (newSelectedAbilityNumber >= 5)
					{
						newSelectedAbilityNumber = 1;
					}
				}
				else
				{
					if (newSelectedAbilityNumber > GetNextUnusedAbilitySlotNumber() - 1)
					{
						newSelectedAbilityNumber = 1;
					}
				}
				break;
		}
		var ability = GetAbility(newSelectedAbilityNumber);
		if (ability != null)
		{
			_selectedAbililtyNumber = newSelectedAbilityNumber;
			RaiseAbilityChanged(newSelectedAbilityNumber);
		}
	}

	AbilityInfo GetCurrentSelectedAbility() => GetAbility(_selectedAbililtyNumber);

	void RaiseAbilityChanged(int slotNumber)
	{
		AbilityChanged?.Invoke(this, new AbilityChangedEventArgs(GetCurrentSelectedAbility(), slotNumber));
	}

	private void RaiseAbilitySlotUpdated(AbilityInfo newAbility, int slotNumber)
	{
		AbilitySlotUpdated?.Invoke(this, new AbilityChangedEventArgs(newAbility, slotNumber));
	}

	private Player _player;
	int _selectedAbililtyNumber;
	CastingComponent _castingComponent;
	AudioSource _audioSource;

	public sealed class AbilityChangedEventArgs : EventArgs
	{
		public AbilityChangedEventArgs(AbilityInfo info, int slotNumber)
		{
			abilityInfo = info;
			SlotNumber = slotNumber;
		}
		public AbilityInfo abilityInfo { get; }
		public int SlotNumber { get; }
	}
}

public enum SelectAbilityDirection
{
	Left = 1,
	Right = 2,
}
