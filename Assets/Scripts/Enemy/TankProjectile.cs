using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankProjectile : MonoBehaviour
{
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

    void Process(Collider collider)
    {
        //if (collider.gameObject.tag == "Enemy" || collider.gameObject.tag == "PlayerProjectile") return;

        if (collider.gameObject.tag != "Player") return;
        try
        {
            Damagable damagable = collider.gameObject.GetComponent<Damagable>();
            damagable.TakeDamage(1000);
        }
        catch (NullReferenceException) { }
        Destroy(gameObject);
    }
}
