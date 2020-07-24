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
                if (dp != null)
                {
                    return !dp.HasSource ? "" : dp.GetValue() == null ? "" : dp.GetValue().ToString();
                }
                if(dc != null)
                    return dc.dataSource == null ? "<null>" : dc.dataSource.ToString();
                return null;
            }
        }

        public bool hasSource
        {
            get
            {
                if (dp != null)
                    return dp.HasSource;
                if (dc != null)
                    return dc.isConnected;
                return false;
            }
        }

        public bool hasObserver
        {
            get
            {
                if (dp != null)
                    return dp.HasObservers();
                return false;
            }
        }


        public bool IsData { get { return dp != null; } }

        public DataBindPair dp;
        public DataBindCollection dc;

        public BindingTreeElement(string name)
        {
            base.name = name;
        }

        public BindingTreeElement(string name, DataBindPair data) : base()
        {
            base.name = name;
            dp = data;
        }

        public BindingTreeElement(string name, DataBindCollection data) : base()
        {
            base.name = name;
            dc = data;
        }
    }
}
