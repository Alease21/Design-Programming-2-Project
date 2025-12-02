using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private EnemySO _enemySO;

    [SerializeField] private float _maxHealth;
    [SerializeField] private float _health;
    [SerializeField] private float _speed;

    public EnemySO GetEnemyBaseStats => _enemySO;

    private void Awake()
    {
        _maxHealth = _enemySO.maxHealth;
        _health = _maxHealth;
        _speed = _enemySO.speed;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void ChangeHealth(float amount, bool isGain = false)
    {
        if (isGain)
        {
            _health += amount;
            if (_health > _maxHealth)
                _health = _maxHealth;
        }
        else
        {
            _health -= amount;
            if (_health < 0f)
                _health = 0f;
        }

        if (_health <= 0f)
        {
            //do death things
            Destroy(gameObject);
        }
    }
}
