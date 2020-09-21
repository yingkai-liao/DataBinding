using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Joybrick;

public class UIPrefabPool :MonoBehaviour
{
    public UICommonPrefab prefab;
    public List<UICommonPrefab> list;

    private void Awake()
    {
        if (prefab.transform.parent != null)
            prefab.gameObject.SetActive(false);
    }

    public void SetItemData(int count, UIPrefabSetupCB callback)
    {
        UIHelper.UpdatePoolByPrefab<UICommonPrefab>(prefab, list, count, transform);        
        for (var i = 0; i < count; i++)
            list[i].UpdateItemData(new UIPrefabDesc() { index = i, callabck = callback });        
    }

    public void SetItemData(IList dataList, UIPrefabSetupCB callback)
    {
        UIHelper.UpdatePoolByPrefab<UICommonPrefab>(prefab, list, dataList.Count, transform);
        for (var i = 0; i < dataList.Count; i++)
            list[i].UpdateItemData(new UIPrefabDesc() { index = i, data = dataList[i], callabck = callback });
    }
}