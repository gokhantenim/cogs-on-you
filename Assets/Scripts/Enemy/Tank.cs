using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Tank : Enemy
{
    StateMachine _stateMachine = new();
    State _wanderState;
    State _chaseState;
    State _attackState;

    NavMeshAgent _agent;
    [SerializeField] Animator _animator;
    [SerializeField] GameObject _turret;
    [SerializeField] GameObject _gun;
    [SerializeField] GameObject _firePoint;
    [SerializeField] GameObject _projectilePrefab;
    Vector3 _lastKnownPosition = Vector3.zero;
    float _lastRotationY;
    AudioSource _audioSource;

    protected override void Awake()
    {
        _lastRotationY = transform.rotation.eulerAngles.y;
        _wanderState = new(WanderEnter, WanderExit, Wander);
        //_chaseState = new(ChaseEnter, ChaseExit, Chase);
        _attackState = new(AttackEnter, AttackExit, Attack);

        _audioSource = GetComponent<AudioSource>();
        _agent = GetComponent<NavMeshAgent>();
        SetAgentPlay(true);
        base.Awake();
    }

    void Start()
    {
        _stateMachine.SetState(_wanderState);
    }

    // Update is called once per frame
    void Update()
    {
        MoveAnimation();
        _stateMachine.Update();
    }

    void MoveAnimation()
    {
        float rotation = _agent.transform.rotation.eulerAngles.y - _lastRotationY;
        if (rotation < -0.1)
        {
            rotation = -1;
        }
        else if (rotation > 0.1)
        {
            rotation = 1;
        }
        else
        {
            rotation = 0;
        }

        _lastRotationY = _agent.transform.rotation.eulerAngles.y;

        Vector3 movement = transform.worldToLocalMatrix * _agent.velocity;
        _animator.SetBool("forward", movement.z > 0.5);
        _animator.SetFloat("rotation", rotation);
    }

    public void OnScanPlayer(List<GameObject> seenPlayers)
    {
        bool isPlayerSeen = seenPlayers.Count > 0;
        if (isPlayerSeen)
        {
            UpdatePlayerKnownPosition();
        }
        if (isPlayerSeen)
        {
            _stateMachine.SetState(_attackState);
            return;
        }
        if (!isPlayerSeen)
        {
            _stateMachine.SetState(_wanderState);
        }
    }
    public void TakeDamage()
    {
        if (_stateMachine.IsState(_attackState)) return;
        UpdatePlayerKnownPosition();
        SetAgentDestination(_lastKnownPosition);
        //_stateMachine.SetState(_chaseState);
    }

    void UpdatePlayerKnownPosition()
    {
        if (PlayerController.Instance == null) return;
        _lastKnownPosition = PlayerController.Instance.transform.position;
    }

    void SetAgentPlay(bool play)
    {
        if (!_agent.isOnNavMesh) return;
        _agent.isStopped = !play;
    }
    void SetAgentDestination(Vector3 position)
    {
        if (!_agent.isOnNavMesh) return;
        _agent.SetDestination(position);
    }

    void WanderEnter()
    {
        SetAgentDestination(_lastKnownPosition);
        //SetAgentPlay(true);
    }

    void WanderExit()
    {
        //SetAgentPlay(false);
    }

    void Wander()
    {
        if (_agent.remainingDistance > 1) return;
        SetRandomWanderPoint();
    }

    void SetRandomWanderPoint()
    {
        for (int i = 0; i < 30; i++)
        {
            NavMeshHit hit;
            Vector3 randomPoint = transform.position + UnityEngine.Random.insideUnitSphere * 50;
            if (NavMesh.SamplePosition(randomPoint, out hit, 100, NavMesh.AllAreas))
            {
                SetAgentDestination(hit.position);
                break;
            }
        }
    }

    //void Chase()
    //{
    //    SetAgentDestination(_lastKnownPosition);
    //    GunRotation();
    //    //if (CheckFacingWithPlayer(15))
    //    //{
    //    //    _stateMachine.SetState(_attackState);
    //    //    return;
    //    //}
    //}

    //void ChaseEnter()
    //{
    //    SetAgentPlay(true);
    //    SetAgentDestination(_lastKnownPosition);
    //}

    //void ChaseExit()
    //{
    //    SetAgentPlay(false);
    //}

    void AttackEnter()
    {
        //SetAgentPlay(true);
        InvokeRepeating("Fire", 2, 2);
    }

    void AttackExit()
    {
        CancelInvoke();
    }

    void Attack()
    {
        SetAgentDestination(_lastKnownPosition);
        GunRotation();
    }

    void GunRotation()
    {
        Vector3 targetPosition = (_lastKnownPosition + new Vector3(0, 3));
        // turret
        Vector3 direction = targetPosition - _turret.transform.position;
        float turretRotation = Quaternion.LookRotation(direction).eulerAngles.y;
        turretRotation = Mathf.LerpAngle(_turret.transform.eulerAngles.y, turretRotation, Time.deltaTime * 2);
        _turret.transform.eulerAngles = _turret.transform.eulerAngles.Rewrite(y: turretRotation);

        // gun
        Vector3 gunDirection = _gun.transform.position - targetPosition;
        float gunRotation = Quaternion.LookRotation(gunDirection).eulerAngles.x;
        gunRotation = gunRotation > 30 && gunRotation < 180 ? 30 : gunRotation;// clamp
        gunRotation = Mathf.LerpAngle(_gun.transform.eulerAngles.x, gunRotation, Time.deltaTime * 2);
        _gun.transform.eulerAngles = _gun.transform.eulerAngles.Rewrite(x: gunRotation);
    }

    void Fire()
    {
        Vector3 targetPosition = _lastKnownPosition + new Vector3(0, 2);
        Vector3 targetDirection = targetPosition - _firePoint.transform.position;
        if (targetDirection == Vector3.zero) return;

        GameObject projectileGameobject = Instantiate(_projectilePrefab);
        projectileGameobject.transform.position = _firePoint.transform.position;
        projectileGameobject.transform.rotation = Quaternion.LookRotation(targetDirection, Vector3.forward);

        TankProjectile bullet = projectileGameobject.GetComponent<TankProjectile>();
        bullet.Velocity = _gun.transform.forward.normalized * -1 * 50;
        _audioSource.Play();
    }
}
