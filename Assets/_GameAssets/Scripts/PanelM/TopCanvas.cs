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
            if (!_instance)
                _instance = Addressables.InstantiateAsync(nameof(TopCanvas)).WaitForCompletion()
                    .GetComponent<TopCanvas>();
            return _instance;
        }
    }
    public BlockSceen blockSceen;
    public SceneTransition sceneTransition;
    private void Awake()
    {
        if (!_instance)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else DestroyImmediate(gameObject);
    }
}
