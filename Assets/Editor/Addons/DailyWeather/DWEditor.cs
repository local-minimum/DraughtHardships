using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DailyWeather {

    [CustomEditor(typeof(DailyWeather))]
    public class DWEditor : Editor {

        Seasons season = Seasons.Winter;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DailyWeather dw = target as DailyWeather;

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Seasons");

            season = (Seasons)EditorGUILayout.EnumPopup(season);

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            if (dw.HasSeason(season))
            {
                ShowEditSeason();
            } else
            {
                if (AddSeason(dw))
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }

        }

        void ShowEditSeason() {

            SerializedProperty seasons =  serializedObject.FindProperty("seasons");
            int seasonValue = (int)season;

            for (int i=0, l=seasons.arraySize; i<l; i++)
            {
                SerializedProperty seasonProp = seasons.GetArrayElementAtIndex(i);
                if (seasonProp.FindPropertyRelative("seasonType").enumValueIndex == seasonValue)
                {
                    if (EditorGUILayout.PropertyField(seasonProp))
                    {
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }

        bool AddSeason(DailyWeather dw)
        {
            bool ret = false;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+ Add"))
            {
                ret = dw.AddSeason(season);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            return ret;
        }
    }
}
