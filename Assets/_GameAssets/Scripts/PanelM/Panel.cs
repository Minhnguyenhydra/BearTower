using System;
using System.Collections.Generic;
using System.Linq;
using BrunoMikoski.AnimationSequencer;
using Lean.Transition;
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
                p.Key = p.name = panelKey;
                p.OnDestroyEvent += () => Addressables.Release(op);
                p.Anim.SetProgress(1);
                GetPanelSuccess(p, onCompleted, container);
            };
        }
    }
    private static bool TryGetPooledPanel(string panelKey,out Panel panel)
    {
        panel = PoolPanels.FirstOrDefault(p => p.Key == panelKey);
        return panel != null;
    }
    
    private static void GetPanelSuccess(Panel panel, Action<Panel> onCompleted = null, Transform container = null)
    {
        panel.transform.SetParent(container);
        panel.transform.SetAsLastSibling();
        panel.TurnOn();
        onCompleted?.Invoke(panel);
    }
    #endregion

    private string _key;

    public string Key
    {
        get
        {
            if (string.IsNullOrEmpty(_key)) _key = name;
            return _key;
        }
        private set => _key = value;
    }

    [SerializeField] private bool destroyOnTurnOff;
    [SerializeField] private bool hideBackPanel;
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
        if (Anim != null)
            Anim.PlayBackwards(false, TurnOnDone);
        else TurnOnDone();
    }

    protected virtual void TurnOnDone()
    {
        if (hideBackPanel) SetActiveBackPanel(false);
    }

    private void SetActiveBackPanel(bool isShow)
    {
        for (var i = transform.parent.childCount - 2; i >= 0; i--)
        {
            var p = transform.parent.GetChild(i).GetComponent<Panel>();
            if (p != null && !PoolPanels.Contains(p))
            {
                p.gameObject.SetActive(isShow);
                if (p.hideBackPanel) break;
            }
        }
    }

    public virtual void TurnOff()
    {
        if (!PoolPanels.Contains(this)) PoolPanels.Add(this);
        if (hideBackPanel) SetActiveBackPanel(true);
        if (Anim != null) Anim.PlayForward(false, TurnOffDone);
        else TurnOffDone();

    }

    protected virtual void TurnOffDone()
    {
        if (destroyOnTurnOff) Destroy(gameObject);
        else gameObject.SetActive(false);
    }

    public virtual void BackClick()
    {
        TurnOff();
    }
    
    protected virtual void OnDestroy()
    {
        if (PoolPanels.Contains(this)) PoolPanels.Remove(this);
        OnDestroyEvent?.Invoke();
    }
}