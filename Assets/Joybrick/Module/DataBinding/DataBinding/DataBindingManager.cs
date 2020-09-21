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
        static DataBindingManager _Instance;
        public static DataBindingManager Instance { get { if (_Instance == null) _Instance = new DataBindingManager(); return _Instance; } }

        public static bool AutoExposeChildren = false;
        public DataBindPair root = new DataBindPair();
        Dictionary<string, DataBindPair> _cachedPath = new Dictionary<string, DataBindPair>();

        public static char[] split = new char[] { '.' };
        object locker = new object();

        public DataBindingManager()
        {
#if UNITY_EDITOR
            AutoExposeChildren = true;
#endif
        }

    public DataBindPair GetValue(string key)
        {
            if(_cachedPath.TryGetValue(key,out var result ))
                return result;

            lock (locker)
            {
                if (key.StartsWith(".") || key.EndsWith(".")) //非法請求
                    return null;
                
                string[] splitResult = key.Split(split);
                result = root.GetValue(splitResult, 0);
            }

            return result;
        }

        public IDisposable Subscribe(string path, Action<object> callback)
        {
            return GetValue(path)?.Subscribe(callback);
        }

        public void SetSource(string path, object source)
        {
            GetValue(path).SetSource(source);
        }

        public void UpdateChildren(string path)
        {
            GetValue(path).onValueChange();
        }
    }
}