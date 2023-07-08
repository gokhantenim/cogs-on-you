using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigun : Gun
{
    [SerializeField] GameObject graphic;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;

    //public Minigun()
    //{
    //    damage = 100;
    //    magazineCapacity = 50;
    //    fireRate = 10;
    //    reloadTime = 3;
    //}

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
        projectile.Velocity = targetDirection.normalized * 75;
        //Rigidbody bulletRigidbody = projectileGameobject.GetComponent<Rigidbody>();
        //bulletRigidbody.AddForce(targetDirection.normalized * 10000);
    }
}
