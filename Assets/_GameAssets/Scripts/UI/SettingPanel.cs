using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPanel : Panel
{
    public void RateClick()
    {
        Application.OpenURL("https://google.com");
    }

    public void QuitClick()
    {
        Popup.Show("Quit", "Are you sure?", "Yes", Application.Quit, "No");
    }
}
