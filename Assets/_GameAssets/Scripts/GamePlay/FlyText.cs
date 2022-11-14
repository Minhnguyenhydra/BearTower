using DG.Tweening;
using Lean.Pool;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class FlyText : MonoBehaviour
{
    [SerializeField] private TMP_Text txtText;
    private static FlyText flyTextPrefab;
    private static FlyText FlyTextPrefab
    {
        get
        {
            if (flyTextPrefab == null)
                flyTextPrefab = Addressables.LoadAssetAsync<GameObject>(nameof(FlyText)).WaitForCompletion()
                    .GetComponent<FlyText>();
            return flyTextPrefab;
        }
    }
    public static void Spawn(Vector3 pos, string text,Color? color=null)
    {
        if (color == null) color = Color.red;
        var flyText = LeanPool.Spawn(FlyTextPrefab, pos, Quaternion.identity);
        flyText.txtText.color = (Color) color;
        flyText.txtText.text = text;
        flyText.transform.DOMoveY(2, 1f).SetRelative(true).OnComplete(() =>
        {
            LeanPool.Despawn(flyText);
        });
    }
}
