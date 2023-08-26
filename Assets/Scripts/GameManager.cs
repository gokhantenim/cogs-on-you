using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] GameObject _homeScene;
    public List<GunDefinition> Guns;
    public List<EnhancementDefinition> Enhancements;
    public List<LevelDefinition> Levels;
    public GameObject CogPrefab;
    public GameObject PlayerPrefab;
    public GameObject ExplosionPrefab;
    public int CurrentLevelIndex = 0;
    bool _currentIsLastLevel => CurrentLevelIndex+1 >= Levels.Count;
    StateMachine _stateMachine = new();
    State _transitionState;
    State _homeState;
    State _warState;
    State _buildState;
    State _pauseState;
    State _gameOverState;

    State _lastState; // for backup current state when getting pause


    PlayerController _player => PlayerController.Instance;

    void Awake()
    {
        Instance = this;
        _transitionState = new(TransitionEnter);
        _homeState = new(HomeEnter, HomeExit);
        _warState = new(WarEnter);
        _buildState = new(BuildEnter);
        _gameOverState = new();
        _pauseState = new(PauseEnter, PauseExit);
    }

    void Start()
    {
#if UNITY_EDITOR
        if (LevelEditorWindow.IsEditScene())
        {
            StartLevel();
            return;
        }
#endif
        _stateMachine.SetState(_homeState);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void TransitionEnter()
    {
        PlayerController.Instance.StateMachine.SetState(PlayerController.Instance.TransitionState);
        UIManager.Instance.StateMachine.SetState(UIManager.Instance.TransitionState);
    }

    void HomeEnter()
    {
        UIManager.Instance.StateMachine.SetState(UIManager.Instance.HomeState);
        PlayerController.Instance.StateMachine.SetState(PlayerController.Instance.HomeComingState);
        CameraManager.Instance.StateMachine.SetState(CameraManager.Instance.HomeState);
        _homeScene.SetActive(true);
    }

    void HomeExit()
    {
        _homeScene.SetActive(false);
    }

    void WarEnter()
    {
        UIManager.Instance.StateMachine.SetState(UIManager.Instance.GamePlayState);
        CameraManager.Instance.FollowPlayer();
        if (_player == null) return;
        _player.StateMachine.SetState(_player.WarState);
    }

    IEnumerator GoToBuildFromWar()
    {
        // 1 - fly
        _player.JumpAnimation();
        _stateMachine.SetState(_transitionState);
        yield return new WaitForSeconds(1);
        // 2 - face cam
        CameraManager.Instance.FacePlayer();
        yield return new WaitForSeconds(1);
        // 3 - set build state
        _stateMachine.SetState(_buildState);
        yield return null;
    }

    void BuildEnter()
    {
        _player.StateMachine.SetState(_player.BuildState);
        UIManager.Instance.StateMachine.SetState(UIManager.Instance.BuildState);
    }

    void PauseEnter()
    {
        UIManager.Instance.StateMachine.SetState(UIManager.Instance.GamePauseState);
        _player.StateMachine.SetState(_player.PauseState);
        Time.timeScale = 0;
    }

    void PauseExit()
    {
        UIManager.Instance.Modal.Hide();
        Time.timeScale = 1;
    }
    public void PauseGame()
    {
        _lastState = _stateMachine.CurrentState;
        _stateMachine.SetState(_pauseState);
    }
    public void ContinueGame()
    {
        _stateMachine.SetState(_lastState);
    }

    public void InstallGun(GunSlot slot, GunDefinition gun)
    {
        Cogs cogs = PlayerController.Instance.GetComponent<Cogs>();
        int cost = gun.Levels[0].Cost;
        if (cogs.TotalCogs < cost)
        {
            UIManager.Instance.Modal.Alert("Insufficient amount of cogs");
            return;
        }
        slot.InstallGun(gun);
        _player.Cogs.Spend(cost);
        SoundManager.Instance.PlayBuildSound();
    }

    public void UpgradeGun(GunSlot slot)
    {
        if (slot.CurrentGun.Definition.Levels.Length <= slot.CurrentGun.Level + 1) return;

        Cogs cogs = PlayerController.Instance.GetComponent<Cogs>();
        int cost = slot.CurrentGun.Definition.Levels[slot.CurrentGun.Level+1].Cost;
        if (cogs.TotalCogs < cost)
        {
            UIManager.Instance.Modal.Alert("Insufficient amount of cogs");
            return;
        }
        bool upgraded = slot.UpgradeGun();
        if (!upgraded) return;
        _player.Cogs.Spend(cost);
        SoundManager.Instance.PlayBuildSound();
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
        if(_currentIsLastLevel)
        {
            GameOver(true);
            return;
        }
        StartCoroutine(GoToBuildFromWar());
    }

    public void GoNextLevel()
    {
        if (_currentIsLastLevel) return;
        StartLevel(CurrentLevelIndex+1);
    }

    public void StartGame()
    {
        ResetPlayer();
        GamePlayUI.Instance.ResetValues();
        Cogs.ClearAllCogs();
        StartLevel();
    }

    void ResetPlayer()
    {
        if (_player == null)
        {
            Instantiate(PlayerPrefab);
        }
        else
        {
            _player.ResetPlayer();
        }
    }

    public async void FlyToStartGame()
    {
        UIManager.Instance.StateMachine.SetState(UIManager.Instance.TransitionState);
        PlayerController.Instance.JumpAnimation();
        await Task.Delay(1300);// wait for the flying
        StartGame();
    }

    public void StartLevel(int LevelIndex=0)
    {
        CurrentLevelIndex = LevelIndex;
        GamePlayUI.Instance.SetLevelNo(CurrentLevelIndex + 1, Levels.Count);
        LevelManager.Instance.LoadLevel(CurrentLevel());
        _stateMachine.SetState(_warState);
    }

    public void GoHome()
    {
        StopAllCoroutines();
        ResetPlayer();
        _stateMachine.SetState(_homeState);
        Cogs.ClearAllCogs();
        LevelManager.Instance.ClearLevel();
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

    public void GameOver(bool success)
    {
        _stateMachine.SetState(_gameOverState);
        UIManager.Instance.GameOver(success);
        if (success) return;
        CameraManager.Instance.StateMachine.SetState(CameraManager.Instance.DeathState);
    }
}
