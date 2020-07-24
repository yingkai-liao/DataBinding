using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using System;
using Joybrick;

public class UIEvent
{
    [DataBind("gameObject")]
    public GameObject gameObject;
    [DataBind("name")]
    public string name;
    [DataBind("position")]
    public Vector3 position;
    [DataBind("rotation")]
    public Quaternion rotation;
    [DataBind("lscale")]
    public Vector3 scale;
    [DataBind("lossyscale")]
    public Vector3 lossyscale;
    [DataBind("value")]
    public string value;

    public UIEvent(string value,GameObject sender)
    {
        gameObject = sender;
        name = gameObject.name;


        var _T = sender.transform;
        position = _T.position;
        rotation = _T.rotation;
        scale = _T.localScale;
        lossyscale = _T.lossyScale;
        this.value = value;
    }
}
