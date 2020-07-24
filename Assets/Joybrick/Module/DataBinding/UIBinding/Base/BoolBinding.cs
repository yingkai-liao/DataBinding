using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using System;
using Joybrick;

public enum BoolCheckType
{
    Bool,
    Equal,
    Greater,
    Less,
    Empty,
    String,
}

public abstract class BoolBinding : DeepBindingBehavior
{
    public BoolCheckType CheckType;
    public string CheckString;
    public bool Invert;

    public bool IsCheck(object value)
    {
        if(value == null)
            return Invert ? true : false;

        var isCheck = false;
        var deepVariable = DeepBindManager.Instance.Request(CheckString);
        string result = deepVariable.Value.ToString();
        deepVariable.Dispose();

        switch (CheckType)
        {
            case BoolCheckType.Bool:
                isCheck = (bool)value;
                break;
            case BoolCheckType.Equal:
                isCheck = (double.Parse(value.ToString()) == (double.Parse(result.ToString())));
                break;
            case BoolCheckType.Greater:
                isCheck = (double.Parse(value.ToString()) > (double.Parse(result.ToString())));
                break;
            case BoolCheckType.Less:
                isCheck = (double.Parse(value.ToString()) < (double.Parse(result.ToString())));
                break;
            case BoolCheckType.String:
                isCheck = (value.ToString() == result);
                break;
        }
        return Invert ? !isCheck : isCheck;
    }
}
