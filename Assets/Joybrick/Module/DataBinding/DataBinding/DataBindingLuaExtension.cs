using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using Cysharp.Threading.Tasks;
using UniRx;
using System.Linq;
using XLua;
using TinaX.Lua;

namespace Joybrick
{
    public static class DataBindingLuaExtension
    {
        [CSharpCallLua]
        public delegate void LuaCallback(object obj);
        public static Dictionary<LuaCallback, IDisposable> handles = new Dictionary<LuaCallback, IDisposable>();

        public static void LuaSubscribe(string path, LuaCallback callback, LuaBehaviour luaBehaviour)
        {
            var bindValue = DataBindingManager.Instance.GetValue(path);
            if (bindValue != null)
            {
                var result = bindValue.Subscribe(x => callback(x));
                if (luaBehaviour != null)
                    luaBehaviour.m_DisposableGroup.Register(result);
                callback(bindValue.GetValue());
            }
        }

        public static void BindObjectList(string path, List<Dictionary<string, object>> list)
        {
            DataBindingManager.Instance.SetSource(path, list);
        }
    }
}