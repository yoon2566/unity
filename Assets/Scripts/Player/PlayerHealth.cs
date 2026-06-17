using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHP = 100f;
    private float currentHP;
    private bool isDead;

    public float CurrentHP => currentHP;
    public float MaxHP => maxHP;
    public bool IsDead => isDead;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDamaged;
    public event Action OnDeath;

    private void Awake()
    {
        currentHP = maxHP;
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(currentHP, maxHP);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHP = Mathf.Max(0f, currentHP - amount);
        OnHealthChanged?.Invoke(currentHP, maxHP);
        OnDamaged?.Invoke();

        if (currentHP <= 0f)
        {
            isDead = true;
            OnDeath?.Invoke();
        }
    }
}