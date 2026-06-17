using UnityEngine;

public class ZombieHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHP = 40f;
    private float currentHP;

    public static System.Action<GameObject> OnAnyZombieDeath;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        if (currentHP <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        OnAnyZombieDeath?.Invoke(gameObject);
        Destroy(gameObject);
    }
}
