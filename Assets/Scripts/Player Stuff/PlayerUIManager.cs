using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealthManager player;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image shieldIcon;
    [SerializeField] private TextMeshProUGUI cooldownText;

    void Start()
    { 
        player = FindObjectOfType<PlayerHealthManager>();

        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = player != null ? player.MaxHealth : 100f;
            healthSlider.value   = player != null ? player.CurrentHealth : healthSlider.maxValue;
        }

        shieldIcon.gameObject.SetActive(false);
        cooldownText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = player.MaxHealth;
            healthSlider.value    = player.CurrentHealth;
        }

        if (shieldIcon != null)
        {
            shieldIcon.gameObject.SetActive(player.AbilityActive);

        }

        if (cooldownText != null)
        {
            bool showCd = player.AbilityOnCooldown && !player.AbilityActive;
            if (showCd)
            {
                int seconds = Mathf.CeilToInt(player.CooldownRemaining);
                cooldownText.text = $"{seconds} seconds left";
            }
            cooldownText.gameObject.SetActive(showCd);
        }
    }
}
