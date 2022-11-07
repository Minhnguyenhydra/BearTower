using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StartupPanel : Panel
{
    [SerializeField] private Slider slLoading;
    private void Start()
    {
        slLoading.DOValue(1f, 2f).From(0).OnComplete(() =>
        {
            SceneTransition.ChangeScene("MainMenu");
        });
    }
}
