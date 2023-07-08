using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

[RequireComponent(typeof(Scanner))]
[RequireComponent(typeof(Cogs))]
[RequireComponent(typeof(Damagable))]
public class PlayerController : MonoBehaviour
{
    public StateMachine StateMachine = new();
    public State WarState;
    public State BuildState;

    public GunSlot[] slots = new GunSlot[] { };
    public Transform CameraFollowTarget;
    public Transform CameraFaceTarget;
    public Transform CameraSlotTarget;

    public Cogs Cogs;
    Animator _animator;
    CharacterController _controller;
    float _speed = 10;
    float _rotationY;
    float _verticalVelocity = -0.35f;
    
    //float _scanDistance = 75;
    //float _scanAngle = 45;
    //Collider[] seenColliders = new Collider[100];
    //List<Enemy> seenEnemies = new List<Enemy>();
    //int seenCount = 0;
    //[SerializeField] LayerMask scanLayers;
    //[SerializeField] LayerMask checkLayers;
    //float _scanInterval = 1 / 10;
    //float _scanTimer = 0;

    void OnDestroy()
    {
        GameManager.Instance.Player = null;
    }

    void Awake()
    {
        WarState = new(update: WarUpdate, exit: WarExit);
        BuildState = new(BuildEnter);
    }

    void Start()
    {
        GameManager.Instance.SetPlayer(this);
        Cogs = GetComponent<Cogs>();
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        //_controller.detectCollisions = false;
        //Cursor.lockState = CursorLockMode.Locked;
        _rotationY = transform.rotation.eulerAngles.y;

        InstallSlots();

        //StateMachine.SetState(States.WAR);
        StateMachine.SetState(WarState);
    }

    // Update is called once per frame
    void Update()
    {
        //_state.Update(this);
        StateMachine.Update();
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = seenEnemies.Count > 0 ? Color.red : Color.green;
    //    Gizmos.DrawWireSphere(transform.position, _scanDistance);
    //    Gizmos.DrawLine(transform.position, transform.position + (Quaternion.Euler(0, _scanAngle, 0) * transform.forward * _scanDistance));
    //    Gizmos.DrawLine(transform.position, transform.position + (Quaternion.Euler(0, -_scanAngle, 0) * transform.forward * _scanDistance));
    //    Gizmos.color = Color.white;
    //    foreach (Enemy enemy in seenEnemies)
    //    {
    //        Gizmos.DrawLine(transform.position + new Vector3(0, 4f, 0), enemy.transform.position);
    //    }
    //}

    void WarUpdate()
    {
        Rotate();
        Move();

        //_scanTimer -= Time.deltaTime;
        //if (_scanTimer < 0)
        //{
        //    _scanTimer += _scanInterval;
        //    Scan();
        //}
    }

    void WarExit()
    {
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
        if (InputManager.Instance.move == Vector3.zero) return Vector3.zero;

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

        Vector3 verticalDirection = new Vector3(0, _verticalVelocity, 0);
        _controller.Move((_speed * Time.deltaTime * horizontalDirection()) + verticalDirection);
    }

    public void SetTargets(List<Enemy> enemies) 
    {
        enemies = enemies.OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position)).ToList();
        foreach (GunSlot slot in slots)
        {
            slot.SetTarget(enemies.Count > 0 ? enemies[0] : null);
        }
    }
    //public void Scan()
    //{
    //    seenCount = Physics.OverlapSphereNonAlloc(transform.position, _scanDistance, seenColliders, scanLayers);
    //    seenEnemies.Clear();
    //    for(int i = 0; i < seenCount; i++)
    //    {
    //        Collider seenCollider = seenColliders[i];
    //        if (seenCollider.tag == "Enemy" && InSight(seenCollider.gameObject))
    //        {
    //            seenEnemies.Add(seenCollider.GetComponent<Enemy>());
    //        }
    //    }
    //    SetTargets(seenEnemies);
    //}

    //bool InSight(GameObject seenObject)
    //{
    //    Vector3 direction = seenObject.transform.position - transform.position;
    //    float angle = Vector3.Angle(direction, transform.forward);
    //    if (angle > _scanAngle) return false;

    //    RaycastHit hit;
    //    if(Physics.Linecast(transform.position + new Vector3(0, 4f, 0), seenObject.transform.position, out hit, checkLayers)
    //        && hit.collider.gameObject.Equals(seenObject))
    //    {
    //        return true;
    //    }
    //    return false;
    //}
}
