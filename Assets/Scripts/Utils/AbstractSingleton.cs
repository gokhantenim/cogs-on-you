using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSingleton<T> : MonoBehaviour where T : Component
{
    static T s_Instance;

    /// <summary>
    /// static Singleton instance
    /// </summary>
    public static T Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType<T>(true);
            }

            return s_Instance;
        }
        set { s_Instance = value; }
    }

    protected virtual void Awake()
    {
        if (s_Instance == null || s_Instance.Equals(this))
        {
            s_Instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}