using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CoroutineStarter : MonoBehaviour
{
    private static CoroutineStarter _instance;

    public static CoroutineStarter Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("CoroutineStarter");
                _instance = go.AddComponent<CoroutineStarter>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public static void Start(IEnumerator coroutine)
    {
        Instance.StartCoroutine(coroutine);
    }
}