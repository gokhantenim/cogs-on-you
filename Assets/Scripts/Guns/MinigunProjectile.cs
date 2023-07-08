using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigunProjectile : MonoBehaviour
{
    public Gun gun;
    public Vector3 Velocity = Vector3.zero;
    Rigidbody _rigidbody;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Destroy(gameObject, 3);
    }
    private void Update()
    {
        _rigidbody.velocity = Velocity;
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.tag);
        //if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "PlayerProjectile") return;
        if (collision.gameObject.tag == "Enemy")
        {
            try
            {
                Damagable damagable = collision.gameObject.GetComponent<Damagable>();
                damagable.TakeDamage(gun.damage);
            }
            catch (NullReferenceException) { }
        }
        Destroy(gameObject);
    }
}
