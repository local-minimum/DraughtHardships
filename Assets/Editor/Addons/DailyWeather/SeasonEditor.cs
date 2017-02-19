using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DailyWeather
{
    [CustomPropertyDrawer(typeof(Season))]
    public class SeasonEditor : PropertyDrawer
    {
        Weathers currentWeather;
        bool showingWeather = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DailyWeather dw = (DailyWeather)property.serializedObject.targetObject;
            Seasons seasonType = (Seasons)property.FindPropertyRelative("seasonType").enumValueIndex;
            SerializedProperty transitions = property.FindPropertyRelative("seasonTransitions");
            SerializedProperty transitionProbabilities = property.FindPropertyRelative("seasonTransitionProbabilities");

            position.height = 20;

            int l = Mathf.Min(transitions.arraySize, transitionProbabilities.arraySize);

            EditorGUI.LabelField(position, "Transition Probabilities");

            position.y += 22;

            EditorGUI.indentLevel++;

            for (int i=0; i<l; i++)
            {
                position = Transition(
                    transitions.GetArrayElementAtIndex(i),
                    transitionProbabilities.GetArrayElementAtIndex(i),
                    i == 0,
                    position);

                position.y += 22;
            }
            EditorGUI.indentLevel--;

            SerializedProperty weathers = property.FindPropertyRelative("weathers");

            position.y += 2;
            float w = position.width;
            float x = position.x;

            currentWeather = (Weathers)EditorGUI.EnumPopup(position, "Weather:", currentWeather);

            position.y += 22;
            showingWeather = false;

            for (int i = 0; i < weathers.arraySize; i++)
            {
                SerializedProperty wProp = weathers.GetArrayElementAtIndex(i);

                if (wProp.FindPropertyRelative("weatherType").enumValueIndex == (int) currentWeather)
                {
                    EditorGUI.PropertyField(position, wProp);
                    showingWeather = true;
                    break;
                }
                    
            }

            if (!showingWeather)
            {
                position.width = 50;
                position.x += 30;
                if (GUI.Button(position, "+ Add"))
                {
                    if (dw.GetSeason(seasonType).AddWeather(currentWeather))
                    {
                        property.serializedObject.ApplyModifiedProperties();
                        
                        Debug.Log("Added");
                    }
                    else
                    {
                        Debug.Log("Not Added");
                    }
                }
                showingWeather = false;
            }

        }

        Rect Transition(SerializedProperty transition, SerializedProperty transitionP, bool selfRef, Rect rect)
        {
            string label = transition.enumDisplayNames[transition.enumValueIndex];
            if (selfRef)
            {
                label = string.Format("Cont. {0}", label);
            }
            if (EditorGUI.PropertyField(rect, transitionP, new GUIContent(label)))
            {
               
            }
            return rect;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            
            return 2 * 22 + property.FindPropertyRelative("seasonTransitions").arraySize * 22 + (showingWeather ? 22 : 24) ;
        }
    }
}
