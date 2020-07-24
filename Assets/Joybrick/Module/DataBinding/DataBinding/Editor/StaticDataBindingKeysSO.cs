using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEditor;
using UnityEditor.TreeViewExamples;

namespace Joybrick
{
    [CreateAssetMenu(fileName = "StaticDataBindingKeys", menuName = "Joybrick/StaticDataBindingKeys")]
    class StaticDataBindingKeysSO : ScriptableObject
    {
        [SerializeField]
        public List<string> staticKeys = new List<string>();

        public void Valid()
        {
            string lastKey = "";
            string[] lastSplit = new string[0];
            for (int i = 0; i < staticKeys.Count; i++)
            {
                var s = staticKeys[i];
                if (string.IsNullOrWhiteSpace(s) || s == lastKey)
                {
                    staticKeys.RemoveAt(i);
                    i--;
                    continue;
                }

                var keyCheck = "";
                var newSplit = s.Split('.');
                for (var part = 0; part < newSplit.Length - 1; part++)
                {
                    if (keyCheck != "") keyCheck += ".";
                    keyCheck += newSplit[part];
                    if (lastSplit.Length <= part || newSplit[part] != lastSplit[part])
                    {
                        staticKeys.Insert(i, keyCheck);
                        i--;
                        break;
                    }
                }
                lastSplit = newSplit;
                lastKey = s;
            }
        }
    }


    [CustomEditor(typeof(StaticDataBindingKeysSO), true)]
    class StaticDataBindingKeysSOEditor : Editor
    {
        [SerializeField]
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Valid"))
            {
                StaticDataBindingKeysSO __target = (StaticDataBindingKeysSO)target;
                __target.Valid();
            }
        }
    }
}