using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using DG.Tweening;

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
    public State WarState;
    public State BuildState;

    public GunSlot[] slots = new GunSlot[] { };
    public Transform CameraFollowTarget;
    public Transform CameraFaceTarget;
    public Transform CameraSlotTarget;
    [SerializeField] SphereCollider _magnetCollider;
    float _magnetColliderRadius = 0;

    public Cogs Cogs;
    Camera _camera;
    Animator _animator;
    CharacterController _controller;
    float _speed = 10;
    float _rotationY;
    public float Gravity = -15.0f;
    float _verticalVelocity = -2f;
    [SerializeField] LayerMask _groundLayers;
    float _groundCheckRadius = 0.3f;
    bool _grounded = false;
    float _groundedOffset = 0;
    float _fallTimeoutDelta = 0;
    public float FallTimeout = 0.15f;
    public float FlyingDistance = 20;
    [SerializeField] float _footStepDelay = 0.5f;
    float _footStepTimeout = 0;
    [SerializeField] AudioClip _landingAudioClip;
    [SerializeField] AudioClip[] _footstepAudioClips;
    [SerializeField] [Range(0, 1)] public float _footstepAudioVolume = 0.5f;

    void Awake()
    {
        _camera = Camera.main;
        WarState = new(WarEnter, WarExit, WarUpdate);
        BuildState = new(BuildEnter);
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
        _magnetColliderRadius = _magnetCollider.radius;
        CameraManager.Instance.SetPlayer(this);
        Cogs = GetComponent<Cogs>();
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        //_controller.detectCollisions = false;
        //Cursor.lockState = CursorLockMode.Locked;
        _rotationY = transform.rotation.eulerAngles.y;
        _fallTimeoutDelta = FallTimeout;
        InstallSlots();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Player Update");
        StateMachine.Update();
    }

    void WarEnter()
    {
        Debug.Log("Player War Enter");
        _magnetCollider.radius = _magnetColliderRadius;

        //Debug.Log("Player Position before");
        //Vector3 playerPosition = LevelManager.Instance.LoadedLevel.PlayerStartPosition != null 
        //    ? LevelManager.Instance.LoadedLevel.PlayerStartPosition 
        //    : Vector3.zero;
        //transform.position = playerPosition + new Vector3(0, 20);
        //Debug.Log("Player Position after");
    }

    void WarUpdate()
    {
        Rotate();
        GroundedCheck();
        GravityCheck();
        Move();
    }

    void WarExit()
    {
        _magnetCollider.radius = 1000;
        WalkAnimation(0, 0);
        foreach (GunSlot slot in slots)
        {
            slot.SetTarget(null);
        }
    }

    void BuildEnter()
    {
        foreach (GunSlot slot in slots)
        {
            slot.SetTargetToIdle();
            slot.SetTarget(null);
        }
    }

    public void OnHealthChange(float healthPercent)
    {
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
        GameManager.Instance.GameOver();
        Cogs.Spill();
        Destroy(gameObject);
    }

    void InstallSlots()
    {
        foreach(GunSlot gunSlotDefinition in slots)
        {
            if (gunSlotDefinition.DefaultGun == null) continue;
            gunSlotDefinition.InstallGun(gunSlotDefinition.DefaultGun);
        }
    }

    public void Rotate()
    {
        if (InputManager.Instance.look == Vector2.zero) return;

        _rotationY += InputManager.Instance.look.x;
        transform.rotation = Quaternion.Euler(0, _rotationY, 0);
    }

    Vector3 horizontalDirection()
    {
        if (InputManager.Instance.move == Vector3.zero)
        {
            _footStepTimeout = 0;
            return Vector3.zero;
        }
        _footStepTimeout -= Time.deltaTime;
        if(_footStepTimeout <= 0)
        {
            PlayFootstepSound();
            _footStepTimeout += _footStepDelay;
        }

        float inputDirection = Mathf.Atan2(InputManager.Instance.move.x, InputManager.Instance.move.z) * Mathf.Rad2Deg;
        Vector3 targetDirection = Quaternion.Euler(0, inputDirection, 0) * transform.forward;
        return targetDirection;
    }

    public void WalkAnimation(float x, float z)
    {
        _animator.SetFloat("move_x", x);
        _animator.SetFloat("move_z", z);
    }

    public void Move()
    {
        WalkAnimation(InputManager.Instance.move.x, InputManager.Instance.move.z);

        Vector3 verticalDirection = new Vector3(0, _verticalVelocity, 0) * Time.deltaTime;
        _controller.Move((_speed * Time.deltaTime * horizontalDirection()) + verticalDirection);
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset,
            transform.position.z);
        _grounded = Physics.CheckSphere(spherePosition, _groundCheckRadius, _groundLayers,
            QueryTriggerInteraction.Ignore);

        _animator.SetBool("grounded", _grounded);
    }

    void GravityCheck()
    {
        if (_grounded)
        {
            _fallTimeoutDelta = FallTimeout;
            _verticalVelocity = -5f;
            _animator.SetBool("free_fall", false);
        }
        else
        {
            if(_fallTimeoutDelta > 0)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            } else
            {
                _animator.SetBool("free_fall", true);
                _verticalVelocity += Gravity * 4 * Time.deltaTime;
            }
        }

    }

    public void SetTargets(List<Enemy> enemies) 
    {
        enemies = enemies.OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position)).ToList();
        foreach (GunSlot slot in slots)
        {
            slot.SetTarget(enemies.Count > 0 ? enemies[0] : null);
        }
    }

    void PlayFootstepSound()
    {
        if (_footstepAudioClips.Length == 0) return;
        var index = UnityEngine.Random.Range(0, _footstepAudioClips.Length);
        AudioSource.PlayClipAtPoint(_footstepAudioClips[index], _camera.transform.position, _footstepAudioVolume);
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        AudioSource.PlayClipAtPoint(_landingAudioClip, _camera.transform.position, _footstepAudioVolume);
    }

    public void OnJumpCompleted()
    {
        transform.DOMoveY(transform.position.y + FlyingDistance, 1);
    }

    internal void Fly()
    {
        _animator.SetTrigger("jump");
    }
}