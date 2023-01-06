using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanel : Panel
{
    public override void TurnOn()
    {
        base.TurnOn();
        Time.timeScale = 0;
    }
    
    public override void TurnOff()
    {
        base.TurnOff();
        Time.timeScale = 1;
    }

    public void GoHome()
    {
        Popup.Show("Go Home", "Are you sure?", "Yes", () =>
        {
            Time.timeScale = 1;
            SceneTransition.ChangeScene("MainMenu");
        }, "No");
    }

    public void Retry()
    {
        Popup.Show("Retry", "Are you sure?", "Yes", () =>
        {
            Time.timeScale = 1;
            SceneTransition.ChangeScene("Game");
        }, "No");
    }
}
