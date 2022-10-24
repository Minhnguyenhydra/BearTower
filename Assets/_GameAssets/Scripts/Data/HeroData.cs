

using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class Config
{
    [SerializeField] private List<HeroConfig> listHero=new List<HeroConfig>();
    private Dictionary<string, HeroConfig> _heroConfigs;
    public Dictionary<string, HeroConfig> HeroConfigs => _heroConfigs ??= listHero.ToDictionary(config => config.Name, config => config);
}

[Serializable]
public class HeroConfig
{
    [ShowInInspector]
    public string Name => prefab==null?"":prefab.name;
    public Sprite icon;
    public Hero prefab;
    public float timeTraining=1;
    public int priceGoldTraining;
    public int priceManaTraining;
}

public partial class UserData
{
    public List<HeroUserData> listHeroUserData = new List<HeroUserData>();
}

[Serializable]
public class HeroUserData
{
    public string name;
    public int level;
}
