using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using System;

namespace Joybrick
{
    [CustomEditor(typeof(BindingPathRoot), true)]
    public class BindingRootPathEditor : Editor
    {
        private BindingPathRoot __target;

        public override void OnInspectorGUI()
        {
            if (__target == null)
                __target = (BindingPathRoot)target;

            var parent = __target.transform.parent;
            if (parent != null)
            {
                var collectB = parent.GetComponent<CollectionBinding>();
                if (collectB != null)
                {
                    GUI.enabled = false;
                    base.OnInspectorGUI();
                    GUI.enabled = true;
                    EditorGUILayout.LabelField("RequestText 會由上層的 CollectionBinding 指定 ");
                    if (!EditorApplication.isPlaying)
                    {
                        System.Text.StringBuilder _sb = new System.Text.StringBuilder(collectB.requestText);
                        string root = collectB.basePath == null ? "" : collectB.basePath.requestText;
                        _sb = _sb.Replace("$", root);
                        if (_sb[0] == '{')
                            _sb.Remove(0, 1);
                        if (_sb[_sb.Length - 1] == '}')
                            _sb.Remove(_sb.Length - 1, 1);
                        EditorGUILayout.LabelField("=> " + _sb.ToString());
                        _sb.Append(".0");
                        __target.requestText = _sb.ToString();
                    }
                }
                else
                {
                    base.OnInspectorGUI();
                }
            }
        }
    }
}