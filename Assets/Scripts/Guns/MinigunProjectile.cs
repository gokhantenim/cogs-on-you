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

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward * -1, out hit, 5))
        {
            Process(hit.collider);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        Process(collision);
    }

    void Process(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) return;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Projectile")) return;

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
