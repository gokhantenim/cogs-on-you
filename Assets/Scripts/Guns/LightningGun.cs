using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningGun : Gun
{
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] Transform _firePoint;
    [SerializeField] float _lightningFrequency = 1;
    Vector3[] points;
    AudioSource _audioSource;

    protected override void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        base.Awake();
    }

    public override void FireUpdate()
    {
        MakeLightning();
    }

    public override void Fire()
    {
        Vector3 targetDirection = TargetDirection();
        if (targetDirection == Vector3.zero) return;

        RaycastHit hit;
        if(Physics.Raycast(_firePoint.position, targetDirection, out hit))
        {
            if (hit.collider.tag != "Enemy") return;
            try
            {
                Damagable damagable = hit.collider.gameObject.GetComponent<Damagable>();
                damagable.TakeDamage(damage);
            }
            catch (NullReferenceException) { }
        }
    }

    void MakeLightning()
    {
        Vector3 targetPosition = TargetPosition();
        Vector3 targetDirection = TargetDirection();
        if (targetDirection == Vector3.zero) return;

        float targetDistance = Vector3.Distance(targetPosition, _firePoint.position);
        int totalPoint = Mathf.RoundToInt(targetDistance / _lightningFrequency);
        points = new Vector3[totalPoint];
        points[0] = Vector3.zero;

        Vector3 directionLocal = _lineRenderer.transform.worldToLocalMatrix * targetDirection.normalized;

        for (int pointIndex = 1; pointIndex < totalPoint; pointIndex++)
        {
            Vector3 randomPoint = UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(0.15f, 0.5f);
            Vector3 pointPosition = _lightningFrequency * pointIndex * directionLocal + randomPoint;
            points[pointIndex] = pointPosition;
        }

        _lineRenderer.positionCount = points.Length;
        _lineRenderer.SetPositions(points);
    }

    public override void FireStart()
    {
        _audioSource.Play();
        base.FireStart();
    }

    public override void FireStop()
    {
        _audioSource.Stop();
        _lineRenderer.positionCount = 0;
        base.FireStop();
    }
}
