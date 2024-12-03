using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
	public bool round = false;
    [SerializeField] TextMeshPro _textField;

	private void Awake()
	{
		_aProjMot = GetComponent<AdvancedProjectileMotion>();
	}

	public void SetTextProperties(float amount, Color color)
	{
		if (round) amount = Mathf.RoundToInt(amount);
		_textField.text = amount.ToString();
		_textField.color = color;
		_aProjMot.StartMoving();
	}

	AdvancedProjectileMotion _aProjMot;
}
