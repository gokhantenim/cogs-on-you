using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModalView : MonoBehaviour
{
    [SerializeField] GameObject _modalsContainer;
    public GameObject SuccessModalView;
    public GameObject FailureModalView;
    public GameObject PauseModalView;
    public GameObject SettingsModalView;
    public GameObject CreditsModalView;
    [SerializeField] GameObject _alertModalView;
    [SerializeField] TextMeshProUGUI _alertMessageText;

    GameObject lastShowedView;

    public void Show(GameObject modalView)
    {
        HideLastView();
        lastShowedView = modalView;
        modalView.SetActive(true);
        _modalsContainer.SetActive(true);
    }

    public void Hide()
    {
        HideLastView();
        _modalsContainer.SetActive(false);
    }

    void HideLastView()
    {
        if (lastShowedView == null) return;
        lastShowedView.SetActive(false);
    }

    public void Alert(string message)
    {
        _alertMessageText.text = message;
        _alertModalView.SetActive(true);
        SoundManager.Instance.PlayAlertSound();
    }

    public void AlertOkButton()
    {
        _alertModalView.SetActive(false);
    }
}
