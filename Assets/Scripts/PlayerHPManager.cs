using System;
using UnityEngine;

public class PlayerHPManager : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHP = 100f;

    private float currentHP;
    private bool isDead;

    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;
    public bool IsDead => isDead;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDied;

    private void Awake()
    {
        maxHP = Mathf.Max(maxHP, 1f);
        currentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        if (isDead || damage <= 0f)
        {
            return;
        }

        currentHP = Mathf.Max(currentHP - damage, 0f);
        OnHealthChanged?.Invoke(currentHP, maxHP);

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead || amount <= 0f)
        {
            return;
        }

        currentHP = Mathf.Min(currentHP + amount, maxHP);
        OnHealthChanged?.Invoke(currentHP, maxHP);
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        PlayerMovementController movementController = GetComponent<PlayerMovementController>();
        if (movementController != null)
        {
            movementController.enabled = false;
        }

        PlayerWeaponHandler weaponHandler = GetComponent<PlayerWeaponHandler>();
        if (weaponHandler != null)
        {
            weaponHandler.enabled = false;
        }

        OnDied?.Invoke();
    }
}