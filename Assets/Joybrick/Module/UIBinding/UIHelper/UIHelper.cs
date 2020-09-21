using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Joybrick
{
    public static class UIHelper
    {
        public static IAssetLoader assetLoader;

        static public void SetBindingValue(string path, string value)
        {
            DataBindingManager.Instance.SetSource(path, value);
        }

        public static async UniTask LoadIcon(Image image, string path)
        {
            image.enabled = false;

            if (!path.EndsWith(".png"))
                path = $"{path}.png";

            if (assetLoader == null)
            {
                await UniTask.WaitUntil(() => assetLoader != null);
            }

            var sprite = await assetLoader.LoadAsync<Sprite>(path);
            if (sprite == null)
                Debug.LogError($"image not found : {path}");

            image.sprite = sprite;
            image.enabled = true;
        }

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
}