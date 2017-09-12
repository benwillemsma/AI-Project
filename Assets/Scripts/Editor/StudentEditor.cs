using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Student)),CanEditMultipleObjects]
public class StudentEditor : Editor
{
    SerializedProperty statsProp;
    SerializedProperty resourcesProp;
    SerializedProperty courseProp;
    SerializedProperty labProp;

    public void OnEnable()
    {
        statsProp = serializedObject.FindProperty("stats");
        resourcesProp  = serializedObject.FindProperty("resources");
        courseProp = serializedObject.FindProperty("currentCourse");
        labProp = serializedObject.FindProperty("currentLab");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        //EditorGUILayout.PropertyField(courseProp);
        EditorGUILayout.PropertyField(labProp);

        GUILayout.Space(10);
        
        if (statsProp.isArray)
        {
            string[] statNames = System.Enum.GetNames(typeof(studentStats));
            statsProp.arraySize = statNames.Length - 1;
            GUILayout.Label("Stats:");
            for (int i = 0; i < statsProp.arraySize; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(statNames[i], GUILayout.Width(160));
                EditorGUILayout.Slider(statsProp.GetArrayElementAtIndex(i), 0, 10, GUIContent.none);
                GUILayout.EndHorizontal();
            }
        }

        if (resourcesProp.isArray)
        {
            string[] resourceNames = System.Enum.GetNames(typeof(studentResources));
            resourcesProp.arraySize = resourceNames.Length - 1;
            GUILayout.Label("Resources:");
            for (int i = 0; i < resourcesProp.arraySize; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(resourceNames[i], GUILayout.Width(160));
                EditorGUILayout.PropertyField(resourcesProp.GetArrayElementAtIndex(i), GUIContent.none);
                GUILayout.EndHorizontal();
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
