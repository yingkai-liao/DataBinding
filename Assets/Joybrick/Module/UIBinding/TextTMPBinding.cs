using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using System;
using TMPro;

[AddComponentMenu("DataBinding/TextTMPBinding")]
[RequireComponent(typeof(TextMeshProUGUI))]
public class TextTMPBinding : DeepBindingBehavior
{
    TextMeshProUGUI targetText;
    public string Format = "";

    public override void Start()
    {
        targetText = GetComponent<TextMeshProUGUI>();
        base.Start();
    }

    public override async void onChange(object result)
    {
        if(targetText == null)
            targetText = GetComponent<TextMeshProUGUI>();

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
