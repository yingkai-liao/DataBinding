using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using System;

[AddComponentMenu("DataBinding/ImageResourcesBinding")]
[RequireComponent(typeof(Image))]
public class ImageResourcesBinding : DeepBindingBehavior
{
    Image image;

    public override void Start()
    {
        image = GetComponent<Image>();
        base.Start();
    }

    public override async void onChange(object result)
    {
        await UniTask.SwitchToMainThread();
        if (result == null || string.IsNullOrEmpty(result.ToString()))
            return;

        image.sprite = Resources.Load<Sprite>(result.ToString());
    }

    public async override void OnInvalidResult()
    {
        await UniTask.SwitchToMainThread();
        image.sprite = null;
    }
}
