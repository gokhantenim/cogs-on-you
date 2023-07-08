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
    float _healthPercent => Health / _maxHealth;

    void Awake()
    {
        _maxHealth = Health;
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
        OnChangeHealth?.Invoke(_healthPercent);

        if (Health <= 0)
        {
            OnDead?.Invoke();
        }
    }
}
