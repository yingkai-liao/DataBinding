using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using System;
using Joybrick;

public class OnButtonClickBinding : SetBindingVariable
{
    Button button;

    public override void Start()
    {
        base.Start();
        button = GetComponent<Button>();
        button.onClick.AddListener(()=>DoSetBindingValue());
    }
}
