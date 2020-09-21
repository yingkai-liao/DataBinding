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
        ReBuildTrueRequestPath(false);
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
            DataBindingManager.Instance.SetSource(deepResult, eventData);
        }
        
        if (!string.IsNullOrEmpty(SetValue))
        {
            var trueSetVale = SetValue;
            if (variables != null)
            {
                foreach (var item in variables.variable)
                {
                    trueSetVale = trueSetVale.Replace($"{{$.{item.name}}}", item.value);
                }
            }

            var valueResult = DeepBindManager.Instance.GetRequestResult(trueSetVale);
            var data = DataBindingManager.Instance.GetValue(deepResult);
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
