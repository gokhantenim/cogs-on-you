using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildUI : MonoBehaviour
{
    public StateMachine StateMachine = new();
    public State PlayerBuildState;
    public State GunSlotBuildState;

    public VisualTreeAsset slotButtonTemplate;
    public VisualTreeAsset gunCardTemplate;

    UIDocument document;
    public static BuildUI Instance;

    GunSlotsUIController _slotsUI;
    GunSlotUIController _selectedSlotUI;

    VisualElement _selectedGunOperations;

    Button _doneButton;

    public GunSlot SelectedSlot;
    GunDefinition _selectedGun;

    Camera _camera;

    void Awake()
    {
        Instance = this;
        PlayerBuildState = new(PlayerBuildEnter, PlayerBuildExit, PlayerBuildUpdate);
        GunSlotBuildState = new(GunSlotBuildEnter, GunSlotBuildExit);
        _camera = Camera.main;
        document = GetComponent<UIDocument>();

        _slotsUI = new GunSlotsUIController(document, _camera);
        _selectedSlotUI = new GunSlotUIController(document);

        _doneButton = document.rootVisualElement.Q<Button>("DoneButton");

        _selectedSlotUI.Show(false);

    }

    //void OnDisable()
    //{
    //    Button backButton = document.rootVisualElement.Q<Button>("BackButton");
    //    backButton.clicked -= BackButton;
    //}

    private void Update()
    {
        StateMachine.Update();
    }

    void PlayerBuildEnter()
    {
        _slotsUI.CreateSlotButtons();
        _slotsUI.Show(true);
        CameraManager.Instance.FacePlayer();
    }

    void PlayerBuildExit()
    {
        _slotsUI.Show(false);
    }

    void PlayerBuildUpdate()
    {
        _slotsUI.Update();
    }

    void GunSlotBuildEnter()
    {
        CameraManager.Instance.FaceGunSlot(SelectedSlot);
        _selectedSlotUI.Show(true);
        _selectedSlotUI.SetCurrentGun(SelectedSlot.CurrentGun);
        SelectGun(SelectedSlot.CurrentGun != null ? SelectedSlot.CurrentGun.Definition : null);
    }

    void GunSlotBuildExit()
    {
        _selectedSlotUI.Show(false);
    }
    
    public void SelectSlot(GunSlot slot)
    {
        SelectedSlot = slot;
        StateMachine.SetState(GunSlotBuildState);
    }

    public void SelectGun(GunDefinition gun)
    {
        _selectedGun = gun;
        if(gun == null)
        {
            _selectedSlotUI.SetSelectedGun(null);
            return;
        }
        _selectedSlotUI.SetSelectedGun(new GunSelectionItem(gun, SelectedSlot.CurrentGun));
        SelectedSlot.InstantiateGun(gun);
    }

    public void BackFromSlot()
    {
        SelectedSlot.Back2Normal();
        StateMachine.SetState(PlayerBuildState);
    }

    public void InstallGun()
    {
        GameManager.Instance.InstallGun(SelectedSlot, _selectedGun);
        BackFromSlot();
    }

    public void UpgradeGun()
    {
        GameManager.Instance.UpgradeGun(SelectedSlot);
        BackFromSlot();
    }
}
