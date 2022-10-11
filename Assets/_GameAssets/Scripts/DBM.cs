using System;
using UnityEngine;

[Serializable]
public partial class Config
{
    
}
[Serializable]
public partial class UserData
{
    
}
[CreateAssetMenu(fileName = "DataM",menuName = "SO/DBM")]
public class DBM : ScriptableObject
{
    public Config config;
    public UserData initUserData;
}
