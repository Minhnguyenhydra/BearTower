using DG.Tweening;
using UnityEngine;

public class BlockSceen : MonoBehaviour
{
    public static BlockSceen Instance => TopCanvas.Instance.blockSceen;
    public static void Show()
    {
        Instance.cg.interactable = Instance.cg.blocksRaycasts = true;
        Instance.cg.DOKill();
        Instance.cg.DOFade(1, 0.2f).SetDelay(0.2f);
    }
    public static void Hide()
    {
        Instance.cg.interactable = Instance.cg.blocksRaycasts = false;
        Instance.cg.DOKill();
        Instance.cg.DOFade(0, 0.2f);
    }
    [SerializeField] private CanvasGroup cg;
}
