using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using System.Reflection;

namespace Joybrick
{
    public class DataBindAttributeHelper
    {
        //static 
        public static Dictionary<Type, DataBindAttributeHelper> reflectionData = new Dictionary<Type, DataBindAttributeHelper>();
        public static object GetValueFromAttribute(object value, string childName)
        {
            if (value == null)
                return null;

            var type = value.GetType();
            return GetAttrInfo(type).GetValue(value, childName);
        }

        public static List<string> GetAllAttribute(object value)
        {
            if (value == null)
                return new List<string>();

            var type = value.GetType();
            return GetAttrInfo(type).GetKeys();
        }

        static DataBindAttributeHelper GetAttrInfo(Type type)
        {
            if (!reflectionData.TryGetValue(type, out var result))
            {
                result = new DataBindAttributeHelper(type);
                reflectionData.Add(type, result);
            }
            return result;
        }

        //instance
        public Dictionary<string, PropertyInfo> Properties = new Dictionary<string, PropertyInfo>();
        public Dictionary<string, FieldInfo> fields = new Dictionary<string, FieldInfo>();
        public Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();

        public DataBindAttributeHelper(Type type)
        {
            ProcessProperties(type);
            ProcessField(type);
            ProcessMethod(type);
        }

        public List<string> GetKeys()
        {
            List<string> result = new List<string>();
            result.AddRange(Properties.Keys);
            result.AddRange(fields.Keys);
            result.AddRange(methods.Keys);
            return result;
        }

        public object GetValue(object source,string key)
        {
            if (Properties.ContainsKey(key))
                return Properties[key].GetValue(source);
            if (fields.ContainsKey(key))
                return fields[key].GetValue(source);
            if (methods.ContainsKey(key))
            {
                Func<object> function = methods[key].CreateDelegate(typeof(Func<object>), source) as Func<object>;
                return function;
            }

            return null;
        }        

        private void ProcessProperties(Type type)
        {
            foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                //是否掛bind
                var bindSettings = p.GetCustomAttributes(typeof(DataBindAttribute), false);
                if (bindSettings.Length > 0)
                {                    
                    var bindName = ((DataBindAttribute)bindSettings[0]).name;
                    Properties.Add(bindName, p);
                }
            }
        }

        private void ProcessField(Type type)
        {
            foreach (var f in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                //是否掛bind
                var bindSettings = f.GetCustomAttributes(typeof(DataBindAttribute), false);
                if (bindSettings.Length > 0)
                {
                    var bindName = ((DataBindAttribute)bindSettings[0]).name;
                    fields.Add(bindName, f);
                }
            }
        }

        private void ProcessMethod(Type type)
        {
            foreach (var m in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                //是否掛provider
                var bindSettings = m.GetCustomAttributes(typeof(DataBindAttribute), false);
                if (bindSettings.Length > 0)
                {
                    var bindName = ((DataBindAttribute)bindSettings[0]).name;
                    methods.Add(bindName, m);
                }
            }
        }
    }
}