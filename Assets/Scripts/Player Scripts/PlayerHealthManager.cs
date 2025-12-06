using UnityEngine;

public class PlayerHealthManager : MonoBehaviour, IPlayer
{
    [Header("Health")]
    public float maxHealth = 10f;
    private float _currentHealth;

    [Header("Damage Handling")]
    [Tooltip("Multiplies all incoming damage. 1 = normal, 0.5 = half damage, 0 = invulnerable.")]
    public float damageMultiplier = 1f;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (_currentHealth <= 0f)
        {
            return;
        }
        if (damage <= 0f)
        {
            return;
        }

        float finalDamage = damage * damageMultiplier;
        _currentHealth -= finalDamage;

        // Debug.Log($"Took {finalDamage} damage. HP now {_currentHealth}");

        if (_currentHealth <= 0f)
        {
            _currentHealth = 0f;
            // TODO: death logic here
            Destroy(gameObject);
        }
    }

    public void Heal(float amount)
    {
        if (amount <= 0f || _currentHealth <= 0f)
        {
            return;
        }

        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0f, maxHealth);
    }
}