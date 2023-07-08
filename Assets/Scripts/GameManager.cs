using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<GunDefinition> Guns;
    public GameObject CogPrefab;
    StateMachine _stateMachine = new();
    State _warState;
    State _buildState;

    public int TotalCogs = 0;

    public PlayerController Player;

    void Awake()
    {
        Instance = this;
        _warState = new(WarEnter, WarExit);
        _buildState = new(BuildEnter, BuildExit);
    }

    void Start()
    {
        _stateMachine.SetState(_warState);

        //await Task.Delay(2000);
        //_stateMachine.SetState(_buildState);
    }

    // Update is called once per frame
    void Update()
    {
        //_stateMachine.Update();
    }

    void WarEnter()
    {
        UIManager.Instance.GamePlayUI.gameObject.SetActive(true);
        CameraManager.Instance.FollowPlayer();
        Player.StateMachine.SetState(Player.WarState);
    }

    void WarExit()
    {
        UIManager.Instance.GamePlayUI.gameObject.SetActive(false);
    }

    void BuildEnter()
    {
        CameraManager.Instance.FacePlayer();
        UIManager.Instance.StateMachine.SetState(UIManager.Instance.BuildState);
        Player.StateMachine.SetState(Player.BuildState);
    }

    void BuildExit()
    {
        UIManager.Instance.BuildUI.gameObject.SetActive(false);
    }

    public void AddCogs(int amount)
    {
        TotalCogs += amount;
        GamePlayUI.Instance.UpdateTotalCogs(TotalCogs);
    }

    public void InstallGun(GunSlot slot, GunDefinition gun)
    {
        slot.InstallGun(gun);
        Player.Cogs.Spend(gun.Levels[0].Cost);
    }

    public void UpgradeGun(GunSlot slot)
    {
        bool upgraded = slot.UpgradeGun();
        if (!upgraded) return;
        Player.Cogs.Spend(slot.CurrentGun.Definition.Levels[slot.CurrentGun.Level].Cost);
    }

    public void SetPlayer(PlayerController player)
    {
        Player = player;
        CameraManager.Instance.SetPlayer(player);
    }

    public void GameOver()
    {
        Debug.Log("game over");
    }
}
