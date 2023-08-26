using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildUI : MonoBehaviour
{
    public StateMachine StateMachine = new();
    public State DisabledState;
    public State PlayerBuildState;
    public State GunSlotBuildState;

    public VisualTreeAsset slotButtonTemplate;
    public VisualTreeAsset gunCardTemplate;
    public VisualTreeAsset enhancementItemTemplate;

    UIDocument document;
    public static BuildUI Instance;

    GunSlotsUIController _slotsUI;
    GunSlotUIController _selectedSlotUI;
    EnhancementsListController _enhancementsUI;

    VisualElement _selectedGunOperations;

    Button _doneButton;

    public GunSlot SelectedSlot;
    GunDefinition _selectedGun;

    Camera _camera;

    void Awake()
    {
        Instance = this;
        DisabledState = new();
        PlayerBuildState = new(PlayerBuildEnter, PlayerBuildExit, PlayerBuildUpdate);
        GunSlotBuildState = new(GunSlotBuildEnter, GunSlotBuildExit);

        _camera = Camera.main;
        document = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        _slotsUI = new GunSlotsUIController(document, _camera);
        _selectedSlotUI = new GunSlotUIController(document);
        _enhancementsUI = new EnhancementsListController(document);

        _doneButton = document.rootVisualElement.Q<Button>("DoneButton");
        _doneButton.clicked += DoneButton;

        _selectedSlotUI.Show(false);

        StateMachine.SetState(PlayerBuildState);
    }

    private void OnDisable()
    {
        StateMachine.SetState(DisabledState);
    }

    private void Update()
    {
        StateMachine.Update();
    }

    async void DoneButton()
    {
        await Task.Delay(100);
        GameManager.Instance.GoNextLevel();
        SoundManager.Instance.PlayButtonSound();
    }

    void PlayerBuildEnter()
    {
        _slotsUI.CreateSlotButtons();
        _slotsUI.Show(true);
        _enhancementsUI.Show(true);
        CameraManager.Instance.FacePlayer();
    }

    void PlayerBuildExit()
    {
        _slotsUI.Show(false);
        _enhancementsUI.Show(false);
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
        if (SelectedSlot == null) return;
        SelectedSlot.Back2Normal();
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
        StateMachine.SetState(PlayerBuildState);
        SoundManager.Instance.PlayButtonSound();
    }

    public void InstallGun()
    {
        GameManager.Instance.InstallGun(SelectedSlot, _selectedGun);
        _selectedSlotUI = new GunSlotUIController(document);
        _selectedSlotUI.SetSelectedGun(new GunSelectionItem(_selectedGun, SelectedSlot.CurrentGun));
    }

    public void UpgradeGun()
    {
        GameManager.Instance.UpgradeGun(SelectedSlot);
        _selectedSlotUI = new GunSlotUIController(document);
        _selectedSlotUI.SetSelectedGun(new GunSelectionItem(_selectedGun, SelectedSlot.CurrentGun));
    }
}
