using System;
using System.Collections.Generic;
using System.Linq;
using BrunoMikoski.AnimationSequencer;
using UnityEngine;
using UnityEngine.AddressableAssets;


[DisallowMultipleComponent]
public class Panel : MonoBehaviour
{
    #region Static Region

    private static Transform _defaultContainer;

    private static Transform DefaultContainer
    {
        get
        {
            if (_defaultContainer == null) _defaultContainer = FindObjectOfType<PanelCloser>().transform;
            return _defaultContainer;
        }
    }
    
    public static readonly List<Panel> PoolPanels = new List<Panel>();
    public static void Open<T>(string panelKey,Action<T> onCompleted = null,Transform container=null) where T : Panel
    {
        Open(string.IsNullOrEmpty(panelKey) ? typeof(T).ToString() : panelKey, panel => onCompleted?.Invoke(panel as T), container);
    }
    public static void Open<T>(Action<T> onCompleted = null,Transform container=null) where T : Panel
    {
        Open(typeof(T).ToString(), panel => onCompleted?.Invoke(panel as T),container);
    }
    public static void Open(string panelKey, Action<Panel> onCompleted = null, Transform container = null)
    {
        if (container == null) container = DefaultContainer;
        if (TryGetPooledPanel(panelKey, out var pooledPanel))
        {
            PoolPanels.Remove(pooledPanel);
            GetPanelSuccess(pooledPanel, onCompleted, container);
        }
        else
        {
            var handle = Addressables.InstantiateAsync(panelKey, container);
            BlockSceen.Show();
            handle.Completed += op =>
            {
                BlockSceen.Hide();
                var p = op.Result.GetComponent<Panel>();
                p._key = panelKey;
                p.OnDestroyEvent += () => Addressables.Release(op);
                GetPanelSuccess(p, onCompleted, container);
            };
        }
    }
    private static bool TryGetPooledPanel(string panelKey,out Panel panel)
    {
        panel = PoolPanels.FirstOrDefault(p => p._key == panelKey);
        return panel != null;
    }
    
    private static void GetPanelSuccess(Panel panel, Action<Panel> onCompleted = null, Transform container = null)
    {
        panel.transform.SetParent(container);
        panel.transform.SetAsLastSibling();
        panel.Anim.SetProgress(1);
        panel.TurnOn();
        onCompleted?.Invoke(panel);
    }
    #endregion
    
    private string _key;
    private event Action OnDestroyEvent;
    private AnimationSequencerController _anim;
    private AnimationSequencerController Anim
    {
        get
        {
            if (_anim == null)
                _anim = GetComponent<AnimationSequencerController>();
            return _anim;
        }
    }
    public virtual void TurnOn()
    {
        gameObject.SetActive(true);
        Anim.PlayBackwards(false);
    }
    public virtual void TurnOff()
    {
        if (!PoolPanels.Contains(this)) PoolPanels.Add(this);
        Anim.PlayForward(false, () => gameObject.SetActive(false));
    }
    protected virtual void OnDestroy()
    {
        if (PoolPanels.Contains(this)) PoolPanels.Remove(this);
        OnDestroyEvent?.Invoke();
    }
}