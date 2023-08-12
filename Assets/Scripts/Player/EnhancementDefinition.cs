using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnhancementDefinition : ScriptableObject
{
    public string Name;
    public int Cost = 0;
    public Sprite Image;

    public virtual void Buy(){}
}
