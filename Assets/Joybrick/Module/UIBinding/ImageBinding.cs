using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using System;
using Joybrick;

[AddComponentMenu("DataBinding/ImageBinding")]
[RequireComponent(typeof(Image))]
public class ImageBinding : DeepBindingBehavior
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

        await UIHelper.LoadIcon(image, result.ToString());
    }

    public async override void OnInvalidResult()
    {
        await UniTask.SwitchToMainThread();
        image.sprite = null;
    }
}
