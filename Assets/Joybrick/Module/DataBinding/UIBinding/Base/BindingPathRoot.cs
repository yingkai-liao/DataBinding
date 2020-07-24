using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using System;
using System.Collections.Generic;

[AddComponentMenu("DataBinding/BindingPathRoot")]
public class BindingPathRoot : MonoBehaviour
{
    public string requestText = "";
    List<BindingBehaviorBase> childrens = new List<BindingBehaviorBase>();
    
    public void Register(BindingBehaviorBase item)
    {
        childrens.Add(item);
        item.OnRootPathUpdate();
    }

    internal void SetRootPath(string path)
    {        
        this.requestText = path;
        foreach(var item in childrens)
            item.OnRootPathUpdate();
    }
}
