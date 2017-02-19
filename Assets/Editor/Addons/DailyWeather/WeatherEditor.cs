using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DailyWeather
{

    [CustomPropertyDrawer(typeof(Weather))]
    public class WeatherEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20;

            SerializedProperty sProp = property.FindPropertyRelative("anualTemperatureModification");

            EditorGUI.PropertyField(position, sProp, new GUIContent("Temp Mod"), false);

            position.y += (position.height + 2);

            SerializedProperty weathersProp = property.FindPropertyRelative("weathers");
            SerializedProperty transitionProp = property.FindPropertyRelative("weatherTransitions");

            int l = Mathf.Min(weathersProp.arraySize, transitionProp.arraySize);
            EditorGUI.LabelField(position, "Transition Probabilities");

            position.y += position.height + 2;

            EditorGUI.indentLevel++;

            for (int i = 0; i < l; i++)
            {
                position = Transition(
                    weathersProp.GetArrayElementAtIndex(i),
                    transitionProp.GetArrayElementAtIndex(i),
                    i == 0,
                    position);

                position.y += position.height + 2;
            }
            EditorGUI.indentLevel--;


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
            return 22 * 3 + property.FindPropertyRelative("weathers").arraySize * 22;
        }
    }

}