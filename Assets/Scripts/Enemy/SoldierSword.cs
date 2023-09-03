using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierSword : MonoBehaviour
{
    AudioSource _audioSource;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag != "Player") return;
        try
        {
            Damagable damagable = collision.gameObject.GetComponent<Damagable>();
            damagable.TakeDamage(500);
            _audioSource.Play();
        }
        catch (NullReferenceException) { }
    }
}
