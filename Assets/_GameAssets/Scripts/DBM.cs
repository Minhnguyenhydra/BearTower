using System;
using Newtonsoft.Json;
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
[CreateAssetMenu(fileName = "DBM",menuName = "SO/DBM")]
public class DBM : ScriptableObject
{
    private static DBM _instance;
    private static DBM Instance
    {
        get
        {
            if (_instance == null) _instance = Addressables.LoadAssetAsync<DBM>(nameof(DBM)).WaitForCompletion();
            return _instance;
        }
    }
    [SerializeField] private Config config;
    public static Config Config => Instance.config;
    [SerializeField] private UserData initUserData;
    private UserData _userData;
    public static UserData UserData
    {
        get
        {
            if (Instance._userData == null)
            {
                if (PlayerPrefs.GetString(nameof(UserData), "") == "")
                    PlayerPrefs.SetString(nameof(UserData), JsonConvert.SerializeObject(Instance.initUserData));
                Instance._userData =JsonConvert.DeserializeObject<UserData>(PlayerPrefs.GetString(nameof(UserData), ""));
            }
            return Instance._userData;
        }
    }
}

