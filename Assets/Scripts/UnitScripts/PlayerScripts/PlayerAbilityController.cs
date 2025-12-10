using AbilitySystem;
using System.Collections;
using UnityEditor.Playables;
using UnityEngine;

[RequireComponent(typeof(UnitScript))]
public class PlayerAbilityController : MonoBehaviour
{
    //[Header("References")]
    //[SerializeField] private PlayerHealthManager health;
    //[SerializeField] private MagicManager magic;

    //public PlayerClassType playerClass = PlayerClassType.Dwarf;
    //[Header("Player Class")]
    //[SerializeField] private CharacterClassSOBase _characterClass;

    //[Header("Defense Charm")]
    //public float charmDuration = 10f;
    //public float charmCooldown = 60f;
    //[Range(0f, 1f)] public float charmDamageMultiplier = 0.5f;
    
    private UnitScript _unitScript;
    private AbilityDefinition _basicAbility;
    private AbilityDefinition _ultimateAbility;

    //private bool _basicActive = false;
    [SerializeField]private bool _basicOnCooldown = false;
    //private float _basicCooldownRemaining;

    //private bool _ultimateActive = false;
    [SerializeField] private bool _ultimateOnCooldown = false;
    //private float _ultimateCooldownRemaining;
    
    //[Header("Berserk Shout")]
    //public float berserkDuration = 10f;
    //public float berserkCooldown = 30f;
    //public float berserkDamageMultiplier = 2f;

    //public bool BerserkActive { get; private set; }
    //public bool BerserkOnCooldown { get; private set; }
    //public float BerserkCooldownRemaining { get; private set; }

    //private float originalMagicDamageMultiplier = 1f;
    //[SerializeField] private SpriteRenderer playerSprite;  // or MeshRenderer if 3D
    //public Color berserkColor = Color.red;
    //private Color _originalColor;

    //[Header("Healing Cloud")]
    //public GameObject healCloudPrefab;
    //public float healCloudCooldown = 20f;
    //public bool HealCloudOnCooldown { get; private set; }
    //public float HealCloudCooldownRemaining { get; private set; }
    
    private void Awake()
    {
        if (!TryGetComponent<UnitScript>(out _unitScript)) return;

        _basicAbility = _unitScript.GetCharacterClass.GetBasicAbility;
        _ultimateAbility = _unitScript.GetCharacterClass.GetUltimateAbility;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            if (!_basicOnCooldown)
                StartCoroutine(BasicAbilityCoolDownCoro());

        if (Input.GetKeyDown(KeyCode.Q))
            if (!_ultimateOnCooldown)
                StartCoroutine(UltimateAbilityCoolDownCoro());
    }

    public IEnumerator BasicAbilityCoolDownCoro()
    {
        _basicAbility.UseAbility(this);
        _basicOnCooldown = true;
        float abilityCD = _basicAbility.GetRootNode.AbilityCD;

        for (float timer = 0f; timer < abilityCD; timer += Time.deltaTime)
        {
            //update a ui thing?
            yield return null;
        }

        _basicOnCooldown = false;
    }
    public IEnumerator UltimateAbilityCoolDownCoro()
    {
        _ultimateAbility.UseAbility(this);
        _ultimateOnCooldown = true;
        float abilityCD = _ultimateAbility.GetRootNode.AbilityCD;

        for (float timer = 0f; timer < abilityCD; timer += Time.deltaTime)
        {
            //update a ui thing?
            yield return null;
        }

        _ultimateOnCooldown = false;
    }

    /*
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
    
    }*/
}
