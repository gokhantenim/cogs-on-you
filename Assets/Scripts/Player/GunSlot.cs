using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

public class GunSlot : MonoBehaviour
{
    [SerializeField] Rig _rig;
    [SerializeField] GameObject _ikTarget;
    [SerializeField] Transform _ikIdlePosition;
    Enemy _target;
    public Gun CurrentGun;
    public Gun TempGun;
    public GunDefinition DefaultGun;

    void Update()
    {
        if (_target == null || _rig == null) return;

        //Debug.DrawRay(transform.position, TargetDirection());

        _ikTarget.transform.position = Vector3.Lerp(_ikTarget.transform.position, TargetPosition(), Time.deltaTime * 4);
        _ikTarget.transform.rotation = Quaternion.LookRotation(TargetDirection(), Vector3.up);
    }

    public void SetTargetToIdle()
    {
        if (_rig == null) return;
        _rig.weight = 1;
        if (_ikIdlePosition == null || _ikTarget == null) return;
        _ikTarget.transform.position = _ikIdlePosition.position;
        _ikTarget.transform.rotation = _ikIdlePosition.rotation;
    }

    protected Vector3 TargetPosition()
    {
        if (_target.TargetPosition != null) return _target.TargetPosition.position;
        return _target.transform.position;
    }

    protected Vector3 TargetDirection()
    {
        if (_target == null) return Vector3.zero;
        return _target.transform.position - transform.position;
    }

    public void SetTarget(Enemy target)
    {
        if (_target != null && _target.Equals(target)) return;
        _target = target;

        if (CurrentGun == null) return;
        CurrentGun.SetTarget(target);

        if (_rig == null) return;
        if (target != null) return;
        SetTargetToIdle();
        //_rig.weight = target == null ? 0 : 1;
    }

    public void InstantiateGun(GunDefinition gun)
    {
        RemoveTempGun();
        if (CurrentGun != null)
        {
            CurrentGun.gameObject.SetActive(false);
        }
        GameObject gunObject = Instantiate(gun.GunPrefab, transform, true);
        gunObject.transform.localPosition = Vector3.zero;
        gunObject.transform.localRotation = Quaternion.identity;
        TempGun = gunObject.GetComponent<Gun>();
    }

    public void InstallGun(GunDefinition gun)
    {
        RemoveSettledGun();
        InstantiateGun(gun);
        CurrentGun = TempGun;
        TempGun = null;
        CurrentGun.SetTarget(_target);
    }

    public void RemoveTempGun()
    {
        if (TempGun == null) return;
        Destroy(TempGun.gameObject);
        TempGun = null;
    }

    public void RemoveSettledGun()
    {
        if (CurrentGun == null) return;
        Destroy(CurrentGun.gameObject);
        CurrentGun = null;
    }

    public void Back2Normal()
    {
        RemoveTempGun();
        if(CurrentGun != null)
        {
            CurrentGun.gameObject.SetActive(true);
        }
    }

    //public void InstallGun(GunDefinition gun)
    //{
    //    SetGun(gun);
    //    GameManager.Instance.TotalCogs -= gun.Levels[0].Cost;
    //}

    public bool UpgradeGun()
    {
        if (CurrentGun.Definition.Levels.Length < CurrentGun.Level + 1) return false;
        CurrentGun.Level++;
        return true;
    }
}
