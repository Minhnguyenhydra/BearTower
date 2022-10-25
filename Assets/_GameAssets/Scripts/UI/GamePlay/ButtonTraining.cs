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
    [SerializeField] private TMP_Text txtQueueCount;
    [SerializeField] private TMP_Text txtPriceGold;
    [SerializeField] private TMP_Text txtPriceMana;
    [SerializeField] private TMP_Text txtName;

    private TrainingProcess _trainingProcess;
    public void Setup(TrainingProcess trainingProcess)
    {
        _trainingProcess = trainingProcess;
        txtName?.SetText(_trainingProcess.heroConfig.Name);
        imgIcon.sprite = _trainingProcess.heroConfig.icon;
        txtPriceGold.gameObject.SetActive(_trainingProcess.heroConfig.priceGoldTraining > 0);
        txtPriceGold.text = _trainingProcess.heroConfig.priceGoldTraining.ToString();
        txtPriceMana.gameObject.SetActive(_trainingProcess.heroConfig.priceManaTraining > 0);
        txtPriceMana.text = _trainingProcess.heroConfig.priceManaTraining.ToString();
        _trainingProcess.onQueueCountChanged += OnQueueCountChange;
        _trainingProcess.onCoolDownChanged += OnCoolDownChange;
        OnQueueCountChange();
    }

    private void OnDestroy()
    {
        _trainingProcess.onCoolDownChanged -= OnCoolDownChange;
        _trainingProcess.onQueueCountChanged -= OnQueueCountChange;
    }

    private void OnCoolDownChange()
    {
        imgCooldown.fillAmount = _trainingProcess.CoolDown / _trainingProcess.heroConfig.timeTraining;
    }
    private void OnQueueCountChange()
    {
        imgCooldown.gameObject.SetActive(_trainingProcess.QueueCount>0);
        txtQueueCount.text = _trainingProcess.QueueCount.ToString();
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
