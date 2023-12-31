using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigun : Gun
{
    [SerializeField] GameObject graphic;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;
    AudioSource _audioSource;

    protected override void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        base.Awake();
    }

    public override void FireUpdate()
    {
        graphic.transform.Rotate(0, 0, 5);
    }

    public override void Fire()
    {
        Vector3 targetDirection = TargetDirection();
        if (targetDirection == Vector3.zero) return;
        //Debug.DrawRay(transform.position, targetDirection);
        GameObject projectileGameobject = Instantiate(bulletPrefab);
        projectileGameobject.transform.position = firePoint.position;
        projectileGameobject.transform.rotation = Quaternion.LookRotation(targetDirection, Vector3.forward);
        MinigunProjectile projectile = projectileGameobject.GetComponent<MinigunProjectile>();
        projectile.gun = this;
        projectile.Velocity = targetDirection.normalized * 150;
        _audioSource.Play();
        //Rigidbody bulletRigidbody = projectileGameobject.GetComponent<Rigidbody>();
        //bulletRigidbody.AddForce(targetDirection.normalized * 10000);
    }
}
