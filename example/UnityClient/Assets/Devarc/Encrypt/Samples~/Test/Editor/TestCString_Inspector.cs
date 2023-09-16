using UnityEngine;
using UnityEditor;
using System.Text;
using Devarc;

[CustomEditor(typeof(TestCString))]
public class TestCString_Inspector : Editor
{
    SerializedProperty data;

    void OnEnable()
    {
        data = serializedObject.FindProperty("value").FindPropertyRelative("data");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        serializedObject.Update();
        EditorGUILayout.LabelField("CString::data", data.stringValue);
    }
}
