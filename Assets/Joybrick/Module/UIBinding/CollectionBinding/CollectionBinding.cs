using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Joybrick;
using System;

[AddComponentMenu("DataBinding/CollectionBinding")]
[RequireComponent(typeof(UIPrefabPool))]
public class CollectionBinding : DeepBindingBehavior
{
    UIPrefabPool _prefabPool;
    UIPrefabPool PrefabPool { get { if (_prefabPool == null) _prefabPool = GetComponent<UIPrefabPool>(); return _prefabPool; } }

    string pathRoot = "";

    public async override void onChange(object value)
    {
        await UniTask.SwitchToMainThread();
        if (value == null)
        {
            OnInvalidResult();
            return;
        }

        if(deepBinder.process.Count > 0)
            pathRoot = deepBinder.process.Last().request;

        if (this.IsVariable)
        {
            var target = deepBinder.GetTargetDataBindPair();
            var keys = target.source.GetAllAttributeName().ToArray();
            UpdateList(keys);
        }
        else 
        {
            var result = value.ToString().Split(new char[] { ',' });
            UpdateList(result);
        }
    }

    protected virtual void UpdateList(string[] setPath)
    {
        PrefabPool.SetItemData(setPath, UpdateItem);
    }

    private void UpdateItem(int index, object data, UICommonPrefab instance)
    {
        var bindingVariable = instance.GetComponent<DataBindVariable>();
        if(bindingVariable != null)
        {            
            var key = (string)data;
            bindingVariable.SetVariable("Path", $"{pathRoot}.{key}");
            bindingVariable.SetVariable("Index", key);            
        }
    }

    public async override void OnInvalidResult()
    {
        await UniTask.SwitchToMainThread();
        UpdateList(new string[0]);
    }
}