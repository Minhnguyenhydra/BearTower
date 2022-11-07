
using System;
using _GameAssets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : Panel
{
    [SerializeField] private Transform container;
    private void Start()
    {
        container.FillData<ShopItem, Button>(DBM.Config.shop.listItem, (item, button, idx) =>
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnClickItemShop(item));
            button.transform.GetChild(0).GetComponent<Image>().sprite = item.icon;
            button.GetComponentInChildren<TMP_Text>().text =item.price==0?"IAP":item.price.ToString();
        });
    }

    private void OnClickItemShop(ShopItem item)
    {
        var success = item.Buy();
        if (!success) Toast.Show("Can't buy");
    }
}
