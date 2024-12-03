using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrapCooldownIcon : MonoBehaviour
{
    [SerializeField] Image selectedBorder;
    [SerializeField] Image icon;
    [SerializeField] Image cooldownFill;
    [SerializeField] TextMeshProUGUI cooldownText;
    public TrapInfo trapInfo;
    private float cooldown;

    private void Awake()
    {
        ToggleCooldownUI(false);
    }

    private void FixedUpdate()
    {
        if(cooldown > 0)
        {
            cooldown -= Time.deltaTime;
            SetTrapIconCooldownText(cooldown);
            cooldownFill.fillAmount = 1 - (cooldown / trapInfo.activeDuration);
        }
        else
        {
            ToggleCooldownUI(false);
        }
    }

    private void SetTrapIconCooldownText(float cooldown)
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

    public void ActivateCooldownUI()
    {
        cooldown = trapInfo.activeDuration;
        ToggleCooldownUI(true);
        SetTrapIconCooldownText(cooldown);
        cooldownFill.fillAmount = 1 - (cooldown / trapInfo.activeDuration);
    }

    private void ToggleCooldownUI(bool toggleState)
    {
        selectedBorder.enabled = toggleState;
        icon.enabled = toggleState;
        cooldownFill.enabled = toggleState;
        cooldownText.enabled = toggleState;
    }
}
