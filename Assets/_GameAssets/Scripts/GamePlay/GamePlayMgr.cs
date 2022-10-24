using System;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayMgr : MonoBehaviour
{
    public List<TeamMgr> teams;
    private void Awake()
    {
        foreach (var team in teams)
            team.Setup();
        Application.targetFrameRate = 60;
    }
}
