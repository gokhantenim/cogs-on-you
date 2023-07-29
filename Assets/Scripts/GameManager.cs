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
    public List<LevelDefinition> Levels;
    public int CurrentLevelIndex = 0;
    bool CurrentIsLastLevel => CurrentLevelIndex+1 >= Levels.Count;
    StateMachine _stateMachine = new();
    State _homeState;
    State _warState;
    State _buildState;

    PlayerController _player => PlayerController.Instance;

    void Awake()
    {
        Instance = this;
        _homeState = new(HomeEnter, HomeExit);
        _warState = new(WarEnter, WarExit);
        _buildState = new(BuildEnter, BuildExit);
    }

    void Start()
    {
#if UNITY_EDITOR
        if (LevelEditorWindow.IsEditScene())
        {
            _stateMachine.SetState(_warState);
            return;
        }
#endif
        _stateMachine.SetState(_homeState);
    }

    // Update is called once per frame
    void Update()
    {
        //_stateMachine.Update();
    }

    void HomeEnter()
    {
        UIManager.Instance.HomeUI.SetActive(true);
    }

    void HomeExit()
    {
        UIManager.Instance.HomeUI.SetActive(false);
    }

    void WarEnter()
    {
        Debug.Log("Game War Enter");
        LoadLevel();
        UIManager.Instance.GamePlayUI.gameObject.SetActive(true);
        CameraManager.Instance.FollowPlayer();
        _player.StateMachine.SetState(_player.WarState);
    }

    void WarExit()
    {
        UIManager.Instance.GamePlayUI.gameObject.SetActive(false);
    }

    IEnumerator BuildEnterSequence()
    {
        // 1 -  set player state build and fly
        _player.StateMachine.SetState(_player.BuildState);
        _player.Fly();
        yield return new WaitForSeconds(1);
        // 2 - face cam
        CameraManager.Instance.FacePlayer();
        // 3 - show ui
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.StateMachine.SetState(UIManager.Instance.BuildState);
        yield return null;
    }

    void BuildEnter()
    {
        StartCoroutine(BuildEnterSequence());
        //CameraManager.Instance.FacePlayer();
        //UIManager.Instance.StateMachine.SetState(UIManager.Instance.BuildState);
        //_player.StateMachine.SetState(_player.BuildState);
    }

    void BuildExit()
    {
        UIManager.Instance.BuildUI.gameObject.SetActive(false);
    }

    public void InstallGun(GunSlot slot, GunDefinition gun)
    {
        slot.InstallGun(gun);
        _player.Cogs.Spend(gun.Levels[0].Cost);
    }

    public void UpgradeGun(GunSlot slot)
    {
        bool upgraded = slot.UpgradeGun();
        if (!upgraded) return;
        _player.Cogs.Spend(slot.CurrentGun.Definition.Levels[slot.CurrentGun.Level].Cost);
    }

    public void StartGame()
    {
        CurrentLevelIndex = 0;
        _stateMachine.SetState(_warState);
    }

    public void LevelCompleted()
    {
#if UNITY_EDITOR
        if (LevelEditorWindow.IsEditScene())
        {
            Debug.Log("Level Completed");
            return;
        }
#endif
        if(CurrentIsLastLevel)
        {
            Debug.Log("Game Completed");
            return;
        }
        _stateMachine.SetState(_buildState);
    }

    public void GoNextLevel()
    {
        if (CurrentIsLastLevel) return;
        CurrentLevelIndex++;
        _stateMachine.SetState(_warState);
    }

    LevelDefinition CurrentLevel()
    {
#if UNITY_EDITOR
        if (LevelEditorWindow.IsEditScene())
        {
            LevelDefinition level = LevelEditorWindow.SelectedLevel != null
                ? LevelEditorWindow.SelectedLevel
                : Levels[0];
            return level;
        }
#endif
        return Levels[CurrentLevelIndex];
    }

    public void LoadLevel()
    {
        LevelManager.Instance.LoadLevel(CurrentLevel());
    }

    public void GameOver()
    {
        Debug.Log("game over");
    }
}
