using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    public Camera Camera;

    public CinemachineVirtualCamera CameraFollowPlayer;
    public CinemachineVirtualCamera CameraFollowUI;
    public CinemachineVirtualCamera CameraFollowUISlot;
    public CinemachineVirtualCamera CameraFollowDeath;
    public CinemachineVirtualCamera CameraFollowHome;

    public StateMachine StateMachine = new();
    public State FollowPlayerState;
    public State FacePlayerState;
    public State FaceGunSlotState;
    public State DeathState;
    public State HomeState;

    void Awake()
    {
        Instance = this;
        Camera = Camera.main;

        FollowPlayerState = new(
            enter: () => { CameraFollowPlayer.gameObject.SetActive(true); },
            exit: () => { CameraFollowPlayer.gameObject.SetActive(false); }
        );

        FacePlayerState = new(
            enter: () => { CameraFollowUI.gameObject.SetActive(true); },
            exit: () => { CameraFollowUI.gameObject.SetActive(false); }
        );

        FaceGunSlotState = new(
            enter: () => { CameraFollowUISlot.gameObject.SetActive(true); },
            exit: () => { CameraFollowUISlot.gameObject.SetActive(false); }
        );

        DeathState= new(
            enter: () => { CameraFollowDeath.gameObject.SetActive(true); },
            exit: () => { CameraFollowDeath.gameObject.SetActive(false); }
        );

        HomeState = new(
            enter: () => { CameraFollowHome.gameObject.SetActive(true); },
            exit: () => { CameraFollowHome.gameObject.SetActive(false); }
        );

        CameraFollowPlayer.gameObject.SetActive(false);
        CameraFollowUI.gameObject.SetActive(false);
        CameraFollowUISlot.gameObject.SetActive(false);
        CameraFollowDeath.gameObject.SetActive(false);
        CameraFollowHome.gameObject.SetActive(false);
    }

    public void SetPlayer(PlayerController player)
    {
        CameraFollowPlayer.Follow = player.CameraFollowTarget;
        CameraFollowPlayer.LookAt = player.CameraFollowTarget;

        CameraFollowUI.Follow = player.CameraFaceTarget;
        CameraFollowUI.LookAt = player.CameraFaceTarget;

        CameraFollowUISlot.Follow = player.CameraSlotTarget;
        CameraFollowUISlot.LookAt = player.CameraSlotTarget;

        CameraFollowDeath.Follow = player.CameraDeathTarget;
        CameraFollowDeath.LookAt = player.CameraDeathTarget;

        CameraFollowHome.Follow = player.CameraHomeTarget;
        CameraFollowHome.LookAt = player.CameraHomeTarget;
    }

    public void FollowPlayer()
    {
        StateMachine.SetState(FollowPlayerState);
    }

    public void FacePlayer()
    {
        StateMachine.SetState(FacePlayerState);
    }

    public void FaceGunSlot(GunSlot slot)
    {
        //CameraFollowUISlot.gameObject.SetActive(true);
        CameraFollowUISlot.LookAt.position = slot.transform.position;
        StateMachine.SetState(FaceGunSlotState);
    }
}
