using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(Interactable),true),CanEditMultipleObjects]
public class InteractableEditor : Editor
{
    SerializedProperty inUseProp;
    SerializedProperty oneUseProp;
    SerializedProperty typeProp;

    SerializedProperty PointProp;
    SerializedProperty NameProp;
    SerializedProperty statProp;
    SerializedProperty resourceProp;

    public void OnEnable()
    {
        inUseProp = serializedObject.FindProperty("InUse");
        typeProp = serializedObject.FindProperty("type");
        PointProp = serializedObject.FindProperty("activityPoint");
        ActivityProp();
    }
    private void ActivityProp()
    {
        NameProp = serializedObject.FindProperty("activity.activityName");
        oneUseProp = serializedObject.FindProperty("activity.oneUse");
        statProp = serializedObject.FindProperty("activity.statsDelta");
        resourceProp = serializedObject.FindProperty("activity.resourcesDelta");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (!serializedObject.isEditingMultipleObjects || !inUseProp.hasMultipleDifferentValues)
        {
            statProp.arraySize = System.Enum.GetNames(typeof(studentStats)).Length - 1;
            resourceProp.arraySize = System.Enum.GetNames(typeof(studentResources)).Length - 1;
        
            EditorGUILayout.PropertyField(typeProp);
            EditorGUILayout.PropertyField(inUseProp);

            GUILayout.Space(20);
            EditorGUILayout.PropertyField(NameProp);
            EditorGUILayout.PropertyField(oneUseProp);
            EditorGUILayout.PropertyField(PointProp);

            GUILayout.Label("Stat Deltas:");
            string[] statNames = System.Enum.GetNames(typeof(studentStats));
            for (int i = 0; i < statProp.arraySize; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(statNames[i], GUILayout.Width(100));
                EditorGUILayout.PropertyField(statProp.GetArrayElementAtIndex(i), GUIContent.none);
                GUILayout.EndHorizontal();
            }

            GUILayout.Label("Resource Deltas:");
            string[] resourceNames = System.Enum.GetNames(typeof(studentResources));
            for (int i = 0; i < resourceProp.arraySize; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(resourceNames[i], GUILayout.Width(100));
                EditorGUILayout.PropertyField(resourceProp.GetArrayElementAtIndex(i), GUIContent.none);
                GUILayout.EndHorizontal();
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
