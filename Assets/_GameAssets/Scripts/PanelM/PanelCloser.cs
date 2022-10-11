using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelCloser : MonoBehaviour
{
    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        for (var i = transform.childCount-1; i >=0; i--)
        {
            var panel = transform.GetChild(i).GetComponent<Panel>();
            if (panel == null || !panel.gameObject.activeSelf || Panel.PoolPanels.Contains(panel)) continue;
            if (panel is IBackClickHandle backClickHandle) backClickHandle.BackClick(); else panel.TurnOff();
            break;
        }
    }
}
