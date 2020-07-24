using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using System;
using Joybrick;

public abstract class DeepBindingBehavior : BindingBehaviorBase, IDisposable
{
    public DeepBindVarable deepBinder;
    protected IDisposable handle;

    public bool IsVariable { get { return deepBinder.parseResult == DeepBindVarable.ParseResult.FinalVariable; } }

    bool disableDetect = false;
    abstract public void onChange(object value);

    public virtual void Dispose()
    {
        if (handle != null)
        {
            handle.Dispose();
            handle = null;
        }
        if (deepBinder != null)
            deepBinder.Dispose();
    }

    private void OnDestroy()
    {
        Dispose();
    }

    public virtual void OnDisable()
    {
        if (disableDetect)
            return;

        disableDetect = true;
        //下個frame才停止subscribe 避免同frame瞬間開關觸動
        UniTask.NextFrame(PlayerLoopTiming.EarlyUpdate).ToObservable().Subscribe(x => {
            if (!isActiveAndEnabled)
                Dispose();              
            disableDetect = false;
        });
        
    }

    public virtual void OnEnable()
    {
        if(disableDetect) //正在確定是否真的關了
            return;
        
        if (deepBinder != null && handle == null)
        {
            deepBinder.Update();
            handle = deepBinder.Subscribe(onChange);
            onChange(deepBinder.Value);
        }
    }

    public override void OnRequest()
    {
        Dispose();

        var manager = DeepBindManager.Instance;
        deepBinder = manager.Request(trueRequestText);
        handle = deepBinder.Subscribe(onChange);
        onChange(deepBinder.Value);
    }
}