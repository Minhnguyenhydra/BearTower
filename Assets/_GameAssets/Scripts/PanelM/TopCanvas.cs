using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TopCanvas : MonoBehaviour
{
    private static TopCanvas _instance;
    public static TopCanvas Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TopCanvas>();
            }

            if (_instance == null)
            {
                _instance = Addressables.InstantiateAsync(nameof(TopCanvas)).WaitForCompletion()
                    .GetComponent<TopCanvas>();
            }

            return _instance;
        }
    }

    // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    // public static void OnLoad()
    // {
    //     Addressables.InstantiateAsync(nameof(TopCanvas));
    // }
    public BlockSceen blockSceen;
    public SceneTransition sceneTransition;
    private void Awake()
    {
        if (_instance==null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else DestroyImmediate(gameObject);
    }
}
