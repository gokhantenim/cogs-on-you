using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StateMachine
{
    //public Dictionary<Enum, State> States = new Dictionary<Enum, State>();
    public State CurrentState;
    //Enum _stateKey;

    //public void AddState(Enum key, State state)
    //{
    //    States.Add(key, state);
    //}

    ////public void SetState(Enum key)
    ////{
    ////    if (IsState(key)) return;
    ////    if(!States.ContainsKey(key)) return;

    ////    //Debug.Log(key.ToString());
    ////    if(State != null)
    ////    {
    ////        State.Exit();
    ////    }
    ////    _stateKey = key;
    ////    State = States[key];
    ////    State.Enter();
    ////}

    //public bool IsState(Enum key)
    //{
    //    try
    //    {
    //        return _stateKey.Equals(key);
    //    }catch(NullReferenceException)
    //    {
    //        return false;
    //    }
    //}

    public void SetState(State state)
    {
        if (state == null || IsState(state)) return;
        if(CurrentState != null)
        {
            CurrentState.Exit();
        }
        CurrentState = state;
        CurrentState.Enter();
    }

    public bool IsState(State state)
    {
        if (state == null || CurrentState == null) return false;
        if (state.Equals(CurrentState)) return true;
        return false;
    }

    public void Update()
    {
        try
        {
            CurrentState.Update();
        }
        catch (NullReferenceException){}
    }
}
