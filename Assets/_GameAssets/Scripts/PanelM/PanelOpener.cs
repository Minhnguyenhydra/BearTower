using UnityEngine;
using UnityEngine.AddressableAssets;
[CreateAssetMenu(fileName = "PanelOpener",menuName = "SO/PanelOpener")]
public class PanelOpener : ScriptableObject
{
    public void OpenPanel(string key)
    {
        Panel.Open(key);
    }

    public void OpenToast(string message)
    {
        Toast.Show(message);
    }

    public void OpenPopup(string message)
    {
        Popup.Show(message);
    }

    public void ChangeScene(string sceneName)
    {
        SceneTransition.ChangeScene(sceneName);
    }
}
