
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class Config
{
    public UpgradeConfig upgrade = new UpgradeConfig();
}
[Serializable]
public class UpgradeConfig
{
    public List<int> priceList=new List<int>();
}

public partial class UserData
{
    [JsonProperty] public UpgradeUserData upgrade = new UpgradeUserData();
}

[Serializable]
public class UpgradeUserData
{
    [SerializeField,JsonProperty] private List<UpgradeHeroItem> listHero = new List<UpgradeHeroItem>();
    private Dictionary<string, UpgradeHeroItem> dictHero;
    public Dictionary<string, UpgradeHeroItem> DictHero => dictHero ??= listHero.ToDictionary(i => i.heroId, i => i);
    public string Upgrade(string heroId)
    {
        if (!DictHero.ContainsKey(heroId)) return "Can't find "+heroId;
            if(DictHero[heroId].level >= DBM.Config.upgrade.priceList.Count) return "Max Level";
            if (DBM.Config.upgrade.priceList[DictHero[heroId].level] > DBM.UserData.Resources[ResourcesType.Gold].Value)
                return "Not enough gold, need " + DBM.Config.upgrade.priceList[DictHero[heroId].level] +
                       " to upgrade to level " + (DictHero[heroId].level + 1);
            DBM.UserData.Resources[ResourcesType.Gold].Value -= DBM.Config.upgrade.priceList[DictHero[heroId].level];
            DictHero[heroId].level++;
            DBM.Save();
            return "Success";
    }
}

[Serializable]
public class UpgradeHeroItem
{
    [JsonProperty]
#if UNITY_EDITOR
    [ValueDropdown(nameof(ListHeroId))]
#endif
    public string heroId;
    
#if UNITY_EDITOR
    private IEnumerable<string> ListHeroId => DBM.Config.HeroConfigs.Keys;
#endif
    [JsonProperty]
    public int level = 0;
}

