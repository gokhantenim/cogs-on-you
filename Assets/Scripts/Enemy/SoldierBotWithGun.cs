using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using DG.Tweening;
using System;
using UnityEngine.PlayerLoop;

public class SoldierBotWithGun : Enemy
{
    StateMachine _stateMachine = new();
    State _wanderState;
    State _chaseState;
    //State _aimState;
    State _attackState;

    NavMeshAgent _agent;
    [SerializeField] Animator _animator;
    [SerializeField] Transform _gunTarget;
    [SerializeField] Rig _gunArmRig;
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] GameObject _gunLaser;
    [SerializeField] GameObject _gunFirePoint;
    [SerializeField] GameObject _gunLaserEnd;
    Vector3 _lastKnownPosition = Vector3.zero;

    Vector3 _initialLaserScale;
    Vector3 _lastAttackPosition;
    Coroutine _attackCoroutine;
    AudioSource _audioSource;
    string _armRigTweenId => "gun-arm-rig-" + gameObject.GetInstanceID();
    string _laserScaleTweenId => "laser-" + gameObject.GetInstanceID();

    protected override void Awake()
    {
        _wanderState = new(WanderEnter, WanderExit, Wander);
        _chaseState = new(ChaseEnter, ChaseExit, Chase);
        //_aimState = new(AimEnter, AimExit, Aim);
        _attackState = new(AttackEnter, AttackExit, Attack);

        _audioSource = GetComponent<AudioSource>();
        _agent = GetComponent<NavMeshAgent>();
        _animator.fireEvents = false;
        base.Awake();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        DOTween.Kill(_armRigTweenId);
        DOTween.Kill(_laserScaleTweenId);
    }

    void Start()
    {
        _initialLaserScale = _gunLaser.transform.localScale;

        _stateMachine.SetState(_wanderState);
    }

    void Update()
    {
        //LookForPlayer();
        MoveAnimation();
        _stateMachine.Update();
    }

    void MoveAnimation()
    {
        Vector3 movement = transform.worldToLocalMatrix * _agent.velocity;
        _animator.SetFloat("move_z", movement.z);
    }

    public void TakeDamage()
    {
        if (!_stateMachine.IsState(_wanderState)) return;
        UpdatePlayerKnownPosition();
        _stateMachine.SetState(_chaseState);
    }

    public void OnScanPlayer(List<GameObject> seenPlayers)
    {
        bool isPlayerSeen = seenPlayers.Count > 0;
        if (isPlayerSeen)
        {
            UpdatePlayerKnownPosition();
        }
        if (isPlayerSeen && _stateMachine.IsState(_wanderState))
        {
            _stateMachine.SetState(_chaseState);
            return;
        }
        if(!isPlayerSeen && !_stateMachine.IsState(_wanderState))
        {
            _stateMachine.SetState(_wanderState);
        }
    }

    void UpdatePlayerKnownPosition()
    {
        if (PlayerController.Instance == null) return;
        _lastKnownPosition = PlayerController.Instance.transform.position;
    }

    void WanderEnter()
    {
        SetAgentPlay(false);
    }

    void WanderExit()
    {
        SetAgentPlay(true);
    }

    void Wander()
    {
        if (_agent.remainingDistance > 1) return;
        SetRandomWanderPoint();
    }

    void SetRandomWanderPoint()
    {
        for(int i=0;i<30; i++)
        {
            NavMeshHit hit;
            Vector3 randomPoint = transform.position + UnityEngine.Random.insideUnitSphere * 50;
            if(NavMesh.SamplePosition(randomPoint, out hit, 100, NavMesh.AllAreas))
            {
                SetAgentDestination(hit.position);
                break;
            }
        }
    }

    bool CheckFacingWithPlayer(float checkAngle = 45, float checkDistance = 30)
    {
        if (Vector3.Distance(transform.position, _lastKnownPosition) > checkDistance) return false;

        Vector3 direction = _lastKnownPosition - transform.position;
        direction.y = transform.forward.y;
        float angle = Vector3.Angle(direction, transform.forward);
        if (angle > checkAngle) return false;

        return true;
    }

    void Chase()
    {
        SetAgentDestination(_lastKnownPosition);
        if (CheckFacingWithPlayer(15))
        {
            _stateMachine.SetState(_attackState);
            return;
        }
    }

    void ChaseEnter()
    {
        SetAgentPlay(false);
        SetAgentDestination(_lastKnownPosition);
    }

    void ChaseExit()
    {
        SetAgentPlay(true);
    }

    void SetAgentPlay(bool isStopped)
    {
        if (!_agent.isOnNavMesh) return;
        _agent.isStopped = isStopped;
    }

    void SetAgentDestination(Vector3 position)
    {
        if (!_agent.isOnNavMesh) return;
        _agent.SetDestination(position);
    }

    void SetArmRigWeight(float value)
    {
        DOTween.To((val) =>
        {
            _gunArmRig.weight = val;
        }, _gunArmRig.weight, value, 0.5f)
            .SetId(_armRigTweenId);
    }

    //void AimEnter()
    //{
    //    SetArmRigWeight(1);
    //    _lastAttackPosition = _lastKnownPosition;
    //}

    //void AimExit()
    //{
    //    SetArmRigWeight(0);
    //}

    //void Aim()
    //{
    //    if (!CheckFacingWithPlayer(45))
    //    {
    //        _stateMachine.SetState(_chaseState);
    //        return;
    //    }

    //    Vector3 targetPosition = _lastKnownPosition + new Vector3(0, 2);
    //    Vector3 setPosition = Vector3.Lerp(_gunTarget.position, targetPosition, Time.deltaTime * 4);

    //    _gunTarget.position = setPosition;

    //    float distance = Vector3.Distance(targetPosition, _gunTarget.position);
    //    if (distance < 0.2f)
    //    {
    //        _stateMachine.SetState(_attackState);
    //    }
    //}

    //void LaserScale()
    //{
    //    float robotScale = _animator.gameObject.transform.localScale.z;
    //    float distance = Vector3.Distance(_gunFirePoint.transform.position, _gunLaserEnd.transform.position) / robotScale;
    //    Vector3 laserSize = new Vector3(_initialLaserScale.x, distance / 2f, _initialLaserScale.z);
    //    _gunLaser.transform.localScale = laserSize;
    //    Vector3 middlePoint = (_gunFirePoint.transform.position + _gunLaserEnd.transform.position) / 2f;
    //    _gunLaser.transform.position = middlePoint;
    //    Vector3 rotationDirection = _gunLaserEnd.transform.position - _gunFirePoint.transform.position;
    //    _gunLaser.transform.up = rotationDirection;
    //}

    void AttackEnter()
    {
        SetArmRigWeight(1);
        _lastAttackPosition = _lastKnownPosition;
        _attackCoroutine = StartCoroutine(AttackEnumerator());
        //SetLaserAlpha(0.3f);
        //_gunLaserEnd.transform.position = _gunFirePoint.transform.position;
        //_gunLaser.SetActive(true);
        //_gunLaserEnd.transform.DOMove(_gunTarget.position, 0.5f)
        //    .SetId(_laserScaleTweenId)
        //    .OnUpdate(() =>
        //    {
        //        LaserScale();
        //    })
        //    .OnComplete(OnLaserReached);
    }

    void AttackExit()
    {
        SetArmRigWeight(0);
        //_gunLaser.SetActive(false);
        //DOTween.Kill(_laserScaleTweenId);
        try
        {
            StopCoroutine(_attackCoroutine);
        }
        catch (NullReferenceException) { }
    }

    void Attack()
    {
        if (!CheckFacingWithPlayer(45))
        {
            _stateMachine.SetState(_chaseState);
            return;
        }
        //if (Vector3.Distance(_lastAttackPosition, _lastKnownPosition) > 5)
        //{
        //    _stateMachine.SetState(_chaseState);
        //    return;
        //}

        Vector3 targetPosition = _lastKnownPosition + new Vector3(0, 2);
        Vector3 setPosition = Vector3.Lerp(_gunTarget.position, targetPosition, Time.deltaTime * 4);

        _gunTarget.position = setPosition;
    }

    IEnumerator AttackEnumerator()
    {
        //for (int i = 0; i < 1; i++)
        //{
        //    _gunLaser.SetActive(false);
        //    yield return new WaitForSeconds(0.1f);
        //    _gunLaser.SetActive(true);
        //    yield return new WaitForSeconds(0.4f);
        //}
        //_gunLaser.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        Fire();
        yield return new WaitForSeconds(0.2f);
        Fire();
        yield return new WaitForSeconds(0.2f);
        Fire();
        yield return new WaitForSeconds(1);
        AttackEnter();
    }

    //void SetLaserAlpha(float alpha)
    //{
    //    MeshRenderer meshRenderer = _gunLaser.GetComponent<MeshRenderer>();
    //    Color color = meshRenderer.material.color;
    //    color.a = alpha;
    //    meshRenderer.material.color = color;
    //}

    //void OnLaserReached()
    //{
    //    //SetLaserAlpha(0.75f);
    //    _attackCoroutine = StartCoroutine(AttackEnumerator());
    //}

    void Fire()
    {
        Vector3 targetPosition = _lastAttackPosition + new Vector3(0, 2);
        Vector3 targetDirection = targetPosition - _gunFirePoint.transform.position;
        if (targetDirection == Vector3.zero) return;
        //Debug.DrawRay(transform.position, targetDirection);
        GameObject projectileGameobject = Instantiate(_bulletPrefab);
        projectileGameobject.transform.position = _gunFirePoint.transform.position;
        projectileGameobject.transform.rotation = Quaternion.LookRotation(targetDirection, Vector3.forward);
        SoldierBotProjectile bullet = projectileGameobject.GetComponent<SoldierBotProjectile>();
        bullet.Velocity = targetDirection.normalized * 50;
        _audioSource.Play();
        //Rigidbody bulletRigidbody = projectileGameobject.GetComponent<Rigidbody>();
        //bulletRigidbody.AddForce(targetDirection.normalized * 1000);
    }
}
