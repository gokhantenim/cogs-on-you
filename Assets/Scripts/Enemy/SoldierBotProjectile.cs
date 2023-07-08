using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierBotProjectile : MonoBehaviour
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
    }

    //private void OnTriggerEnter(Collider collision)
    //{
    //    Process(collision);
    //}
    private void OnCollisionEnter(Collision collision)
    {
        Process(collision.collider);
    }

    void Process(Collider collider)
    {
        //Debug.Log(collider.gameObject.tag);
        if (collider.gameObject.tag == "Enemy" || collider.gameObject.tag == "PlayerProjectile") return;

        if (collider.gameObject.tag == "Player")
        {
            try
            {
                Damagable damagable = collider.gameObject.GetComponent<Damagable>();
                damagable.TakeDamage(250);
            }
            catch (NullReferenceException) { }

        }
        Destroy(gameObject);
    }
}
