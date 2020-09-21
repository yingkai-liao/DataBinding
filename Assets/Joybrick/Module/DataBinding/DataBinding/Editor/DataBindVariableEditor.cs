using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using System;
using UnityEditorInternal;

namespace Joybrick
{
    [CustomEditor(typeof(DataBindVariable), true)]
    public class DataBindVariableEditor : Editor
    {
        private DataBindVariable __target;

        public override void OnInspectorGUI()
        {
            if (__target == null)
                __target = (DataBindVariable)target;

            base.OnInspectorGUI();           
        }
    }
}
