using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public GunSlot[] Slots = new GunSlot[] { };
    Animator _animator;
    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void InstallSlots()
    {
        foreach (GunSlot gunSlotDefinition in Slots)
        {
            if (gunSlotDefinition.DefaultGun == null) continue;
            gunSlotDefinition.InstallGun(gunSlotDefinition.DefaultGun);
        }
    }

    public void SetRigWeights(float weight)
    {
        foreach (GunSlot slot in Slots)
        {
            slot.SetRigWeight(weight);
        }
    }

    public void ResetTargets()
    {
        foreach (GunSlot slot in Slots)
        {
            slot.SetTarget(null);
        }
    }

    public void SetTargetsToIdle()
    {
        foreach (GunSlot slot in Slots)
        {
            slot.SetTarget(null);
            slot.SetTargetToIdle();
        }
    }

    public void SetTargets(List<Enemy> enemies)
    {
        foreach (GunSlot slot in Slots)
        {
            slot.SetTarget(enemies.Count > 0 ? enemies[0] : null);
        }
    }

    public void JumpAnimation()
    {
        _animator.SetTrigger("jump");
    }

    public void GroundedAnimation(bool isGrounded)
    {
        _animator.SetBool("grounded", isGrounded);
    }

    public void FreeFallAnimation(bool isFreeFall)
    {
        _animator.SetBool("free_fall", isFreeFall);
    }

    public void WalkAnimation(float x, float z)
    {
        _animator.SetFloat("move_x", x);
        _animator.SetFloat("move_z", z);
    }
}
