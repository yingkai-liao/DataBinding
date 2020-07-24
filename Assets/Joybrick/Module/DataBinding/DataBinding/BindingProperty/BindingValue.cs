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

    [Serializable]
    public class StringBindingValue : StringReactiveProperty, IBindingProperty
    {
        public StringBindingValue() { }
        public StringBindingValue(string value) { Value = value; }

        public object GetValue() { return Value; }
        public IDisposable Subscribe(IObserver<object> observer) { return ((StringReactiveProperty)this).Subscribe(x => observer.OnNext(x)); }
    }

    [Serializable]
    public class BoolBindingValue : BoolReactiveProperty, IBindingProperty
    {
        public BoolBindingValue() { }
        public BoolBindingValue(bool value) { Value = value; }
        public object GetValue() { return Value; }
        public IDisposable Subscribe(IObserver<object> observer) { return ((BoolReactiveProperty)this).Subscribe(x => observer.OnNext(x)); }
    }

    [Serializable]
    public class IntBindingValue : IntReactiveProperty, IBindingProperty
    {
        public IntBindingValue() { }
        public IntBindingValue(int value) { Value = value; }
        public object GetValue() { return Value; }
        public IDisposable Subscribe(IObserver<object> observer) { return ((IntReactiveProperty)this).Subscribe(x => observer.OnNext(x)); }
    }

    [Serializable]
    public class FloatBindingValue : FloatReactiveProperty, IBindingProperty
    {
        public FloatBindingValue() { }
        public FloatBindingValue(float value) { Value = value; }
        public object GetValue() { return Value; }
        public IDisposable Subscribe(IObserver<object> observer) { return ((FloatReactiveProperty)this).Subscribe(x => observer.OnNext(x)); }
    }

    [Serializable]
    public class LongBindingValue : LongReactiveProperty, IBindingProperty
    {
        public LongBindingValue() { }
        public LongBindingValue(long value) { Value = value; }
        public object GetValue() { return Value; }
        public IDisposable Subscribe(IObserver<object> observer) { return ((LongReactiveProperty)this).Subscribe(x => observer.OnNext(x)); }
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
    }
}