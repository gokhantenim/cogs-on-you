using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using DG.Tweening;
using System;
using UnityEngine.PlayerLoop;

public class SoldierBotWithSword : Enemy
{
    StateMachine _stateMachine = new();
    State _wanderState;
    State _chaseState;
    State _attackState;

    NavMeshAgent _agent;
    [SerializeField] Animator _animator;
    Vector3 _lastKnownPosition = Vector3.zero;

    float _wanderSpeed = 3;
    float _chaseSpeed = 8;
    float _attackDistance = 5;

    protected override void Awake()
    {
        _wanderState = new(WanderEnter, WanderExit, Wander);
        _chaseState = new(ChaseEnter, ChaseExit, Chase);
        _attackState = new(AttackEnter, AttackExit, Attack);

        //_audioSource = GetComponent<AudioSource>();
        _agent = GetComponent<NavMeshAgent>();
        _animator.fireEvents = false;
        base.Awake();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    void Start()
    {
        _stateMachine.SetState(_wanderState);
    }

    void Update()
    {
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
        SetAgentSpeed(_wanderSpeed);
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
        float distance = Vector3.Distance(transform.position, _lastKnownPosition);
        if (distance > checkDistance) return false;

        Vector3 direction = _lastKnownPosition - transform.position;
        direction.y = transform.forward.y;
        float angle = Vector3.Angle(direction, transform.forward);
        if (angle > checkAngle) return false;

        return true;
    }

    void Chase()
    {
        SetAgentDestination(_lastKnownPosition);
        if (CheckFacingWithPlayer(10, _attackDistance))
        {
            _stateMachine.SetState(_attackState);
            return;
        }
    }

    void ChaseEnter()
    {
        SetAgentPlay(false);
        SetAgentDestination(_lastKnownPosition);
        SetAgentSpeed(_chaseSpeed);
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

    void SetAgentSpeed(float speed)
    {
        if (!_agent.isOnNavMesh) return;
        _agent.speed = speed;
    }

    void SetAgentDestination(Vector3 position)
    {
        if (!_agent.isOnNavMesh) return;
        _agent.SetDestination(position);
    }

    void AttackEnter()
    {
        //_lastAttackPosition = _lastKnownPosition;
        _animator.SetBool("attack", true);
        SetAgentSpeed(0);
    }

    void AttackExit()
    {
        _animator.SetBool("attack", false);
    }

    void Attack()
    {
        if (!CheckFacingWithPlayer(10, _attackDistance))
        {
            _stateMachine.SetState(_chaseState);
            return;
        }
    }
}
