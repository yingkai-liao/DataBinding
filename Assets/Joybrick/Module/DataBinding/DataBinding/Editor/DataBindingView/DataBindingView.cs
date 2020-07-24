using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEditor;
using UnityEditor.TreeViewExamples;

namespace Joybrick
{
    class DataBindingView : EditorWindow
    {
        [NonSerialized] bool m_Initialized;
        [SerializeField] TreeViewState m_TreeViewState; // Serialized in the window layout file so it survives assembly reloading
        [SerializeField] MultiColumnHeaderState m_MultiColumnHeaderState;
        SearchField m_SearchField;
        BindingTreeView m_TreeView;

        List<BindingTreeElement> datas = new List<BindingTreeElement>();
        static StaticDataBindingKeysSO _saveData;

        public static Action<string> OnCopy;

        [MenuItem("Joybrick/DataBinding")]
        public static DataBindingView GetWindow()
        {
            var window = GetWindow<DataBindingView>();
            window.titleContent = new GUIContent("DataBinding");
            window.InitIfNeeded();
            window.Focus();
            window.Repaint();
            return window;
        }

        Rect multiColumnTreeViewRect { get { return new Rect(20, 30, position.width - 40, position.height - 60); } }
        Rect toolbarRect { get { return new Rect(20f, 10f, position.width - 40f, 20f); } }
        Rect bottomToolbarRect { get { return new Rect(20f, position.height - 18f, position.width - 40f, 16f); } }
        public BindingTreeView treeView { get { return m_TreeView; } }

        void InitIfNeeded()
        {
            if (!m_Initialized)
            {
                // Check if it already exists (deserialized from window layout file or scriptable object)                                
                if (m_TreeViewState == null)
                    m_TreeViewState = new TreeViewState();

                UpdateList();
                if (m_TreeView == null)
                    return;
                
                m_SearchField = new SearchField();
                m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;

                m_Initialized = true;
            }
        }

        private void UpdateList()
        {
            datas.Clear();
            datas.Add(new BindingTreeElement("root") { depth = -1, id = -1 });
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                if (_saveData == null)
                {
                    string[] path = AssetDatabase.FindAssets("t:StaticDataBindingKeysSO");
                    if (path.Length == 0) return;
                        _saveData = AssetDatabase.LoadAssetAtPath<StaticDataBindingKeysSO>(AssetDatabase.GUIDToAssetPath(path[0]));
                }
                if (_saveData == null)
                    return;

                var newList = new List<string>(_saveData.staticKeys);
                newList.Sort();
                int i = 0;
                foreach  (var item in newList)
                {
                    var newItem = new BindingTreeElement(item);
                    newItem.id = i++;
                    newItem.depth = item.Split('.').Length - 1;
                    datas.Add(newItem);
                }
            }
            else
            {                
                if (DataBindingManager.Instance == null)
                    return;

                var bindMgr = DataBindingManager.Instance;
                var keys = new List<string>(bindMgr.dataBinding.Keys);
                keys.Sort();                
                foreach (var key in keys)
                {
                    var provider = bindMgr.dataBinding[key];
                    UpdateProvider(key, provider, 0);
                }
            }                   
            

            if (m_TreeView == null)
            {
                var headerState = BindingTreeView.CreateiColumnHeaderState(multiColumnTreeViewRect.width);
                if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderState, headerState))
                    MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderState, headerState);
                m_MultiColumnHeaderState = headerState;

                var multiColumnHeader = new MultiColumnHeader(headerState);
                multiColumnHeader.ResizeToFit();
                var treeModel = new TreeModel<BindingTreeElement>(datas);
                m_TreeView = new BindingTreeView(m_TreeViewState, multiColumnHeader, treeModel);
                m_TreeView.ExpandAll();
            }
            else
            {
                m_TreeView.treeModel.SetData(datas);
                m_TreeView.Reload();
            }
        }

        private void UpdateProvider(string name, DataBindCollection value, int layer, string baseName = "")
        {
            var me = new BindingTreeElement(name, value) { depth = layer, id = datas.Count };
            datas.Add(me);
            string myBase = name + ".";

            var keys = new List<string>(value.providers.Keys).ConvertAll<string[]>(x => new string[] { x, "0" });
            keys.AddRange(new List<string>(value.dataPairs.Keys).ConvertAll<string[]>(x => new string[] { x, "1" }));
            keys.Sort((x,y)=> {
                var result = string.Compare(x[0] , y[0]);
                return result != 0 ? result : string.Compare(x[1], y[1]);
            });
            foreach (var key in keys)
            {
                if (key[1] == "0")
                {
                    var provider = value.providers[key[0]];
                    UpdateProvider(myBase + key[0], provider, layer + 1, myBase);
                }
                else
                {
                    var data = value.dataPairs[key[0]];
                    datas.Add(new BindingTreeElement(myBase + key[0], data) { depth = layer + 1, id = datas.Count });
                }
            }
        }

        void OnGUI()
        {            
            InitIfNeeded();
            if (!m_Initialized)
                return;
            UpdateList();

            SearchBar(toolbarRect);

            DoTreeView(multiColumnTreeViewRect);
            BottomToolBar(bottomToolbarRect);
        }

        void SearchBar(Rect rect)
        {
            treeView.searchString = m_SearchField.OnGUI(rect, treeView.searchString);
        }

        void DoTreeView(Rect rect)
        {
            m_TreeView.OnGUI(rect);
        }

        void BottomToolBar(Rect rect)
        {
            GUILayout.BeginArea(rect);

            using (new EditorGUILayout.HorizontalScope())
            {

                var style = "miniButton";
                if (GUILayout.Button("Expand All", style))
                {
                    treeView.ExpandAll();
                }

                if (GUILayout.Button("Collapse All", style))
                {
                    treeView.CollapseAll();
                }                

                if (EditorApplication.isPlaying)
                {
                    if (GUILayout.Button("Export To Setting", style))
                    {
                        string[] path = AssetDatabase.FindAssets("t:StaticDataBindingKeysSO");
                        var target = AssetDatabase.LoadAssetAtPath<StaticDataBindingKeysSO>(AssetDatabase.GUIDToAssetPath(path[0]));
                        if (path.Length != 0)
                        {
                            Selection.activeObject = target;
                            var adds = datas.ConvertAll<string>(x => x.name);
                            adds.RemoveAt(0); //root
                            target.staticKeys.AddRange(adds);
                            target.Valid();
                        }
                    }
                }
                else
                {
                    if (GUILayout.Button("Select Static Setting", style))
                    {
                        string[] path = AssetDatabase.FindAssets("t:StaticDataBindingKeysSO");
                        if (path.Length != 0)
                        {
                            Selection.activeObject = AssetDatabase.LoadAssetAtPath<StaticDataBindingKeysSO>(AssetDatabase.GUIDToAssetPath(path[0]));
                        }
                    }
                }                

                GUILayout.FlexibleSpace();
                GUILayout.Space(10);
            }

            GUILayout.EndArea();
        }

        private void OnDisable()
        {
            DataBindingView.OnCopy = null;
        }
    }

}
