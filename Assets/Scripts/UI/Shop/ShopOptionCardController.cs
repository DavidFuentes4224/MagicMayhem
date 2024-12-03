using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopOptionCardController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI abilityName;
    [SerializeField] TextMeshProUGUI abilityCost;
    [SerializeField] TextMeshProUGUI abilityDamage;
    [SerializeField] TextMeshProUGUI abilityCooldown;
    [SerializeField] Button purchaseButton;

    public void UpdateShopOptionCardWithAbility(AbilityInfo abilityInfo)
    {
        // TODO add an icon to display
        abilityName.text = abilityInfo.abilityName;
        abilityCost.text = $"Gold: {abilityInfo.cost}";
        abilityDamage.text = $"Damage: {abilityInfo.damage}";
        abilityCooldown.text = $"Cooldown: {abilityInfo.cooldown}";
    }
}
