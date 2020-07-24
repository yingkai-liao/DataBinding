using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Joybrick
{
    public class DataBindAttribute : PropertyAttribute
    {
        public string name;
        public DataBindAttribute(string name)
        {
            this.name = name;
        }
    }

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