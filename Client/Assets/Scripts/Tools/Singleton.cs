using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_Instance;

    public static T Instance
    {
        get => m_Instance;
    }

    protected virtual void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static bool IsInitialized()
    {
        return m_Instance != null;
    }

    protected virtual void OnDestroy()
    {
        m_Instance = null;
    }
}