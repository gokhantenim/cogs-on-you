using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public BuildUI BuildUI;
    public GamePlayUI GamePlayUI;
    public GameObject HomeUI;

    public StateMachine StateMachine = new();
    public State HomeState;
    public State GamePlayState;
    public State BuildState;

    void Awake()
    {
        Instance = this;
        BuildUI.gameObject.SetActive(false);
        GamePlayUI.gameObject.SetActive(false);
        HomeUI.gameObject.SetActive(false);

        HomeState = new(
                enter: () => { HomeUI.gameObject.SetActive(true); },
                exit: () => { HomeUI.gameObject.SetActive(false); }
            );
        GamePlayState = new(GamePlayEnter, GamePlayExit);
        BuildState = new(BuildEnter, BuildExit);
    }

    #region states
    void GamePlayEnter()
    {
        GamePlayUI.gameObject.SetActive(true);
    }
    void GamePlayExit()
    {
        GamePlayUI.gameObject.SetActive(false);
    }
    void BuildEnter()
    {
        BuildUI.gameObject.SetActive(true);
        BuildUI.StateMachine.SetState(BuildUI.PlayerBuildState);
    }
    void BuildExit()
    {
        BuildUI.gameObject.SetActive(false);
    }
    #endregion

    public async void StartGameButton()
    {
        await Task.Delay(100);
        GameManager.Instance.StartGame();
    }
}
