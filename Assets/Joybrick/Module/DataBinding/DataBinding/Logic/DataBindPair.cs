using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using UniRx;
using System.Linq;

namespace Joybrick
{
    public class DataBindPair : IObservable<object>
    {
        public string key { get; private set; }
        IBindingProperty sourceBindingProperty;
        IDisposable bindHandler;
        Subject<object> onChange = new Subject<object>();
        object staticValue;

        public DataBindPair(string key)
        {
            this.key = key;
        }

        public void SetSource(object source)
        {
            if (source == null && !HasSource)
                return;

            if (bindHandler != null)
                bindHandler.Dispose();

            this.sourceBindingProperty = null;
            staticValue = null;

            if (source is IBindingProperty)
                this.sourceBindingProperty = (IBindingProperty)source;
            else
                staticValue = source;

            if (this.sourceBindingProperty != null)
                bindHandler = this.sourceBindingProperty.Subscribe(onChange);

            onChange.OnNext(GetValue());
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            var handle = onChange.Subscribe(observer);
            return handle;
        }

        public object GetValue()
        {
            if (sourceBindingProperty != null)
                return sourceBindingProperty.GetValue();
            return staticValue;
        }

        public bool HasObservers()
        {
            return onChange.HasObservers;
        }

        public bool HasSource { get { return sourceBindingProperty != null || staticValue != null; } }
        public bool IsBindingValue { get { return sourceBindingProperty != null; } }
    }
}