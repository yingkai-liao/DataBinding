using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using System.Reflection;
using UnityEngine;

namespace Joybrick
{
    public class BindDataSource
    {
        public DataBindPair owner;
        //source        
        public object data; // origin data        

        //GET
        IDisposable bindingHandler;
        IBindingProperty bindingProperty;
        public bool IsBindingProperty { get { return bindingProperty != null; } }

        //CHILDREN
        Array array;
        IList list;
        IDictionary dictionary;        
        IDynamicDataSource dynamicData;

        public void SetSource(object source)
        {
            data = source;

            bindingProperty = data as IBindingProperty;
            bindingHandler = bindingProperty?.Subscribe(OnValueChange);            

            array = data as Array;
            dynamicData = data as IDynamicDataSource;
            list = data as IList;
            dictionary = data as IDictionary;            
        }

        public string GetTypeName()
        {
            if (data == null) return "";
            return data.GetType().ToString();
        }

        public List<string> GetAllAttributeName()
        {
            var result = DataBindAttributeHelper.GetAllAttribute(GetValue());
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                    result.Add(i.ToString());
            }
            if (dictionary != null)
            {
                foreach(var key in dictionary.Keys)
                {
                    result.Add(key.ToString());
                }                
            }
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                    result.Add(i.ToString());
            }
            return result;
        }

        public void OnValueChange(object data)
        {
            owner.onValueChange();
        }

        public object GetValue()
        {
            if (bindingProperty != null)
                return bindingProperty.GetValue();

            return data;
        }

        public object GetChildValue(string name)
        {
            if (data == null)
                return null;

            if (dynamicData != null)
                return dynamicData.GetValue(name);
            if (list != null)
                return GetFromList(list, name);
            if (dictionary != null)
                return GetFromDictionary(dictionary, name);
            if (array != null)
                return GetFromArray(array, name);

            return DataBindAttributeHelper.GetValueFromAttribute(GetValue(), name);
        }

        private static object GetFromDictionary(IDictionary dictionarySource, string name)
        {
            if (dictionarySource.Contains(name))
                return dictionarySource[name];

            if (int.TryParse(name, out int index))
            {
                if (dictionarySource.Contains(index))
                    return dictionarySource[index];
            }

            if (dictionarySource.Contains(name))
                return dictionarySource[name];

            return null;
        }

        private static object GetFromList(IList listSource, string name)
        {
            if (int.TryParse(name, out int index))
            {
                if (index >= listSource.Count || index < 0)
                    return null;
                return listSource[index];
            }
            return null;
        }

        private static object GetFromArray(Array arraySource, string name)
        {            
            if (int.TryParse(name, out int index))
            {
                if (index >= arraySource.Length)
                    return null;
                return arraySource.GetValue(index);
            }
            return null;
        }
    }
}