using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellDescription : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI _spellNameText;
	[SerializeField] TextMeshProUGUI _spellDescriptionText;
	[SerializeField] Image _spellIcon;

	public void SetSelectedSpell(AbilityInfo spell)
	{
		_abilityInfo = spell;
		_spellNameText.text = _abilityInfo.abilityName.ToUpper();
		_spellDescriptionText.text = _abilityInfo.description;
		_spellIcon.sprite = _abilityInfo.icon;
	}

	AbilityInfo _abilityInfo;
}
