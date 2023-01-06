using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultPanel : Panel
{
    [SerializeField] private GameObject win, lose; 
    public static void Show(bool isWin, float goldReward=0)
    {
        Open<ResultPanel>(p =>
        {
            p.win.SetActive(isWin);
            p.lose.SetActive(!isWin);
        });
    }
    public void GoHome()
    {
        Time.timeScale = 1;
        SceneTransition.ChangeScene("MainMenu");
    }

    public void Retry()
    {
        Time.timeScale = 1;
        SceneTransition.ChangeScene("Game");
    }

    public void NextLevel()
    {
        Time.timeScale = 1;
        SceneTransition.ChangeScene("Game");
    }
}
