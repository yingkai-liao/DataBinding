using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using System;

public abstract class BindingBehaviorBase : MonoBehaviour
{
    [Tooltip("requestText中的 \"{$}\" 會被替換成rootPath上寫的路徑")]
    public BindingPathRoot basePath;
    public string requestText = "";    
    [NonSerialized]
    public string trueRequestText;
    public bool isRequestValid { get; private set; }

    public virtual void Start()
    {
        if (basePath != null)
            basePath.Register(this);
        else
        {
            trueRequestText = requestText;
            ReBuildTrueRequestPath();
        }
    }

    public void OnRootPathUpdate()
    {
        if (isActiveAndEnabled)
            ReBuildTrueRequestPath();
    }

    public virtual void ReBuildTrueRequestPath()
    {
        bool needRootPath = requestText.Contains("$");
        if (needRootPath)
        {
            if (basePath == null || string.IsNullOrEmpty(basePath.requestText))
            {
                isRequestValid = false;
                Debug.LogError("rootPath is empty!", this);
                OnInvalidResult();
                return;
            }
            trueRequestText = requestText.Replace("$", basePath.requestText);
        }
        else
        {
            trueRequestText = requestText;
        }

        if(string.IsNullOrEmpty(trueRequestText))
        {
            isRequestValid = false;
            Debug.LogError("request is empty!", this);            
            OnInvalidResult();
            return;
        }

        isRequestValid = true;
        OnRequest();
    }

    public abstract void OnInvalidResult();
    public abstract void OnRequest();
}
