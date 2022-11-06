using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Popup : Panel
{
    [SerializeField] private TMP_Text txtHeader;
    [SerializeField] private TMP_Text txtMessage;
    [SerializeField] private Button btnConfirm;
    [SerializeField] private Button btnCancel;
    private bool _forceChoose = true;
    public static void Show(string message)
    {
        Show("Notice", message,"Ok");
    }
    public static void Show(string header, string message, string txtConfirm = "", UnityAction confirmCallback = null,
        string txtCancel = "", UnityAction cancelCallBack = null, bool forceChoose = false)
    {
        Open<Popup>(p => p.SetupPopup(header, message, txtConfirm, confirmCallback, txtCancel, cancelCallBack, forceChoose));
    }
    private void SetupPopup(string header, string message, string txtConfirm, UnityAction confirmCallback,
        string txtCancel, UnityAction cancelCallBack, bool forceChoose)
    {
        this._forceChoose = forceChoose;
        txtHeader.text=header;
        txtMessage.text=message;
        SetupButton(btnConfirm,txtConfirm,confirmCallback);
        SetupButton(btnCancel,txtCancel,cancelCallBack);
    }

    private void SetupButton(Button btn,string text,UnityAction callback)
    {
        if(!btn) return;
        btn.gameObject.SetActive(!string.IsNullOrEmpty(text));
        if (!btn.gameObject.activeSelf) return;
        btn.GetComponentInChildren<TMP_Text>().text = text;
        btn.onClick.RemoveAllListeners();
        if (callback != null) btnConfirm.onClick.AddListener(callback);
        btn.onClick.AddListener(TurnOff);
    }

    public override void BackClick()
    {
        if (!_forceChoose) TurnOff();
    }
}
