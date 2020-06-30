using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T instance;

    virtual protected void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
        }

        instance = this as T;
        DontDestroyOnLoad(this);
    }
}
