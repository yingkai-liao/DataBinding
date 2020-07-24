using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using System;

[AddComponentMenu("DataBinding/ActiveBinding")]
public class ActiveBinding : BoolBinding
{
    public override async void onChange(object obj)
    {
        await UniTask.SwitchToMainThread();
        var on = IsCheck(obj);
        gameObject.SetActive(on);
    }

    public override void OnEnable()
    {
        //enable不影響監聽
    }

    public override void OnDisable()
    {
        //enable不影響監聽
    }

    //parent路徑被設為null 
    public async override void OnInvalidResult()
    {
        await UniTask.SwitchToMainThread();
        gameObject.SetActive(false);
    }
}
