using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTraining : MonoBehaviour
{
    [SerializeField] private Image imgCooldown;
    [SerializeField] private Image imgIcon;
    [SerializeField] private TMP_Text txtCooldown;
    [SerializeField] private TMP_Text txtQueueCount;
    [SerializeField] private TMP_Text txtPriceCrystal;

    private TrainingProcess _trainingProcess;
    public void Setup(TrainingProcess trainingProcess)
    {
        _trainingProcess = trainingProcess;
        imgIcon.sprite = _trainingProcess.heroConfig.icon;
        txtPriceCrystal.gameObject.SetActive(_trainingProcess.heroConfig.priceCrystalTraining > 0);
        txtPriceCrystal.text = _trainingProcess.heroConfig.priceCrystalTraining.ToString();
        _trainingProcess.queueCount.onValueChanged += OnQueueCountChange;
        _trainingProcess.coolDown.onValueChanged += OnCoolDownChange;
        OnQueueCountChange();
    }

    private void OnDestroy()
    {
        _trainingProcess.coolDown.onValueChanged -= OnCoolDownChange;
        _trainingProcess.queueCount.onValueChanged -= OnQueueCountChange;
    }

    private void OnCoolDownChange()
    {
        txtCooldown?.SetText(_trainingProcess.coolDown.Value.ToString(_trainingProcess.coolDown.Value > 1 ? "F0" : "F1"));
        imgCooldown.fillAmount = _trainingProcess.coolDown.Value / _trainingProcess.heroConfig.timeTraining;
    }
    private void OnQueueCountChange()
    {
        imgCooldown.gameObject.SetActive(_trainingProcess.queueCount.Value>0);
        txtQueueCount.text = _trainingProcess.queueCount.ToString();
    }
    public void OnTrainingClick()
    {
        _trainingProcess.AddTrain();
    }
    
    public void OnCancelTrainingClick()
    {
        _trainingProcess.RemoveTrain();
    }
}
