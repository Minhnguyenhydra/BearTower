using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ResoucesView : MonoBehaviour
{
   
    [SerializeField]private ResourcesType resourcesType;
    [SerializeField]private TMP_Text txtValue;
    private void OnEnable()
    {
        DBM.UserData.Resources[resourcesType].onValueChange += UpdateView;
        UpdateView();
    }

    private void UpdateView()
    {
        txtValue.text = DBM.UserData.Resources[resourcesType].Value.ToString();
    }

    private void OnDisable()
    {
        DBM.UserData.Resources[resourcesType].onValueChange -= UpdateView;
    }
}
