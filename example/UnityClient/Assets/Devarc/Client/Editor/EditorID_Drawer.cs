using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class EditorID_Drawer<T> : PropertyDrawer
{
    protected abstract EditorID_Selector<T> getSelector();


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var prop = property.FindPropertyRelative("Value");
        string value = string.Empty;
        switch (prop.propertyType)
        {
            case SerializedPropertyType.Integer:
                value = prop.intValue.ToString();
                break;
            case SerializedPropertyType.Enum:
                value = prop.enumNames[prop.enumValueIndex];
                break;
            case SerializedPropertyType.String:
                value = prop.stringValue;
                break;
            default:
                break;
        }

        EditorGUI.LabelField(position, label);

        if (GUI.Button(CustomEditorUtil.SplitRect(position, 0.35f), value.ToString()))
        {
            show(property, value);
        }
        EditorGUI.EndProperty();
    }


    void show(SerializedProperty property, string serchText)
    {
        var selector = getSelector();

        selector.Show(property);
        selector.Reload();
        selector.Init(serchText);
    }
}
