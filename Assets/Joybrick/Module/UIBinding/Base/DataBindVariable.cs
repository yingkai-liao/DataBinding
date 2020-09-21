using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using System;
using System.Collections.Generic;


[AddComponentMenu("DataBinding/DataBindVariable")]
public class DataBindVariable : MonoBehaviour
{
    [Serializable]
    public class Variable
    {
        public string name;
        public string value;
    }

    [SerializeField]
    public List<Variable> variable = new List<Variable>();
    List<BindingBehaviorBase> childrens = new List<BindingBehaviorBase>();
    
    public void Register(BindingBehaviorBase item)
    {
        childrens.Add(item);
        item.OnVariableUpdate();
    }

    internal void SetVariable(string name, string value)
    {
        var target = variable.Find(x => x.name == name);

        if (target != null)
            target.value = value;
        else
            variable.Add(new Variable() { name = name, value = value });

        foreach (var item in childrens)
            item.OnVariableUpdate();
    }
}
