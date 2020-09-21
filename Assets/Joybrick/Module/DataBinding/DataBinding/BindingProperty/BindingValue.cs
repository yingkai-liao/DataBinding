using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System;

namespace Joybrick
{
    public interface IBindingProperty : IObservable<object>
    {
        object GetValue();
    }
    
    public class BindingProperty<T> : ReactiveProperty<T>, IBindingProperty
    {
        public BindingProperty() { }
        public BindingProperty(T value) { Value = value; }

        public object GetValue() { return Value; }
        public IDisposable Subscribe(IObserver<object> observer) { return ((ReactiveProperty<T>)this).Subscribe(x => observer.OnNext(x)); }
        public IDisposable Subscribe(Action<T> callback) { return ((ReactiveProperty<T>)this).Subscribe(x => callback(x)); }
        public ReactiveProperty<T> body { get { return ((ReactiveProperty<T>)this); } }
    }

    [Serializable]
    public class StringProperty : BindingProperty<string>
    {
        public StringProperty() { }
        public StringProperty(string value) { Value = value; }
    }

    [Serializable]
    public class BoolProperty : BindingProperty<bool>
    {
        public BoolProperty() { }
        public BoolProperty(bool value) { Value = value; }
    }

    [Serializable]
    public class IntProperty : BindingProperty<int>
    {
        public IntProperty() { }
        public IntProperty(int value) { Value = value; }
    }

    [Serializable]
    public class FloatProperty : BindingProperty<float>
    {
        public FloatProperty() { }
        public FloatProperty(float value) { Value = value; }
    }

    [Serializable]
    public class LongProperty : BindingProperty<long>
    {
        public LongProperty() { }
        public LongProperty(long value) { Value = value; }
    }

    [Serializable]
    public class ListProperty<T> : ReactiveCollection<T>, IDynamicDataSource, IBindingProperty
    {
        Subject<ListProperty<T>> subject = new Subject<ListProperty<T>>();

        public ListProperty(IEnumerable<T> collection) : base(collection)
        {
            init();
        }

        public ListProperty(List<T> list) : base(list)
        {
            init();
        }

        public ListProperty()
        {
            init();
        }

        void init()
        {
            ObserveReplace().Subscribe(x => onChange());
            ObserveRemove().Subscribe(x => onChange());
            ObserveMove().Subscribe(x => onChange());
            ObserveAdd().Subscribe(x => onChange());
        }

        void onChange()
        {
            subject.OnNext(this);
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return subject.Subscribe(observer);
        }

        public object GetValue()
        {
            return this;
        }

        public object GetValue(string key)
        {
            int index = int.Parse(key);
            if (this.Count <= index)
                return null;
            return this[index];
        }
    }

    [Serializable]
    public class DictionaryProperty<TKey,TValue> : ReactiveDictionary<TKey, TValue>, IDynamicDataSource, IBindingProperty
    {
        Subject<DictionaryProperty<TKey, TValue>> subject = new Subject<DictionaryProperty<TKey, TValue>>();

        public DictionaryProperty(IEqualityComparer<TKey> equalityComparer) : base(equalityComparer)
        {
            init();
        }

        public DictionaryProperty(Dictionary<TKey, TValue> data) : base(data)
        {
            init();
        }

        public DictionaryProperty()
        {
            init();
        }

        void init()
        {
            ObserveReset().Subscribe(x => onChange());            
            ObserveReplace().Subscribe(x => onChange());
            ObserveRemove().Subscribe(x => onChange());
            ObserveAdd().Subscribe(x => onChange());
        }

        public object GetValue()
        {
            return this;
        }

        public object GetValue(string key)
        {
            TKey convKey = (TKey)GetKey(key);
            if (this.TryGetValue(convKey, out var value))
                return value;
            return null;
        }

        void onChange()
        {
            subject.OnNext(this);
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return subject.Subscribe(observer);
        }

        private object GetKey(string key)
        {
            var type = typeof(TKey);

            if (type == typeof(string))
                return key;
            else if (type == typeof(int))
                return int.Parse(key);
            else if (type == typeof(float))
                return float.Parse(key);
            else if(type == typeof(long))
                return long.Parse(key);
            else
            {
                foreach (var k in Keys)
                {
                    if (k.ToString() == key)
                        return k;
                }
            }
            return null;
        }
    }
}