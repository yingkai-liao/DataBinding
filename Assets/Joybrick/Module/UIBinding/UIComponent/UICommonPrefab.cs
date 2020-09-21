using System.Collections.Generic;
using UnityEngine;

public delegate void UIPrefabSetupCB(int index, object data, UICommonPrefab instance);

public class UIPrefabDesc
{   
    public int index;
    public object data;
    public UIPrefabSetupCB callabck;
}

public class UICommonPrefab : MonoBehaviour
{
    [HideInInspector]
    public Dictionary<string, UnityEngine.Object> ui = new Dictionary<string, UnityEngine.Object>();
    bool inited = false;

    public virtual void Awake()
    {
        if (inited == false)
            Hide();
    }

    public UnityEngine.Object GetUI(string key)
    {
        if(ui.TryGetValue(key,out var obj))
        {
            return obj;
        }

        throw new System.Exception($"Can't find Object - {key}");
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateItemData(object data)
    {
        gameObject.SetActive(true);
        UIPrefabDesc value = data as UIPrefabDesc;
        value.callabck(value.index, value.data, this);
        inited = true;
    }
}