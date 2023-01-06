using System;
using System.Collections;
using System.Collections.Generic;
using _GameAssets.Scripts;
using Coffee.UIEffects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : Panel
{
    [SerializeField] private Transform container;
    [SerializeField] private Image iconSelected;
    [SerializeField] private TMP_Text txtLevel;
    [SerializeField] private Button btnUpgrade;
    [SerializeField] private Slider slider;
    private UpgradeHeroItem heroSelected;
    public void Start()
    {
        UpdateView();
        container.FillData<UpgradeHeroItem, Button>(DBM.UserData.upgrade.DictHero.Values, (item, button, idx) =>
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnSelect(item));
        });
        container.GetChild(0).GetComponent<Button>().onClick.Invoke();
        btnUpgrade.onClick.AddListener(OnClickUpgrade);
    }

    private void UpdateView()
    {
        container.FillData<UpgradeHeroItem, Button>(DBM.UserData.upgrade.DictHero.Values, (item, button, idx) =>
        {
            button.transform.GetChild(1).GetComponent<Image>().sprite = DBM.Config.HeroConfigs[item.heroId].icon;
            button.transform.GetChild(2).GetComponent<TMP_Text>().text = $"LV {item.level}.{item.subLevel}";
        });
    }
    public void OnSelect(UpgradeHeroItem item)
    {
        heroSelected = item;
        iconSelected.sprite = DBM.Config.HeroConfigs[item.heroId].icon;
        txtLevel.text = item.level.ToString();
        btnUpgrade.interactable = item.level < DBM.Config.upgrade.priceList.Count;
        btnUpgrade.GetComponentInChildren<TMP_Text>().text = DBM.Config.upgrade.priceList[item.level-1].ToString();
        slider.value = item.subLevel;
        slider.GetComponentInChildren<TMP_Text>().text = $"{item.subLevel}/5";
    }

    public void OnClickUpgrade()
    {
        var message = DBM.UserData.upgrade.Upgrade(heroSelected.heroId);
        Toast.Show(message);
        if (message == "Success")
        {
            OnSelect(heroSelected);
            UpdateView();
        }
    }
}
