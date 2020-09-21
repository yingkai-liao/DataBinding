using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using System;

[AddComponentMenu("DataBinding/SliderBinding")]
[RequireComponent(typeof(SliderBinding))]
public class SliderBinding : DeepBindingBehavior
{
    Slider target;

    public override void Start()
    {
        target = GetComponent<Slider>();
        base.Start();
    }

    public override async void onChange(object result)
    {
        await UniTask.SwitchToMainThread();

        if (result != null)
        {
            float value = (float)result;
            if (float.IsNaN(value))
                value = 0f;
            value = Mathf.Clamp01(value);
            target.value = value;
        }
        else
            OnInvalidResult();
    }

    public async override void OnInvalidResult()
    {
        await UniTask.SwitchToMainThread();
        target.value = 0;
    }
}
