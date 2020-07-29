using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx;
using UnityEngine;

namespace Joybrick
{
    public class DeepBindMember
    {
        public string request;
        public DataBindPair target;
        public IDisposable handle;
        public string tmpResult;

        public object GetValue()
        {
            if (target == null)
                return null;
            return target.GetValue();
        }

        public string GetValueString()
        {
            if (target == null)
                return "";

            var result = target.GetValue();
            return result != null ? result.ToString() : "";
        }

        public string DebugString()
        {
            if (target == null)
                return "<invalid>";

            var result = target.GetValue();
            return result != null ? result.ToString() : "<null>";
        }
    }

    public class DeepBindVarable : IDisposable, IObservable<object>
    {
        public enum ParseResult
        {
            NewVariable, //找到新變數
            NoMoreVariable, //沒有變數了 要求本身不只變數 回應必定為string
            FinalVariable, //整個request最後要求是變數 按binding的type回應
        }

        string request;
        public List<DeepBindMember> process = new List<DeepBindMember>();
        public object Value { get { return result.Value; } }
        public ParseResult parseResult;

        ReactiveProperty<object> result = new ReactiveProperty<object>();
        //cache
        DataBindingManager bindingMgr;
        static StringBuilder _sbText = new StringBuilder();
        static StringBuilder _sbTemp = new StringBuilder();
        static object locker = new object();

        public DeepBindVarable(string request, DataBindingManager bindingMgr)
        {
            this.bindingMgr = bindingMgr;
            SetReuest(request);
        }

        public void SetReuest(string request)
        {
            this.request = request;
            Update();
        }

        void OnChange(object s)
        {
            Update();
        }

        public void Update()
        {
            Dispose();
            lock (locker)
            {
                _sbText.Clear().Append(request);
                int i = 0;
                while (i < 100)
                {
                    var parseResult = ParseOneVariable(_sbText);
                    if (parseResult == ParseResult.NewVariable)
                    {
                        i++;
                        continue;
                    }

                    if (parseResult == ParseResult.FinalVariable)
                        result.SetValueAndForceNotify(process.Last().GetValue());
                    else
                        result.SetValueAndForceNotify(_sbText.ToString());

                    this.parseResult = parseResult;
                    return;
                }
                Debug.LogError("DynamicText resolve error: Too Many Level!");
            }            
        }

        ParseResult ParseOneVariable(StringBuilder text)
        {
            //找到一個{變數}並parse它
            bool append = false;
            int count = text.Length;
            for (int i = 0; i < count; i++)
            {
                var ch = text[i];
                if (ch == '{')
                {
                    _sbTemp.Clear();
                    append = true;
                }
                else if (ch == '}' && append)
                {
                    DeepBindMember newVariable = new DeepBindMember();
                    newVariable.request = _sbTemp.ToString();
                    newVariable.target = bindingMgr.GetDataPair(newVariable.request);
                    newVariable.handle = newVariable.target.Subscribe(OnChange);
                    process.Add(newVariable);

                    _sbTemp.Insert(0, '{');
                    _sbTemp.Append('}');

                    ParseResult result = _sbTemp == text ? ParseResult.FinalVariable : ParseResult.NewVariable;
                    text.Replace(_sbTemp.ToString(), newVariable.GetValueString());
                    newVariable.tmpResult = text.ToString();
                    return result;
                }
                else if (append == true)
                {
                    _sbTemp.Append(ch);
                }
            }

            return ParseResult.NoMoreVariable;
        }

        public void Dispose()
        {
            foreach (var m in process)
                m.handle.Dispose();
            process.Clear();
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return result.Subscribe(observer);
        }
    }
}