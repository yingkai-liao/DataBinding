using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using System;

public abstract class BindingBehaviorBase : MonoBehaviour
{
    public DataBindVariable variables;
    public string requestText = "";    
    [NonSerialized]
    public string trueRequestText;
    public bool isRequestValid { get; private set; }

    public virtual void Start()
    {
        if (variables != null)
            variables.Register(this);
        else
        {
            trueRequestText = requestText;
            ReBuildTrueRequestPath();
        }
    }

    public void SetRequest(string request)
    {
        this.requestText = request;
        ReBuildTrueRequestPath();
    }

    public void OnVariableUpdate()
    {
        if (isActiveAndEnabled)
            ReBuildTrueRequestPath();
    }

    public virtual void ReBuildTrueRequestPath(bool doRequestAfter = true)
    {        
        if (string.IsNullOrEmpty(requestText))
        {
            isRequestValid = false;
            OnInvalidResult();
            return;
        }

        trueRequestText = requestText;
        if (variables != null)
        {
            foreach(var item in variables.variable)
            {
                trueRequestText = trueRequestText.Replace($"{{$.{item.name}}}", item.value);
            }
        }

        if (trueRequestText.Contains("$"))
        {
            isRequestValid = false;
            OnInvalidResult();
            Debug.Log("variable not prepared!");
            return;
        }

        if (string.IsNullOrEmpty(trueRequestText))
        {
            isRequestValid = false;
            OnInvalidResult();
            return;
        }


        isRequestValid = true;
        if(doRequestAfter)
            OnRequest();
    }

    public abstract void OnInvalidResult();
    public abstract void OnRequest();
}
