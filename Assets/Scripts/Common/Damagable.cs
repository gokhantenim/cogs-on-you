using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damagable : MonoBehaviour
{
    public UnityEvent<float> OnDamaged;
    public UnityEvent<float> OnChangeHealth;
    public UnityEvent OnDead;
    public float Health = 0;
    public MagneticShield Shield;
    public float MaxHealth { get; private set; }
    public float HealthPercent => Health / MaxHealth;

    void Awake()
    {
        MaxHealth = Health;
        OnChangeHealth?.Invoke(HealthPercent);
    }

    public virtual void TakeDamage(float damage)
    {
        if (Shield != null && Shield.IsActive) return;
        if (Health <= 0) return;
        OnDamaged?.Invoke(damage);
        if (damage > Health)
        {
            damage = Health;
        }

        Health -= damage;
        OnChangeHealth?.Invoke(HealthPercent);

        if (Health <= 0)
        {
            OnDead?.Invoke();
        }
    }

    public void SetHealth(float health, bool alsoMaxHealth=false)
    {
        Health = health;
        if (alsoMaxHealth)
        {
            MaxHealth = health;
        }
        OnChangeHealth?.Invoke(HealthPercent);
    }

    public void ResetHealth()
    {
        SetHealth(MaxHealth);
    }
}
