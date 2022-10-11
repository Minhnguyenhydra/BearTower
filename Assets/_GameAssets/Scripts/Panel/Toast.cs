using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Toast : Panel
{
    [SerializeField] private TMP_Text txtMessage;
    private Sequence _coolDown;
    public static void Show(string message,float delayTime=1f)
    {
        Open<Toast>(t =>
        {
            t.txtMessage.text = message;
            t._coolDown = DOTween.Sequence().AppendInterval(delayTime).OnComplete(t.TurnOff);
        });
    }

    public override void TurnOff()
    {
        _coolDown?.Kill();
        base.TurnOff();
    }
}
