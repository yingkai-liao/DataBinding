using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[AddComponentMenu("DataBinding/CollectionBinding")]
public class CollectionBinding : DeepBindingBehavior
{
    protected static StringBuilder _sb = new StringBuilder();

    public BindingPathRoot prefab;
    List<BindingPathRoot> objList = new List<BindingPathRoot>();

    public override void Start()
    {
        if (prefab != null && prefab.transform.parent != null)
            prefab.gameObject.SetActive(false);

        base.Start();
    }

    public async override void onChange(object value)
    {
        await UniTask.SwitchToMainThread();
        if (value == null)
        {
            OnInvalidResult();
            return;
        }

        if (value is IList && this.IsVariable)
        {
            var pathRoot = deepBinder.process.Last().request;
            var list = (IList)value;
            var result = new string[list.Count];

            for (int i = 0; i < list.Count;  i++)
                result[i] = _sb.Clear().Append("{").Append(pathRoot).Append(".").Append(i).Append("}").ToString();
            
            UpdateList(result);
        }
        else
        {
            var result = value.ToString().Split(new char[] { ',' });
            UpdateList(result);
        }
    }

    protected virtual void UpdateList(string[] setPath)
    {
        int count = setPath.Length;
        UIHelper.UpdatePoolByPrefab<BindingPathRoot>(prefab, objList, count, transform);        
        for (var i = 0; i < count; i++)
        {            
            objList[i].SetRootPath(setPath[i]);
            objList[i].gameObject.SetActive(true);
        }

        for (var i = count; i < objList.Count; i++)
        {
            objList[i].SetRootPath(null);
        }
    }

    public async override void OnInvalidResult()
    {
        await UniTask.SwitchToMainThread();
        UpdateList(new string[0]);
    }
}