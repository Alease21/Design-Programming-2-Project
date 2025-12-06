using System.Collections;
using UnityEngine;
public enum PlayerClassType
{
    Dwarf,        // Berserk Shout
    PlagueDoctor,   // Healing Cloud
    MagicGal        // Defense Charm
}

[RequireComponent(typeof(PlayerHealthManager))]
public class PlayerAbilityController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealthManager health;
    [SerializeField] private MagicManager magic;
    
    [Header("Player Class")]
    public PlayerClassType playerClass = PlayerClassType.Dwarf;
    
    [Header("Defense Charm")]
    public float charmDuration = 10f;
    public float charmCooldown = 60f;
    [Range(0f, 1f)] public float charmDamageMultiplier = 0.5f;

    public bool CharmActive { get; private set; }
    public bool CharmOnCooldown { get; private set; }
    public float CharmCooldownRemaining { get; private set; }

    public bool AbilityActive => CharmActive;
    public bool AbilityOnCooldown => CharmOnCooldown;
    public float CooldownRemaining => CharmCooldownRemaining;

    [Header("Berserk Shout")]
    public float berserkDuration = 10f;
    public float berserkCooldown = 30f;
    public float berserkDamageMultiplier = 2f;

    public bool BerserkActive { get; private set; }
    public bool BerserkOnCooldown { get; private set; }
    public float BerserkCooldownRemaining { get; private set; }

    private float originalMagicDamageMultiplier = 1f;
    [SerializeField] private SpriteRenderer playerSprite;  // or MeshRenderer if 3D
    public Color berserkColor = Color.red;
    private Color _originalColor;

    [Header("Healing Cloud")]
    public GameObject healCloudPrefab;
    public float healCloudCooldown = 20f;
    public bool HealCloudOnCooldown { get; private set; }
    public float HealCloudCooldownRemaining { get; private set; }

    private void Awake()
    {
        if (health == null)
        {
            health = GetComponent<PlayerHealthManager>();
        }

        if (magic == null)
        {
            magic = FindFirstObjectByType<MagicManager>();
        }

        if (playerSprite == null)
        {
            playerSprite = GetComponentInChildren<SpriteRenderer>();
        }

        if (playerSprite != null)
        {
            _originalColor = playerSprite.color;
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseClassAbility();
        }
    }
    
    private void UseClassAbility()
    {
        switch (playerClass)
        {
            case PlayerClassType.Dwarf:
            {
                TryActivateBerserk();
                break;
            }

            case PlayerClassType.PlagueDoctor:
            {
                TryActivateHealCloud();
                break;
            }

            case PlayerClassType.MagicGal:
            {
                TryActivateDefenseCharm();
                break;
            }
        }
    }
    
    public bool TryActivateDefenseCharm()
    {
        if (CharmActive || CharmOnCooldown)
        {
            return false;
        }

        StartCoroutine(DefenseCharmRoutine());
        return true;
    }

    private IEnumerator DefenseCharmRoutine()
    {
        CharmActive = true;
        CharmOnCooldown = true;

        float originalMultiplier = health.damageMultiplier;
        health.damageMultiplier = charmDamageMultiplier;

        float endActive = Time.time + charmDuration;
        while (Time.time < endActive)
        {
            yield return null;
        }

        CharmActive = false;
        health.damageMultiplier = originalMultiplier;

        float endCooldown = Time.time + charmCooldown;
        while (Time.time < endCooldown)
        {
            CharmCooldownRemaining = endCooldown - Time.time;
            yield return null;
        }

        CharmCooldownRemaining = 0f;
        CharmOnCooldown = false;
    }

    public bool TryActivateBerserk()
    {
        if (BerserkActive || BerserkOnCooldown || magic == null)
        {
            return false;
        }

        StartCoroutine(BerserkRoutine());
        return true;
    }

    private IEnumerator BerserkRoutine()
    {
        BerserkActive = true;
        BerserkOnCooldown = true;

        originalMagicDamageMultiplier = magic.damageMultiplier;
        magic.damageMultiplier = berserkDamageMultiplier;

        if (playerSprite != null)
        {
            playerSprite.color = berserkColor;
        }

        Debug.Log($"[BERSERK] ON. Multiplier {originalMagicDamageMultiplier} -> {magic.damageMultiplier}");

        float endActive = Time.time + berserkDuration;
        while (Time.time < endActive)
        {
            yield return null;
        }

        BerserkActive = false;
        magic.damageMultiplier = originalMagicDamageMultiplier;

        if (playerSprite != null)
        {
            playerSprite.color = _originalColor;
        }

        Debug.Log($"[BERSERK] OFF. Multiplier reset to {magic.damageMultiplier}");

        float endCooldown = Time.time + berserkCooldown;
        while (Time.time < endCooldown)
        {
            BerserkCooldownRemaining = endCooldown - Time.time;
            yield return null;
        }

        BerserkCooldownRemaining = 0f;
        BerserkOnCooldown = false;
    }
    
    public bool TryActivateHealCloud()
    {
        if (HealCloudOnCooldown || healCloudPrefab == null)
        {
            return false;
        }

        StartCoroutine(HealCloudRoutine());
        return true;
    }

    private IEnumerator HealCloudRoutine()
    {
        HealCloudOnCooldown = true;

        Instantiate(healCloudPrefab, transform.position, Quaternion.identity);

        float endCooldown = Time.time + healCloudCooldown;
        while (Time.time < endCooldown)
        {
            HealCloudCooldownRemaining = endCooldown - Time.time;
            yield return null;
        }

        HealCloudCooldownRemaining = 0f;
        HealCloudOnCooldown = false;
    }
}
