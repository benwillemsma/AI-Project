using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Job), true), CanEditMultipleObjects]
public class JobEditor : InteractableEditor
{
    protected SerializedProperty progressProp;

    protected override void OnEnable()
    {
        base.OnEnable();
        progressProp = serializedObject.FindProperty("progress");
    }
    
    protected override void UpdateInspector()
    {
        base.UpdateInspector();

        GUILayout.Space(20);
        EditorGUILayout.PropertyField(progressProp);
    }
}
