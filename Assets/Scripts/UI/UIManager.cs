using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UIManager : AbstractSingleton<UIManager>
{
    public BuildUI BuildUI;
    public GamePlayUI GamePlayUI;
    public GameObject HomeUI;

    public ModalView Modal;

    public StateMachine StateMachine = new();
    public State TransitionState;
    public State HomeState;
    public State GamePlayState;
    public State GameOverState;
    public State GamePauseState;
    public State BuildState;

    protected override void Awake()
    {
        base.Awake();

        BuildUI.gameObject.SetActive(false);
        GamePlayUI.gameObject.SetActive(false);
        HomeUI.gameObject.SetActive(false);

        TransitionState = new();
        HomeState = new(
                enter: () => { HomeUI.gameObject.SetActive(true); },
                exit: () => { HomeUI.gameObject.SetActive(false); }
            );
        GamePlayState = new(
                enter: () => { 
                    GamePlayUI.gameObject.SetActive(true);
                    GamePlayUI.Head.SetActive(true);
                    GamePlayUI.Controls.SetActive(true);
                },
                exit: () => { 
                    GamePlayUI.gameObject.SetActive(false);
                    GamePlayUI.Head.SetActive(false);
                    GamePlayUI.Controls.SetActive(false);
                }
            );
        GameOverState = new(
                exit: () => { Modal.Hide(); }
            );
        GamePauseState = new(
                enter: () => { Modal.Show(Modal.PauseModalView); },
                exit: () => { Modal.Hide(); }
            );
        BuildState = new(
                enter: () => { 
                    BuildUI.gameObject.SetActive(true);
                    GamePlayUI.gameObject.SetActive(true);
                    GamePlayUI.Head.SetActive(true);
                },
                exit: () => { 
                    BuildUI.gameObject.SetActive(false);
                    GamePlayUI.gameObject.SetActive(false);
                    GamePlayUI.Head.SetActive(false);
                }
            );
    }

    public void StartGameButton()
    {
        GameManager.Instance.FlyToStartGame();
    }

    public void CreditsButton()
    {
        Modal.Show(Modal.CreditsModalView);
    }

    public void SettingsButton()
    {
        Modal.Show(Modal.SettingsModalView);
    }

    public void RestartGameButton()
    {
        GameManager.Instance.RestartGame();
    }

    public void HomeButton()
    {
        GameManager.Instance.GoHome();
    }

    public void PauseButton()
    {
        GameManager.Instance.PauseGame();
    }

    public void ContinueButton()
    {
        GameManager.Instance.ContinueGame();
    }

    public void GameOver(bool success=true)
    {
        StateMachine.SetState(GameOverState);
        Modal.Show(success 
            ? Modal.SuccessModalView
            : Modal.FailureModalView
            );
    }
}
