
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Config
{
    public ShopDataConfig shop = new ShopDataConfig();
}
[Serializable]
public class ShopDataConfig
{
    public List<ShopItem> listItem = new List<ShopItem>();
}
[Serializable]
public class ShopItem
{
    public Sprite icon;
    public ResourcesType type;
    public int value;
    public int price;
    public bool Buy()
    {
        if (price > DBM.UserData.Resources[ResourcesType.Gem].Value) return false;
        DBM.UserData.Resources[ResourcesType.Gem].Value -= price;
        DBM.UserData.Resources[type].Value += value;
        DBM.Save();
        return true;
    }
}
