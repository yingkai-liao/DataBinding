using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Joybrick
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DataBindAttribute : Attribute
    {
        public string name;
        public DataBindAttribute(string name)
        {
            this.name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class DataContainerAttribute : Attribute
    {
        public string name;
        public DataContainerAttribute(string name)
        {
            this.name = name;
        }
    }

    public interface IDynamicDataContainer
    {
        object GetDataContainer(string key);
        object GetBinding(string key);
    }

    public interface IDataContainer
    {
    }
}