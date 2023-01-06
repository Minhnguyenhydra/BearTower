using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayMgr : MonoBehaviour
{
    public static GamePlayMgr Instance;
    public List<TeamMgr> teams;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Instance = this;
    }

    private void Start()
    {
        foreach (var team in teams)
            team.Setup();
    }

    public void EndGame()
    {
        Time.timeScale = 0;
        var isWin = teams[0].castle.Hp > 0;
        ResultPanel.Show(isWin);
    }
}
