using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    [Serializable]
    public class Tab
    {
        public Button tabButton;
        public string tabPanelKey;
    }
    [SerializeField] private int defaultTabIndex;
    private int _currentTabIndex = -1;
    [SerializeField] private List<Tab> tabs;
    [SerializeField] private Transform tabContainer;
    private void Start()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            var idx = i;
            tabs[idx].tabButton.onClick.AddListener(() =>
            {
                if (_currentTabIndex >= 0)
                {
                    tabs[_currentTabIndex].tabButton.interactable = true;
                    tabContainer.GetComponentInChildren<Panel>()?.TurnOff();
                }
                _currentTabIndex = idx;
                tabs[_currentTabIndex].tabButton.interactable = false;
                Panel.Open(tabs[_currentTabIndex].tabPanelKey, container: tabContainer);
            });
        }
        tabs[defaultTabIndex].tabButton.onClick?.Invoke();
    }
}
