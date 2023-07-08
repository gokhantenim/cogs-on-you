using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public BuildUI BuildUI;
    public GamePlayUI GamePlayUI;

    public StateMachine StateMachine = new();
    public State GamePlayState;
    public State BuildState;

    void Awake()
    {
        Instance = this;
        BuildUI.gameObject.SetActive(false);
        GamePlayUI.gameObject.SetActive(false);

        GamePlayState = new(GamePlayEnter, GamePlayExit);
        BuildState = new(BuildEnter, BuildExit);
    }

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
}
