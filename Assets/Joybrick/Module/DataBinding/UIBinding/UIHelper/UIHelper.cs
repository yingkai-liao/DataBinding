using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHelper
{
    static public void UpdatePoolByPrefab<TPrefab>(TPrefab prefab, List<TPrefab> gameObjs, int dataCount, Transform parent = null) where TPrefab : UnityEngine.Component
    {
        if (parent == null)
            parent = prefab.transform.parent;

        for (int i = 0; i < dataCount; i++)
        {
            TPrefab view;
            if (i >= gameObjs.Count)
            {
                view = GameObject.Instantiate<TPrefab>(prefab, parent);
                gameObjs.Add(view);
            }
            else
                view = gameObjs[i];

            view.gameObject.SetActive(true);
        }

        for (int i = dataCount; i < gameObjs.Count; i++)
            gameObjs[i].gameObject.SetActive(false);
    }

    static public void UpdatePoolByPrefab(GameObject prefab, List<GameObject> gameObjs, int dataCount, Transform parent = null) 
    {
        if (parent == null)
            parent = prefab.transform.parent;

        for (int i = 0; i < dataCount; i++)
        {
            GameObject view;
            if (i >= gameObjs.Count)
            {
                view = GameObject.Instantiate<GameObject>(prefab, parent);
                gameObjs.Add(view);
            }
            else
                view = gameObjs[i];

            view.gameObject.SetActive(true);
        }

        for (int i = dataCount; i < gameObjs.Count; i++)
            gameObjs[i].gameObject.SetActive(false);
    }
}
