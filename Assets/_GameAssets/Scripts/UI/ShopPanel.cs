
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
            button.transform.GetChild(2).GetComponent<Image>().sprite = item.icon;
            button.GetComponentInChildren<TMP_Text>().text = item.value+" "+item.type;
            button.transform.GetChild(3).GetComponentInChildren<TMP_Text>().text =item.price==0?"IAP":(item.price+" Gem");
        });
    }

    private void OnClickItemShop(ShopItem item)
    {
        var success = item.Buy();
        Toast.Show(success ? "Success!" : "Can't buy");
    }
}
