using System;
using UnityEngine;

public class SheetPanelContainer:MonoBehaviour
{
    public Panel currentPanel;
    public void OpenPanelSheet(string panelKey)
    {
        currentPanel?.TurnOff();
        Panel.Open(panelKey, panel => currentPanel = panel, transform);
    }
}
