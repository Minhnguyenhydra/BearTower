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
    private UpgradeHeroItem heroSelected;
    public void Start()
    {
        container.FillData<UpgradeHeroItem, Button>(DBM.UserData.upgrade.DictHero.Values, (item, button, idx) =>
        {
            button.GetComponent<Image>().sprite = DBM.Config.HeroConfigs[item.heroId].icon;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnSelect(item));
            if (idx == 0) button.onClick.Invoke();
        });
        btnUpgrade.onClick.AddListener(OnClickUpgrade);
    }
    public void OnSelect(UpgradeHeroItem item)
    {
        heroSelected = item;
        iconSelected.sprite = DBM.Config.HeroConfigs[item.heroId].icon;
        txtLevel.text = item.level.ToString();
        btnUpgrade.interactable = item.level < DBM.Config.upgrade.priceList.Count;
        btnUpgrade.GetComponent<UIEffect>().enabled = !btnUpgrade.interactable;
    }

    public void OnClickUpgrade()
    {
        var message = DBM.UserData.upgrade.Upgrade(heroSelected.heroId);
        Toast.Show(message);
        if (message=="Success")
            OnSelect(heroSelected);
    }
}
