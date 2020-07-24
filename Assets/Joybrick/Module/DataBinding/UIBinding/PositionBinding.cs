using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using System;


public class PositionBinding : DeepBindingBehavior
{

    public override async void onChange(object obj)
    {
        await UniTask.SwitchToMainThread();
        if (obj is Vector3)
        {
            transform.position = (Vector3)obj;
        }
        else
        {
            OnInvalidResult();
        }
    }

    public override void OnInvalidResult()
    {
    }
}
