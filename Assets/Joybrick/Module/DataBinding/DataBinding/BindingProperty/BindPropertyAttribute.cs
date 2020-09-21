using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Joybrick
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class DataBindAttribute : Attribute
    {
        public string name;
        public DataBindAttribute(string name)
        {
            this.name = name;
        }
    }

    public interface IDynamicDataSource
    {
        object GetValue(string key);
    }
}