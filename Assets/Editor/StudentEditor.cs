using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Student))]
public class StudentEditor : Editor
{
    Student myTarget;

    public override void OnInspectorGUI()
    {
        myTarget = (Student)target;
        if (myTarget.stats != null)
        {
            GUILayout.Label("Stats:");
            string[] statNames = System.Enum.GetNames(typeof(studentStats));
            for (int i = 0; i < myTarget.stats.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(statNames[i], GUILayout.Width(100));
                myTarget.stats[i].value = GUILayout.HorizontalSlider(myTarget.stats[i].value, myTarget.stats[i].minValue, myTarget.stats[i].maxValue);
                myTarget.stats[i].value = EditorGUILayout.FloatField(myTarget.stats[i].value, GUILayout.Width(100));
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.Space(10);

        if (myTarget.resources != null)
        {
            GUILayout.Label("Resources:");
            string[] resourceNames = System.Enum.GetNames(typeof(studentResources));
            for (int i = 0; i < myTarget.resources.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(resourceNames[i], GUILayout.Width(100));
                myTarget.resources[i].value = EditorGUILayout.FloatField(myTarget.resources[i].value);
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.Space(10);

        if (myTarget.courses != null)
        {
            GUILayout.Label("Courses:");
            for (int i = 0; i < myTarget.courses.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(myTarget.courses[i].name, GUILayout.Width(150));
                GUILayout.EndHorizontal();
            }
        }
        EditorUtility.SetDirty(target); //to make the inspector continue to update while playing - does not perserve undo states
    }
}
