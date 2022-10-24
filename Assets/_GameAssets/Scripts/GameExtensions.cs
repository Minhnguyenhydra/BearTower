using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _GameAssets.Scripts
{
    public static class GameExtensions
    {
        public static void FillData<TD,T>(this Component component,IEnumerable<TD> data,Action<TD,T,int> itemAction) where T : Component
        {
            var listData = data.ToList();
            var transform = component.transform;
            for (var i = 0; i < Mathf.Max(listData.Count, transform.childCount); i++)
            {
                var idx = i;
                if (idx == transform.childCount) Object.Instantiate(transform.GetChild(0), transform);
                if (idx < listData.Count)
                {
                    transform.GetChild(idx).gameObject.SetActive(true);
                    itemAction.Invoke(listData[idx], transform.GetChild(idx).GetComponent<T>(), idx);
                }
                else transform.GetChild(idx).gameObject.SetActive(false);
            }
        }
    }
}