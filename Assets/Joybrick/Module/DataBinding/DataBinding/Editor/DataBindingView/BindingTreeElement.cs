using System;
using UnityEditor.TreeViewExamples;

namespace Joybrick
{
    [Serializable]
    internal class BindingTreeElement : TreeElement
    {
        public string DataSource
        {
            get
            {
                var data = dataPair.GetValue();
                if (data == null) return "";
                return data.ToString();
            }
        }

        public bool hasSource { get { return dataPair.HasSource; } }
        public bool hasObserver { get { return dataPair.HasObservers; } }
        public bool isBindingProperty { get { return dataPair.IsBindingProperty; } }
        public string type { get { return dataPair.source.GetTypeName(); } }

        public DataBindPair dataPair;

        public BindingTreeElement(string name)
        {
            base.name = name;
        }

        public BindingTreeElement(string name, DataBindPair data) : base()
        {
            base.name = name;
            this.dataPair = data;
        }
    }
}
