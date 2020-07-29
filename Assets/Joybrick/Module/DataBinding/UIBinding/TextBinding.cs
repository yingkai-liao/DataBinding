using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using System;

[AddComponentMenu("DataBinding/TextBinding")]
[RequireComponent(typeof(Text))]
public class TextBinding : DeepBindingBehavior
{
    Text targetText;
    

    public override void Start()
    {
        targetText = GetComponent<Text>();
        base.Start();
    }

    public override async void onChange(object result)
    {
        await UniTask.SwitchToMainThread();
        if (result != null)
            targetText.text = result.ToString();
        else
            OnInvalidResult();
    }

    public async override void OnInvalidResult()
    {
        await UniTask.SwitchToMainThread();
        targetText.text = "";
    }
}
