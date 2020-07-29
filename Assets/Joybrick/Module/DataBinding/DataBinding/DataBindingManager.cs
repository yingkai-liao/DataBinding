using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using Cysharp.Threading.Tasks;
using UniRx;
using System.Linq;

namespace Joybrick
{
    public class DataBindingManager
    {
        public static DataBindingManager Instance;
        public Dictionary<string, DataBindCollection> dataBinding = new Dictionary<string, DataBindCollection>();

        Dictionary<string, DataBindPair> _cachedPath = new Dictionary<string, DataBindPair>();

        static char[] split = new char[] { '.' };
        object locker = new object();

        public DataBindingManager()
        {
            if (Instance == null)
                Instance = this;
        }

        public DataBindPair GetDataPair(string key)
        {
            if(_cachedPath.TryGetValue(key,out var result ))
                return result;

            lock (locker)
            {
                if (key.StartsWith(".") || key.EndsWith(".")) //非法請求
                    return null;

                string[] splitResult = key.Split(split);
                int providerDeep = splitResult.Length - 1;
                DataBindCollection collection = GetCollection(splitResult[0]);

                for (int i = 1; i < providerDeep; i++)
                {
                    string name = splitResult[i];
                    collection = collection.GetCollect(name);
                }
                result = collection.GetBinding(splitResult.Last());
                _cachedPath.Add(key, result);
                return result;
            }
        }

        public DataBindCollection GetCollection(string collectionName)
        {
            if (!dataBinding.TryGetValue(collectionName, out var newProvider))
            {
                newProvider = new DataBindCollection();
                dataBinding[collectionName] = newProvider;
            }
            return newProvider;
        }

        public IDisposable Subscribe(string path, Action<object> callback)
        {
            return GetDataPair(path)?.Subscribe(callback);
        }

        public void SetVarable(string path, object source)
        {
            GetDataPair(path).SetSource(source);
        }

        public void SetDataContainer(string collectionName, object providerInstance)
        {
            lock (locker)
            {
                if (collectionName.StartsWith(".") || collectionName.EndsWith(".")) //非法請求
                    return;

                string[] splitResult = collectionName.Split(split);
                int providerDeep = splitResult.Length;
                DataBindCollection collection = GetCollection(splitResult[0]);
                for (int i = 1; i < providerDeep; i++)
                {
                    string name = splitResult[i];
                    collection = collection.GetCollect(name);
                }

                collection.SetDataSource(providerInstance);
            }
        }
    }
}