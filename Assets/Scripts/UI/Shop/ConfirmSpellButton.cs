using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmSpellButton : MonoBehaviour
{
    [SerializeField] Image _icon;
    [SerializeField] TextMeshProUGUI _abilityName;
	[SerializeField] Button _confirmButton;

	public Button confirmButton => _confirmButton;

    public void SetSelectAction(Action<int> selectAction)
    {
        if (selectAction != null)
        {
            _selectAction = selectAction;
        }
    }

	public void UpdateAbilityInfo(AbilityInfo abilityInfo, int index)
    {
        _icon.sprite = abilityInfo.icon;
        _abilityName.text = abilityInfo.abilityName;
        _index = index;
	}

    public void PurchaseAbility()
    {
        _selectAction?.Invoke(_index);
    }

    Action<int> _selectAction;
    int _index = 0;
}
