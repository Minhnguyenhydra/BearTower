using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public partial class UserData
{ 
    [SerializeField,JsonProperty] private List<ResourcesData> listResources = new List<ResourcesData>();
    private Dictionary<ResourcesType, ResourcesData> _dictResources;
    public Dictionary<ResourcesType, ResourcesData> Resources=>_dictResources ??= listResources.ToDictionary(i => i.type, i => i);
}
public enum ResourcesType
{
    Gold,
    Gem,
}
[Serializable]
public class ResourcesData
{
    [JsonProperty]
    public ResourcesType type;
    [SerializeField,HideInInspector,JsonProperty] private float value=500;
    public Action onValueChange;
    [ShowInInspector]
    public float Value
    {
        get => value;
        set
        {
            this.value = value;
            onValueChange?.Invoke();
        }
    }
}
