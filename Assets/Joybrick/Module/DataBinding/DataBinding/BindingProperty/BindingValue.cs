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
    
    public class BindingValue<T> : ReactiveProperty<T>, IBindingProperty
    {
        public BindingValue() { }
        public BindingValue(T value) { Value = value; }

        public object GetValue() { return Value; }
        public IDisposable Subscribe(IObserver<object> observer) { return ((ReactiveProperty<T>)this).Subscribe(x => observer.OnNext(x)); }
        public ReactiveProperty<T> body { get { return ((ReactiveProperty<T>)this); } }
    }

    [Serializable]
    public class StringBindingValue : BindingValue<string>
    {
        public StringBindingValue() { }
        public StringBindingValue(string value) { Value = value; }
    }

    [Serializable]
    public class BoolBindingValue : BindingValue<bool>
    {
        public BoolBindingValue() { }
        public BoolBindingValue(bool value) { Value = value; }
    }

    [Serializable]
    public class IntBindingValue : BindingValue<int>
    {
        public IntBindingValue() { }
        public IntBindingValue(int value) { Value = value; }
    }

    [Serializable]
    public class FloatBindingValue : BindingValue<float>
    {
        public FloatBindingValue() { }
        public FloatBindingValue(float value) { Value = value; }
    }

    [Serializable]
    public class LongBindingValue : BindingValue<long>
    {
        public LongBindingValue() { }
        public LongBindingValue(long value) { Value = value; }
    }

    [Serializable]
    public class ListBindingValue<T> : ReactiveCollection<T>, IDynamicDataContainer, IBindingProperty
    {
        Subject<ListBindingValue<T>> subject = new Subject<ListBindingValue<T>>();

        public ListBindingValue(IEnumerable<T> collection) : base(collection)
        {
            init();
        }

        public ListBindingValue(List<T> list) : base(list)
        {
            init();
        }

        public ListBindingValue()
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


        public object GetBinding(string key)
        {
            int index = int.Parse(key);
            if (this.Count <= index)
                return null;
            return this[index];
        }

        public object GetDataContainer(string key)
        {
            int index = int.Parse(key);
            if (this.Count <= index)
                return null;
            return this[index];
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

        public ReactiveCollection<T> body { get { return ((ReactiveCollection<T>)this); } }
    }
}