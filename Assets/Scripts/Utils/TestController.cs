using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GamePlayUI.Instance.gameObject.SetActive(true);
        PlayerController.Instance.StateMachine.SetState(PlayerController.Instance.WarState);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
