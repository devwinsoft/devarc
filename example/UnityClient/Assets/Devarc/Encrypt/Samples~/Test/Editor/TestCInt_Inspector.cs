using UnityEngine;
using UnityEditor;
using System.Text;
using Devarc;

[CustomEditor(typeof(TestCInt))]
public class TestCInt_Inspector : Editor
{
    SerializedProperty data1;
    SerializedProperty data2;

    void OnEnable()
    {
        data1 = serializedObject.FindProperty("value").FindPropertyRelative("data1");
        data2 = serializedObject.FindProperty("value").FindPropertyRelative("data2");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        serializedObject.Update();
        EditorGUILayout.LabelField("CInt::data1", getEncryptedStr(data1));
        EditorGUILayout.LabelField("CInt::data2", getEncryptedStr(data2));
    }

    string getEncryptedStr(SerializedProperty prop)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < prop.arraySize; i++)
        {
            if (i > 0)
                sb.Append(", ");
            sb.Append(prop.GetArrayElementAtIndex(i).intValue.ToString("x"));
        }
        return sb.ToString();
    }
}
