using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using System;
using Joybrick;

public class OnToggleChangeBinding : SetBindingVariable
{
    Toggle toggle;

    public override void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(x => OnRequest());
        base.Start();        
    }

    public override void OnRequest()
    {
        //request路徑發生變更
        var path = DoSetBindingValue();
        DataBindingManager.Instance.SetSource(path + ".IsOn", toggle.isOn);
    }
}
