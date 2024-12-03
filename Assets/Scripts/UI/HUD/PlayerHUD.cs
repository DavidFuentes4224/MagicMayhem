using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] PlayerStats playerStats;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Slider healthSlider;
    [SerializeField] TextMeshProUGUI goldText;
	[SerializeField] AbilityIconController[] _abilityIconControllers;
    [SerializeField] Image _frame;

	private void Update()
    {
        UpdateHealthUI();
        goldText.text = playerStats.gold.ToString();
    }

    public void TrackPlayer(Player player)
    {
        playerStats = player.PlayerStats;
		foreach (var iconController in _abilityIconControllers)
        {
            iconController.ConfigureIconController(player.abilitySlotsComponent);
        }
        _frame.color = player.playerColor;
	}

    private void UpdateHealthUI()
    {
        healthSlider.value = playerStats.health / playerStats.maxHealth;
        healthText.text = $"{playerStats.health.ToString("0.0")} / {playerStats.maxHealth.ToString("#.0")}";
    }
}
