using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Gun
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;
    AudioSource _audioSource;

    protected override void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        base.Awake();
    }
    public override void Fire()
    {
        Vector3 targetDirection = TargetDirection();
        if (targetDirection == Vector3.zero) return;
        //Debug.DrawRay(transform.position, targetDirection);
        GameObject bulletGameobject = Instantiate(bulletPrefab);
        bulletGameobject.transform.position = firePoint.position;
        CannonProjectile projectile = bulletGameobject.GetComponent<CannonProjectile>();
        projectile.gun = this;
        projectile.Velocity = targetDirection.normalized * 75;
        _audioSource.Play();
        //Rigidbody bulletRigidbody = bulletGameobject.GetComponent<Rigidbody>();
        //bulletRigidbody.AddForce(targetDirection.normalized * 10000);
    }
}
