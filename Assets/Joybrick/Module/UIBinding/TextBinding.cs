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
    public string Format = "";

    public override void Start()
    {
        targetText = GetComponent<Text>();
        base.Start();
    }

    public override async void onChange(object result)
    {
        await UniTask.SwitchToMainThread();
        if (result != null)
        {
            if (string.IsNullOrWhiteSpace(Format))
                targetText.text = result.ToString();
            else
                targetText.text = string.Format(Format, result);
        }
        else
            OnInvalidResult();
    }

    public async override void OnInvalidResult()
    {
        await UniTask.SwitchToMainThread();
        targetText.text = "";
    }
}
