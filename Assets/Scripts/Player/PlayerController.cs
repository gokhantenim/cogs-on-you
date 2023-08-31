using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using DG.Tweening;
using UnityEngine.Animations.Rigging;
using System.Threading.Tasks;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(Scanner))]
[RequireComponent(typeof(Cogs))]
[RequireComponent(typeof(Damagable))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController _instance;
    public static PlayerController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerController>(true);
            }
            return _instance;
        }
    }
    public StateMachine StateMachine = new();
    public State TransitionState;
    public State HomeComingState;
    public State HomeState;
    public State WarState;
    public State BuildState;
    public State PauseState;

    public Transform CameraFollowTarget;
    public Transform CameraFaceTarget;
    public Transform CameraSlotTarget;
    public Transform CameraDeathTarget;
    public Transform CameraHomeTarget;
    public Transform ShieldPosition;
    [SerializeField] SphereCollider _magnetCollider;
    float _magnetColliderRadius = 0;

    public PlayerCharacter Character;
    AudioSource _audioSource;
    public Cogs Cogs;
    public Damagable Damagable;
    Camera _camera;
    //Animator _animator;
    CharacterController _controller;
    float _speed = 10;
    public float Gravity = -15.0f;
    float _verticalVelocity = -2f;
    [SerializeField] LayerMask _groundLayers;
    float _groundCheckRadius = 0.3f;
    bool _grounded = false;
    float _groundedOffset = 0;
    float _fallTimeoutDelta = 0;
    public float FallTimeout = 0.15f;
    public float FlyingDistance = 20;
    public bool Jumping = false;
    public float JumpHeight = 10;
    [SerializeField] float _footStepDelay = 0.5f;
    float _footStepTimeout = 0;
    [SerializeField] AudioClip _landingAudioClip;
    [SerializeField] AudioClip[] _footstepAudioClips;
    [SerializeField] [Range(0, 1)] public float _footstepAudioVolume = 0.5f;
    Vector3 _horizontalMovement = Vector3.zero;
    Vector3 _verticalMovement = Vector3.zero;
    Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _magnetColliderRadius = _magnetCollider.radius;
        _fallTimeoutDelta = FallTimeout;
        _audioSource = GetComponent<AudioSource>();
        Cogs = GetComponent<Cogs>();
        Damagable = GetComponent<Damagable>();
        _controller = GetComponent<CharacterController>();
        //_animator = GetComponent<Animator>();
        _camera = Camera.main;
        TransitionState = new();
        HomeState = new();
        WarState = new(WarEnter, WarExit, WarUpdate);
        BuildState = new(BuildEnter, BuildExit);
        PauseState = new();
        HomeComingState = new(HomeComingEnter, update: HomeComingUpdate);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset,
            transform.position.z);
        Gizmos.DrawWireSphere(spherePosition, _groundCheckRadius);
    }

    void Start()
    {
        Character.InstallSlots();
        if (CameraManager.Instance == null) return;
        CameraManager.Instance.SetPlayer(this);
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine.Update();
    }

    void HomeComingEnter()
    {
        transform.position = Vector3.zero + new Vector3(0, FlyingDistance / 2);
        transform.rotation = Quaternion.identity;
    }

    void HomeComingUpdate()
    {
        GroundedCheck();
        GravityCheck();
        VerticalMovementCheck();
        Move();

        transform.RewritePosition(x:0, z:0);

        if (_grounded)
        {
            StateMachine.SetState(HomeState);
        }
    }

    void WarEnter()
    {
        if(Damagable.Shield != null)
        {
            Damagable.Shield.gameObject.SetActive(true);
        }
    }

    void WarUpdate()
    {
        Rotate();
        GroundedCheck();
        GravityCheck();
        HorizontalMovementCheck();
        VerticalMovementCheck();
        Move();
        Character.WalkAnimation(InputManager.Instance.Move.x, InputManager.Instance.Move.z);
        FootStepSound();
    }

    void WarExit()
    {
        if (Damagable.Shield != null)
        {
            Damagable.Shield.gameObject.SetActive(false);
        }
        Character.WalkAnimation(0, 0);
        Character.ResetTargets();
    }

    void BuildEnter()
    {
        _magnetCollider.radius = 1000;
        Character.SetTargetsToIdle();
    }

    void BuildExit()
    {
        _magnetCollider.radius = _magnetColliderRadius;
        Character.SetRigWeights(0);
    }

    public void OnHealthChange(float healthPercent)
    {
        if(GamePlayUI.Instance == null) return;
        GamePlayUI.Instance.UpdateHealthBar(healthPercent);
    }

    public void OnChangeTotalCogs(int totalCogs)
    {
        GamePlayUI.Instance.UpdateTotalCogs(totalCogs);
    }

    public void OnScanEnemies(List<GameObject> seenEnemies)
    {
        List<Enemy> enemies = seenEnemies.Select(enemy => enemy.GetComponent<Enemy>()).ToList();
        SetTargets(enemies);
    }

    public void Die()
    {
        GameManager.Instance.GameOver(success:false);
        Instantiate(GameManager.Instance.ExplosionPrefab, transform.position, Quaternion.identity);
        Cogs.Spill();
        Destroy(gameObject);
    }

    public void Rotate()
    {
        if (InputManager.Instance.Look == Vector2.zero) return;
        transform.rotation = Quaternion.Euler(0,
            transform.rotation.eulerAngles.y + InputManager.Instance.Look.x
            , 0);
    }

    void HorizontalMovementCheck()
    {
        if (InputManager.Instance.Move == Vector3.zero)
        {
            _horizontalMovement = Vector3.zero;
            return;
        }

        float inputDirection = Mathf.Atan2(InputManager.Instance.Move.x, InputManager.Instance.Move.z) * Mathf.Rad2Deg;
        Vector3 targetDirection = Quaternion.Euler(0, inputDirection, 0) * transform.forward;
        _horizontalMovement = _speed * Time.deltaTime * targetDirection;
    }

    void FootStepSound()
    {
        if (InputManager.Instance.Move == Vector3.zero)
        {
            _footStepTimeout = 0;
            return;
        }
        _footStepTimeout -= Time.deltaTime;
        if (_footStepTimeout <= 0)
        {
            PlayFootstepSound();
            _footStepTimeout += _footStepDelay;
        }
    }

    public void Move()
    {
        _controller.Move(_horizontalMovement + _verticalMovement);
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset,
            transform.position.z);
        bool checkGrounded = Physics.CheckSphere(spherePosition, _groundCheckRadius, _groundLayers,
            QueryTriggerInteraction.Ignore);
        if(!_grounded && checkGrounded)
        {
            // landed
            PlayLandSound();
        }
        _grounded = checkGrounded;

        Character.GroundedAnimation(_grounded);
    }

    void GravityCheck()
    {
        if (_grounded)
        {
            if (Jumping)
            {
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2 * Gravity);
                return;
            }

            _fallTimeoutDelta = FallTimeout;
            _verticalVelocity = -5f;
            Character.FreeFallAnimation(false);
        }
        else
        {
            Jumping = false;

            if(_fallTimeoutDelta > 0)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            } else
            {
                Character.FreeFallAnimation(true);
                _verticalVelocity += Gravity * 4 * Time.deltaTime;
            }
        }
    }

    void VerticalMovementCheck()
    {
        _verticalMovement = new Vector3(0, _verticalVelocity, 0) * Time.deltaTime;
    }

    public void SetTargets(List<Enemy> enemies) 
    {
        enemies = enemies.OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position)).ToList();
        Character.SetTargets(enemies);
    }

    void PlayFootstepSound()
    {
        if (_footstepAudioClips.Length == 0) return;
        var index = UnityEngine.Random.Range(0, _footstepAudioClips.Length);
        _audioSource.PlayOneShot(_footstepAudioClips[index]);
    }

    void PlayLandSound()
    {
        _audioSource.PlayOneShot(_landingAudioClip);
    }

    public void OnPressJump()
    {
        if (!StateMachine.IsState(WarState) || !_grounded) return;
        Jump();
    }

    public void Jump()
    {
        JumpAnimation(() => 
            Jumping = true
        );
    }

    public void Fly()
    {
        JumpAnimation(() =>
            transform.DOMoveY(transform.position.y + FlyingDistance, 1)
        );
    }

    public void JumpAnimation(Action jumpAction)
    {
        StartCoroutine(JumpCoroutine(jumpAction));
    }

    IEnumerator JumpCoroutine(Action jumpAction)
    {
        Character.JumpAnimation();
        yield return new WaitForSeconds(0.4f);
        jumpAction();
    }

    //public void OnJumpAnimCompleted()
    //{
    //    if (StateMachine.IsState(WarState))
    //    {
    //        Jumping = true;
    //        return;
    //    }

    //    if (StateMachine.IsState(TransitionState) || StateMachine.IsState(HomeState))
    //    {
    //        transform.DOMoveY(transform.position.y + FlyingDistance, 1);
    //    }
    //}

    public void ResetPlayer()
    {
        Character.InstallSlots();
        Damagable damagable = GetComponent<Damagable>();
        damagable.ResetHealth();
        Cogs cogs = GetComponent<Cogs>();
        cogs.TotalCogs = 0;
    }

    //public void JumpAnimation()
    //{
    //    _animator.SetTrigger("jump");
    //}

    //public void GroundedAnimation(bool isGrounded)
    //{
    //    _animator.SetBool("grounded", isGrounded);
    //}

    //public void FreeFallAnimation(bool isFreeFall)
    //{
    //    _animator.SetBool("free_fall", isFreeFall);
    //}

    //public void WalkAnimation(float x, float z)
    //{
    //    _animator.SetFloat("move_x", x);
    //    _animator.SetFloat("move_z", z);
    //}
}