using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using System;
using Joybrick;

public class SetBindingVariable : BindingBehaviorBase
{
    public string SetValue;
    public GameObject SetGameObjectValue;
    [Tooltip("use Time.frameCount as value")]
    public bool autoValue;

    public virtual string DoSetBindingValue()
    {
        ReBuildTrueRequestPath();
        if (!isRequestValid)
        {
            Debug.Log("request path not valid!", this);
            return "";
        }

        if (autoValue)
            SetValue = Time.frameCount.ToString();

        var deepResult = DeepBindManager.Instance.GetRequestResult(trueRequestText).ToString();
        if (SetGameObjectValue != null)
        {
            UIEvent eventData = new UIEvent(SetValue, SetGameObjectValue);
            DataBindingManager.Instance.SetDataContainer(deepResult, eventData);
        }
        
        if (!string.IsNullOrEmpty(SetValue))
        {
            var valueResult = DeepBindManager.Instance.GetRequestResult(SetValue);
            var data = DataBindingManager.Instance.GetDataPair(deepResult);
            if (data != null)
            {
                data.SetSource(valueResult);
            }
            else
            {
                Debug.Log("request path not valid!", this);
            }
        }
        return deepResult;
    }

    protected virtual void SetExtraValue(string deepResult)
    {
    }

    public override void OnInvalidResult()
    {
        
    }

    public override void OnRequest()
    { 
        
    }
}
