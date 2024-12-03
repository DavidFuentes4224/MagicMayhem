using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIconController : MonoBehaviour
{
    AbilitySlotsComponent _abilitySlotsComponent;
    
    [SerializeField] Image selectedBorder;
    [SerializeField] Image icon;
    [SerializeField] Image cooldownFill;
    [SerializeField] TextMeshProUGUI cooldownText;
    [SerializeField] int abiltySlotIconNumber;

    private void FixedUpdate()
    {
        if (_abilitySlotsComponent.GetAbility(abiltySlotIconNumber) != null)
        {
            SetAbilityIconCooldownText(_abilitySlotsComponent.GetAbilityCooldown(abiltySlotIconNumber));
            cooldownFill.fillAmount = _abilitySlotsComponent.GetAbilityCooldown(abiltySlotIconNumber) / _abilitySlotsComponent.GetAbility(abiltySlotIconNumber).cooldown;
        }
    }

    private void OnDestroy()
    {
        if (_abilitySlotsComponent != null )
        {
		    _abilitySlotsComponent.AbilityChanged -= AbilitySlotsComponent_OnAbilitySlotChanged;
		    _abilitySlotsComponent.AbilitySlotUpdated -= AbilitySlotsComponent_OnAbilitySlotUpdated;
        }
    }

    public void ConfigureIconController(AbilitySlotsComponent abilitySlotsComponent)
    {
        _abilitySlotsComponent = abilitySlotsComponent;
		_abilitySlotsComponent.AbilityChanged += AbilitySlotsComponent_OnAbilitySlotChanged;
		_abilitySlotsComponent.AbilitySlotUpdated += AbilitySlotsComponent_OnAbilitySlotUpdated;
	}

    private void SetAbilityIconCooldownText(float cooldown)
    {
        var formatedCooldown = Mathf.Ceil(cooldown * 10) / 10;
        if (formatedCooldown > 0)
        {
            cooldownText.text = $"{formatedCooldown.ToString("0.0")} s";
        }
        else
        {
            cooldownText.text = string.Empty;
        }
    }

    private void AbilitySlotsComponent_OnAbilitySlotChanged(object sender, AbilitySlotsComponent.AbilityChangedEventArgs e)
    {
        if (abiltySlotIconNumber == e.SlotNumber)
        {
            selectedBorder.gameObject.SetActive(true);
        }
        else
        {
            selectedBorder.gameObject.SetActive(false);
        }
    }

    private void AbilitySlotsComponent_OnAbilitySlotUpdated(object sender, AbilitySlotsComponent.AbilityChangedEventArgs e)
    {
        if (abiltySlotIconNumber != e.SlotNumber)
        {
            return;
        }

        if (e.abilityInfo && e.abilityInfo.icon)
        {
            icon.sprite = e.abilityInfo.icon;
            icon.enabled = true;
        }
        else if (e.abilityInfo != null)
        {
            icon.sprite = null;
            icon.enabled = true;
        }
        else
        {
            icon.enabled = false;
        }
    }
}
