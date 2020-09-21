using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using System.Reflection;
using Cysharp.Threading.Tasks;

namespace Joybrick
{
    public class DataBindPair : IDisposable, IObservable<object>
    {
        public Dictionary<string, DataBindPair> children = new Dictionary<string, DataBindPair>();
        public BindDataSource source = new BindDataSource();

        Subject<object> onChange = new Subject<object>();

        public bool HasSource { get { return source.data != null; } }
        public bool HasObservers { get { return onChange.HasObservers; } }
        public bool IsBindingProperty { get { return source.IsBindingProperty; } }

        public DataBindPair()
        {
            source.owner = this;
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            
            var handle = onChange.Subscribe(observer);
            return handle;
        }

        public object GetValue()
        {
            return source.GetValue();
        }

        public DataBindPair GetValue(string path)
        {
            string[] splitResult = path.Split(DataBindingManager.split);
            var result = GetValue(splitResult, 0);
            return result;
        }

        public DataBindPair GetValue(string[] pathList, int currentDepth = 0)
        {
            if (currentDepth == pathList.Length)
                return this;

            var name = pathList[currentDepth];            
            return GetChild(name).GetValue(pathList, currentDepth + 1);
        }

        DataBindPair GetChild(string name)
        {
            if (!children.TryGetValue(name, out var result))
            {
                result = new DataBindPair();
                children.Add(name, result);
                result.SetSource(source.GetChildValue(name));
            }
            return result;
        }

        public void SetSource(object dataSource)
        {
            Dispose();

            source.SetSource(dataSource);
            onValueChange();
        }

        public void Dispose()
        {
            foreach (var child in children)
                child.Value.Dispose();

            source.SetSource(null);
        }

        private void UpdateUnbindData()
        {
            foreach (var child in children)
            {
                var name = child.Key;
                var value = child.Value;
                value.SetSource(source.GetChildValue(name));               
            }
        }

        public void onValueChange()
        {
            foreach (var child in children)
                child.Value.Dispose();

            if(DataBindingManager.AutoExposeChildren)
            {
                var allAttributes = source.GetAllAttributeName();
                foreach (var attr in allAttributes)
                {
                    if (children.ContainsKey(attr))
                        continue;
                    children.Add(attr, new DataBindPair());
                }
            }
            
            UpdateUnbindData();
            onChange.OnNext(GetValue());
        }
    }    
}