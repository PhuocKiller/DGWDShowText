using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static readonly object _lock = new object();
    private static bool isApplicationQuit = false;
    private void Awake()
    {
        isApplicationQuit = false;
    }
    public static T Instance
    {
        get {
            lock (_lock)
            {
                if (instance == null)
                {
                    if (isApplicationQuit)
                    {
                        return null;
                    }

                    T[] listT = Resources.FindObjectsOfTypeAll<T>();
                    instance = listT != null && listT.Length > 0 ? listT[0] : null;
                    if(instance == null)
                    {
                        T gameObjectT = new GameObject("Singleton "+ typeof(T).Name).AddComponent<T>();
                        instance = gameObjectT;
                        return instance;
                    }
                    else
                    {
                        if (Application.isPlaying)
                        {
                            DontDestroyOnLoad(instance);
                        }
                        return instance;
                    }

                }
                else
                {
                    return instance;
                }
            }
        }
    }

    private void OnDestroy()
    {
        isApplicationQuit = true;
    }
}
