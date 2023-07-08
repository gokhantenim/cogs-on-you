using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public Action Enter = () => { };
    public Action Update = () => { };
    public Action Exit = () => { };

    public State(Action enter = null, Action exit = null, Action update = null)
    {
        if(enter != null) {
            Enter = enter;
        }
        if (update != null)
        {
            Update = update;
        }
        if (exit != null)
        {
            Exit = exit;
        }
    }
}
