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
    float _maxHealth = 0;
    public float HealthPercent => Health / _maxHealth;

    void Awake()
    {
        _maxHealth = Health;
        OnChangeHealth?.Invoke(HealthPercent);
    }

    public virtual void TakeDamage(float damage)
    {
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

    public void ResetHealth()
    {
        Health = _maxHealth;
        OnChangeHealth?.Invoke(HealthPercent);
    }
}
