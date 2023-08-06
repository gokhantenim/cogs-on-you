using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModalView : MonoBehaviour
{
    public GameObject SuccessModalView;
    public GameObject FailureModalView;
    public GameObject PauseModalView;
    public GameObject SettingsModalView;

    GameObject lastShowedView;

    public void Show(GameObject modalView)
    {
        HideLastView();
        lastShowedView = modalView;
        modalView.SetActive(true);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        HideLastView();
        gameObject.SetActive(false);
    }

    void HideLastView()
    {
        if (lastShowedView == null) return;
        lastShowedView.SetActive(false);
    }
}
