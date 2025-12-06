using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealthManager player;
    [SerializeField] private PlayerAbilityController abilityController;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image shieldIcon;
    [SerializeField] private Image berserkIcon;
    [SerializeField] private TextMeshProUGUI cooldownText;

    private bool _initialized;

    private void Start()
    {
        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = 100f;
            healthSlider.value    = 100f;
        }

        if (shieldIcon != null)
        {
            shieldIcon.gameObject.SetActive(false);
        }

        if (cooldownText != null)
        {
            cooldownText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerHealthManager>();
            if (player == null)
            {
                // No player spawned yet, nothing to do this frame.
                return;
            }
        }
        if (abilityController == null)
        {
            abilityController = player.GetComponent<PlayerAbilityController>();
            if (abilityController == null)
            {
                abilityController = FindFirstObjectByType<PlayerAbilityController>();
            }
        }
        
        if (!_initialized && player != null)
        {
            if (healthSlider != null)
            {
                healthSlider.minValue = 0f;
                healthSlider.maxValue = player.MaxHealth;
                healthSlider.value    = player.CurrentHealth;
            }

            _initialized = true;
        }

        if (healthSlider != null && player != null)
        {
            healthSlider.maxValue = player.MaxHealth;
            healthSlider.value    = player.CurrentHealth;
        }

        if (abilityController == null)
        {
            if (shieldIcon != null)
            {
                shieldIcon.gameObject.SetActive(false);
            }

            if (cooldownText != null)
            {
                cooldownText.gameObject.SetActive(false);
            }

            return;
        }

        if (shieldIcon != null)
        {
            shieldIcon.gameObject.SetActive(abilityController.AbilityActive);
        }

        if (cooldownText != null)
        {
            bool showCd = abilityController.AbilityOnCooldown && !abilityController.AbilityActive;

            if (showCd)
            {
                int seconds = Mathf.CeilToInt(abilityController.CooldownRemaining);
                cooldownText.text = $"{seconds} seconds left";
            }
            
            if (berserkIcon != null && abilityController != null)
            {
                berserkIcon.gameObject.SetActive(abilityController.BerserkActive);
            }

            cooldownText.gameObject.SetActive(showCd);
        }
    }
}
