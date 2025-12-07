using System.Collections;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour, IPlayer
{
    [Header("Health")]
    public float maxHealth = 10f;
    private float _currentHealth;

    [Header("Defense Charm Ability")]
    public float abilityDuration = 10f;
    public float abilityCooldown = 60f;
    [Range(0f, 1f)]
    public float damageNullifier = 0.5f;
    
    public float CurrentHealth => _currentHealth;
    public float MaxHealth => maxHealth;
    public bool AbilityActive { get; private set; }
    public bool AbilityOnCooldown { get; private set; }
    public float CooldownRemaining { get; private set; }

    void Awake()
    {
        _currentHealth = maxHealth;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TryActivateDamageShield();
        }
    }

    public void TakeDamage(float damage)
    {
        float finalDamage = AbilityActive ? damage * damageNullifier : damage;

        _currentHealth -= finalDamage;
        //Debug.Log($"Player took {finalDamage} (raw {damage}). Health now {_currentHealth}");

        if (_currentHealth <= 0f)
        {
            Destroy(gameObject);
        }
    }


    public bool TryActivateDamageShield()
    {
        if (AbilityActive || AbilityOnCooldown)
        {
            return false;
        }

        StartCoroutine(DamageShieldRoutine());
        return true;
    }

    private IEnumerator DamageShieldRoutine()
    {
        // Activate
        AbilityActive = true;
        AbilityOnCooldown = true;
        float endActive = Time.time + abilityDuration;

        while (Time.time < endActive)
        {
            yield return null;            
        }
        
        // Deactivate
        AbilityActive = false;

        // Cooldown
        float endCd = Time.time + abilityCooldown;
        while (Time.time < endCd)
        {
            CooldownRemaining = Mathf.Max(0f, endCd - Time.time);
            yield return null;
        }

        CooldownRemaining = 0f;
        AbilityOnCooldown = false;
    }
}