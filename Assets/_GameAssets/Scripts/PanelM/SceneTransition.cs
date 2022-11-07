using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance => TopCanvas.Instance.sceneTransition;
    public static void ChangeScene(string sceneName)
    {
        Instance.StartCoroutine(Instance.ChangeSceneCor(sceneName));
        
    }
    [SerializeField] private CanvasGroup cg;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text txtPercent;

    private void SetPercentLoaded(float percent)
    {
        slider.value = percent;
        txtPercent.text = $"Now Loading ({slider.value * 100f:F0}%) ...";
    }
    
    private IEnumerator ChangeSceneCor(string sceneName)
    {
        SetPercentLoaded(0);
        cg.interactable = cg.blocksRaycasts = true;
        yield return cg.DOFade(1, 0.2f).WaitForCompletion();
        var loadSceneAsync = Addressables.LoadSceneAsync(sceneName);
        while (!loadSceneAsync.IsDone)
        {
            SetPercentLoaded(loadSceneAsync.PercentComplete);
            yield return null;
        }
        yield return cg.DOFade(0, 0.2f).SetDelay(0.5f).WaitForCompletion();
        cg.interactable = cg.blocksRaycasts = false;
    }
}
