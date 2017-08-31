using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Student)),CanEditMultipleObjects]
public class StudentEditor : Editor
{
    SerializedProperty statsProp;
    SerializedProperty resourcesProp;

    public void OnEnable()
    {
        statsProp = serializedObject.FindProperty("stats");
        resourcesProp  = serializedObject.FindProperty("resources");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (!serializedObject.isEditingMultipleObjects || !resourcesProp.hasMultipleDifferentValues)
        {
            if (statsProp.isArray)
            {
                statsProp.arraySize = System.Enum.GetNames(typeof(studentStats)).Length - 1;
                GUILayout.Label("Stats:");
                string[] statNames = System.Enum.GetNames(typeof(studentStats));
                for (int i = 0; i < statsProp.arraySize; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label(statNames[i], GUILayout.Width(100));
                    EditorGUILayout.Slider(statsProp.GetArrayElementAtIndex(i), 0, 10, GUIContent.none);
                    GUILayout.EndHorizontal();
                }
            }

            if (resourcesProp.isArray)
            {
                resourcesProp.arraySize = System.Enum.GetNames(typeof(studentResources)).Length - 1;
                GUILayout.Label("Resources:");
                string[] resourceNames = System.Enum.GetNames(typeof(studentResources));
                for (int i = 0; i < resourcesProp.arraySize; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label(resourceNames[i], GUILayout.Width(100));
                    EditorGUILayout.PropertyField(resourcesProp.GetArrayElementAtIndex(i), GUIContent.none);
                    GUILayout.EndHorizontal();
                }
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
