using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using System.Reflection;

namespace Joybrick
{
    public class DataBindCollection : IDisposable
    {
        public Dictionary<string, DataBindCollection> providers = new Dictionary<string, DataBindCollection>();
        public Dictionary<string, DataBindPair> dataPairs = new Dictionary<string, DataBindPair>();
        IDisposable bindingHandle;
        //dynamic data source
        public object dataSource;
        DynamicCollectionSource dynamicSource;
        public bool isConnected = false;

        public bool IsBindingDataSource { get { return bindingHandle != null; } }
        public bool IsDynamicDataSouce { get{ return dynamicSource != null; } }

        public DataBindCollection GetCollect(string name)
        {
            if (!providers.TryGetValue(name, out var provider))
            {
                provider = new DataBindCollection();
                providers.Add(name, provider);

                if (dynamicSource != null)
                    provider.SetDataSource(dynamicSource.GetContainer(name));
            }
            return provider;
        }

        public DataBindPair GetBinding(string name)
        {
            if (!dataPairs.TryGetValue(name, out var bindData))
            {
                bindData = new DataBindPair(name);
                dataPairs.Add(name, bindData);

                if (dynamicSource != null)
                    bindData.SetSource(dynamicSource.GetData(name));
            }

            return bindData;
        }

        public void Dispose()
        {
            foreach (var provider in providers)
                provider.Value.Dispose();

            foreach (var data in dataPairs)
            {
                var v = data.Value;
                data.Value.SetSource(null);
            }

            if (bindingHandle != null)
            {
                bindingHandle.Dispose();
                bindingHandle = null;
            }

            dynamicSource = null;
            dataSource = null;
            isConnected = false;
        }

        public void SetDataSource(object providerInstance)
        {
            Dispose();

            dataSource = providerInstance;
            dynamicSource = DynamicCollectionSource.AssignDynamicSource(providerInstance);
            if (providerInstance == null)
                return;

            isConnected = true;
            var type = providerInstance.GetType();
            if (providerInstance is IBindingProperty)
            {
                bindingHandle = ((IBindingProperty)providerInstance).Subscribe(BindingContainerChange);
            }
            else
            {
                BindingContainerChange(providerInstance);
            }
        }

        private void UpdateUnbindData()
        {
            //if self is a dynamic provider , update all binding date
            if (dynamicSource == null)
                return;

            var dataPairRecord = new List<DataBindPair>(dataPairs.Values);
            foreach (var data in dataPairRecord)
            {
                if (!data.HasSource)    //失聯的孩子 databinding 嘗試幫它重連
                    data.SetSource(dynamicSource.GetData(data.key));
            }

            foreach (var provider in providers)
            {
                var target = provider.Value;
                if (!target.isConnected) //失聯的provider 例如Container是List 
                    target.SetDataSource(dynamicSource.GetContainer(provider.Key));

                //有Connected的部分應該都已經跑過SetDataSource和這支了 可以跳過
            }
        }

        public void BindingContainerChange(object providerInstance)
        {
            foreach (var provider in providers)
                provider.Value.Dispose();

            foreach (var data in dataPairs)
            {
                var v = data.Value;
                data.Value.SetSource(null);
            }

            if (providerInstance != null)
            {
                var type = providerInstance.GetType();
                ProcessProperties(type, providerInstance);
                ProcessField(type, providerInstance);
                ProcessMethod(type, providerInstance);
                UpdateUnbindData();
            }
        }

        private void ProcessProperties(Type type, object providerInstance)
        {
            foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                //是否掛provider
                var providerSettings = p.GetCustomAttributes(typeof(DataContainerAttribute), false);
                if (providerSettings.Length > 0)
                {
                    var value = p.GetValue(providerInstance);
                    if (value == null)
                        continue;
                    var providerName = ((DataContainerAttribute)providerSettings[0]).name;
                    GetCollect(providerName).SetDataSource(value);
                    GetBinding(providerName).SetSource(value);
                    continue;
                }

                //是否掛bind
                var bindSettings = p.GetCustomAttributes(typeof(DataBindAttribute), false);
                if (bindSettings.Length > 0)
                {
                    object target = p.GetValue(providerInstance);
                    if (target == null)
                        continue;
                    var bindName = ((DataBindAttribute)bindSettings[0]).name;
                    GetBinding(bindName).SetSource(target);                    
                }
            }
        }

        private void ProcessField(Type type, object providerInstance)
        {
            foreach (var f in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                //是否掛provider
                var providerSettings = f.GetCustomAttributes(typeof(DataContainerAttribute), false);
                if (providerSettings.Length > 0)
                {
                    var value = f.GetValue(providerInstance);
                    if (value == null)
                        continue;
                    var providerName = ((DataContainerAttribute)providerSettings[0]).name;
                    GetCollect(providerName).SetDataSource(value);
                    GetBinding(providerName).SetSource(value);
                    continue;
                }

                //是否掛bind
                var bindSettings = f.GetCustomAttributes(typeof(DataBindAttribute), false);
                if (bindSettings.Length > 0)
                {
                    object target = f.GetValue(providerInstance);
                    if (target == null)
                        continue;
                    var bindName = ((DataBindAttribute)bindSettings[0]).name;
                    GetBinding(bindName).SetSource(target);
                }
            }
        }

        private void ProcessMethod(Type type, object providerInstance)
        {
            foreach (var m in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                //是否掛provider
                var providerSettings = m.GetCustomAttributes(typeof(DataContainerAttribute), false);
                if (providerSettings.Length > 0)
                {                    
                    var providerName = ((DataContainerAttribute)providerSettings[0]).name;
                    Func<string, object> function = m.CreateDelegate(typeof(Func<string, object>), providerInstance) as Func<string, object>;
                    GetCollect(providerName).SetDataSource(function);
                    return;
                }
            }
        }
    }

    public class DynamicCollectionSource
    {
        IDictionary dictionarySource;
        IList listSource;
        IDynamicDataContainer dynamicDataSource;
        Func<string, object> objectGetter;
        Array array;

        public static DynamicCollectionSource AssignDynamicSource(object providerInstance)
        {
            var dynamicDataSource = providerInstance as IDynamicDataContainer;
            if (dynamicDataSource != null)
                return new DynamicCollectionSource() { dynamicDataSource = dynamicDataSource };

            var listSource = providerInstance as IList;
            if (listSource != null)
                return new DynamicCollectionSource() { listSource = listSource };

            var dictionarySource = providerInstance as IDictionary;
            if (dictionarySource != null)
                return new DynamicCollectionSource() { dictionarySource = dictionarySource };

            var objectGetter = providerInstance as Func<string, object>;
            if (objectGetter != null)
                return new DynamicCollectionSource() { objectGetter = objectGetter };

            var array = providerInstance as Array;
            if(array != null)
                return new DynamicCollectionSource() { array = array };

            return null;
        }

        public object GetContainer(string name)
        {
            if (dynamicDataSource != null)
                return dynamicDataSource.GetDataContainer(name);
            else if (listSource != null)
                return GetFromList(listSource, name);
            else if (dictionarySource != null)
                return GetFromDictionary(dictionarySource, name);
            else if (objectGetter != null)
                return objectGetter(name);
            else if (array != null)
                return GetFromArray(array, name);

            return null;
        }

        public object GetData(string name)
        {
            if (dynamicDataSource != null)
                return dynamicDataSource.GetBinding(name);
            else if (listSource != null)
                return GetFromList(listSource, name);
            else if (dictionarySource != null)
                return GetFromDictionary(dictionarySource, name);
            else if (objectGetter != null)
                return objectGetter(name);
            else if (array != null)
                return GetFromArray(array, name);

            return null;
        }

        private static object GetFromDictionary(IDictionary dictionarySource, string name)
        {
            if (dictionarySource.Contains(name))
                return dictionarySource[name];

            if (int.TryParse(name, out int index))
            {
                if (dictionarySource.Contains(index))
                    return dictionarySource[name];
            }

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