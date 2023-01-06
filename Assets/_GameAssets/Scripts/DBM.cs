using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public partial class Config
{
    
}
[Serializable]
public partial class UserData
{
    
}
public class ListReplacementUserContractResolver : DefaultContractResolver
{
    protected override List<MemberInfo> GetSerializableMembers(Type objectType)
    {
        var fields = objectType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToList<MemberInfo>(); 
        var memberInfos = fields.Where(fi => Attribute.IsDefined(fi, typeof(JsonPropertyAttribute))).ToList();
        return memberInfos;
    }
}
[CreateAssetMenu(fileName = "DBM",menuName = "SO/DBM")]
public class DBM : ScriptableObject
{
    [ShowInInspector]
    private static DBM _instance;
    private static DBM Instance
    {
        get
        {
            if (_instance == null){
                _instance = Instantiate(Resources.Load<DBM>(nameof(DBM)));//.LoadAssetAsync<DBM>(nameof(DBM)).WaitForCompletion();
                if (PlayerPrefs.GetString(nameof(UserData), "") != "")
                {
                    _instance._userData = JsonConvert.DeserializeObject<UserData>(
                        PlayerPrefs.GetString(nameof(UserData), ""),
                        new JsonSerializerSettings()
                        {
                            ContractResolver = new ListReplacementUserContractResolver()
                        });
                }
            }
            return _instance;
        }
    }
    [SerializeField] private Config config;
    public static Config Config => Instance.config;
    // [SerializeField] private UserData initUserData;
    [SerializeField] private UserData _userData;
      public static UserData UserData => Instance._userData;

      public static void Save()
    {
        var str = JsonConvert.SerializeObject(Instance._userData, new JsonSerializerSettings()
        {
            ContractResolver = new ListReplacementUserContractResolver()
        });
        Debug.Log(str);
        PlayerPrefs.SetString(nameof(UserData), str);
    }
}

