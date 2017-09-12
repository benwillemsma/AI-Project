using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(CourseAttribute))]
public class CourseDrawer : PropertyDrawer
{
    public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(pos, label, property);
        {
            EditorGUI.PropertyField(pos, property.FindPropertyRelative("name"), new GUIContent("CurrentCourse"));
        }
        EditorGUI.EndProperty();
    }
}
