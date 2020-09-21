using UnityEditor;
using UnityEngine;

namespace Joybrick
{
    [CustomEditor(typeof(SetBindingVariable), true)]
    public class SetBindingVariableEditor : Editor
    {
        private SetBindingVariable __target;

        public override void OnInspectorGUI()
        {
            __target = (SetBindingVariable)target;
            base.OnInspectorGUI();
            if (GUILayout.Button("Select RequestText Set Target"))
            {
                DataBindingView.GetWindow().Show();
                DataBindingView.OnCopy = (s) => { __target.requestText = s; };
            }
        }
    }


    [CustomEditor(typeof(DeepBindingBehavior), true)]
    public class DeepBindingBehaviorEditor : Editor
    {
        private DeepBindingBehavior __target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (__target == null)
                __target = (DeepBindingBehavior)target;

            if (GUILayout.Button("Select RequestText Listen Target"))
            {
                DataBindingView.GetWindow().Show();
                DataBindingView.OnCopy = (s) => { __target.requestText = "{" + s + "}"; };
            }

            if (__target.variables != null)
            {
                var s = __target.requestText;

                foreach (var item in __target.variables.variable)
                    s = s.Replace($"{{$.{item.name}}}", item.value);

                EditorGUILayout.LabelField("=> " + s);
            }

            if (EditorApplication.isPlaying && __target.deepBinder != null && __target.deepBinder.process != null)
            {
                EditorGUILayout.LabelField("Runtime 解析 : ");
                EditorGUILayout.LabelField(__target.trueRequestText);
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginVertical();
                foreach (var p in __target.deepBinder.process)
                {
                    EditorGUILayout.LabelField(p.request, p.DebugString());
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField(p.tmpResult);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel--;
            }

            if (EditorApplication.isPlaying && GUILayout.Button("Reload"))
                __target.ReBuildTrueRequestPath();
        }
    }
}