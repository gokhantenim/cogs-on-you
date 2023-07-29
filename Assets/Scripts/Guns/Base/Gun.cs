using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GunSlot _slot = null;
    public Enemy _target = null;
    protected StateMachine _stateMachine = new();
    protected Dictionary<GunStates, State> _states = new Dictionary<GunStates, State>();

    public GunDefinition Definition;
    public int Level = 0;

    public float reloadTime => Definition.Levels[Level].ReloadTime;
    public float damage => Definition.Levels[Level].Damage;
    public float fireRate => Definition.Levels[Level].FireRate;
    public float magazineCapacity => Definition.Levels[Level].MagazineCapacity;

    public float magazine = 0;

    public GunStates state;
    public enum GunStates
    {
        NONE,
        IDLE,
        FIRE,
        RELOAD
    }

    protected virtual void Awake()
    {
        _states = new Dictionary<GunStates, State>()
        {
            [GunStates.IDLE] = new State(IdleStart, IdleStop, IdleUpdate),
            [GunStates.FIRE] = new State(FireStart, FireStop, FireUpdate),
            [GunStates.RELOAD] = new State(ReloadStart, ReloadStop, ReloadUpdate)
        };
        SetState(GunStates.IDLE);
    }

    void OnEnable()
    {
        magazine = magazineCapacity;
    }

    void Update()
    {
        _stateMachine.Update();
    }

    public void SetTarget(Enemy target)
    {
        _target = target;
        if (IsState(GunStates.RELOAD)) return;
        SetState(_target == null ? GunStates.IDLE : GunStates.FIRE);
    }

    public void SetState(GunStates _state)
    {
        _stateMachine.SetState(_states[_state]);
    }

    public bool IsState(GunStates _state)
    {
        return _stateMachine.IsState(_states[_state]);
    }

    protected Vector3 TargetPosition()
    {
        if (_target.TargetPosition != null) return _target.TargetPosition.position;
        return _target.transform.position;
    }

    protected Vector3 TargetDirection()
    {
        if (_target == null) return Vector3.zero;
        Vector3 targetPosition = TargetPosition();
        return targetPosition - transform.position;
    }

    Coroutine _fireCoroutine = null;
    IEnumerator Firing()
    {
        while (magazine > 0)
        {
            Fire();
            magazine--;
            if (magazine <= 0)
            {
                break;
            }
            yield return new WaitForSeconds(1 / fireRate);
        }
        SetState(GunStates.RELOAD);
    }

    protected IEnumerator Reloading()
    {
        yield return new WaitForSeconds(reloadTime);
        magazine = magazineCapacity;

        SetState(_target == null ? GunStates.IDLE : GunStates.FIRE);
    }

    public virtual void IdleStart(){
    }

    public virtual void IdleUpdate(){}

    public virtual void IdleStop(){
    }

    public virtual void FireStart(){
        _fireCoroutine = StartCoroutine(Firing());
    }

    public virtual void FireUpdate(){}

    public virtual void Fire(){
        Debug.LogWarning("bang");
    }

    public virtual void FireStop()
    {
        if (_fireCoroutine == null) return;
        StopCoroutine(_fireCoroutine);
        _fireCoroutine = null;
    }

    public virtual void ReloadStart()
    {
        StartCoroutine(Reloading());
    }

    public virtual void ReloadUpdate(){}

    public virtual void ReloadStop(){}
}
