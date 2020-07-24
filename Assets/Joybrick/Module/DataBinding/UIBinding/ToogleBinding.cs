using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using System;

[AddComponentMenu("DataBinding/ToogleBinding")]
[RequireComponent(typeof(Toggle))]
public class ToogleBinding : BoolBinding
{
    Toggle toggle;

    public override void Start()
    {
        toggle = GetComponent<Toggle>();
        base.Start();
    }

    public override async void onChange(object obj)
    {
        await UniTask.SwitchToMainThread();
        var on = IsCheck(obj);
        toggle.isOn = on;
    }

    public async override void OnInvalidResult()
    {
        await UniTask.SwitchToMainThread();
        toggle.isOn = false;
    }
}
