
using UnityEngine;

public class MainMenu : Panel
{
    public override void BackClick()
    {
        Popup.Show("Quit?", "Are you sure!", "Yes", Application.Quit, "No");
    }
}