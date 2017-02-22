using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DailyWeather
{
    [CustomEditor(typeof(UI_AnimationTrigger))]
    public class UIReporterEditor : Editor {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();

            SerializedProperty triggers = serializedObject.FindProperty("triggers");
            string[] names = System.Enum.GetNames(typeof(Weathers));
            while (triggers.arraySize < names.Length)
            {
                int index = triggers.arraySize;
                triggers.InsertArrayElementAtIndex(index);
                triggers.GetArrayElementAtIndex(index).stringValue = names[index];
            }

            triggers.isExpanded = EditorGUILayout.Foldout(triggers.isExpanded, "Weather Triggers");
            if (triggers.isExpanded)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < names.Length; i++)
                {
                    EditorGUILayout.PropertyField(triggers.GetArrayElementAtIndex(i), new GUIContent(names[i]));
                }
                EditorGUI.indentLevel--;
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        
    }
}