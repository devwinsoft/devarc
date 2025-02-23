﻿using UnityEngine;
using UnityEditor;

namespace Devarc
{
    [CustomPropertyDrawer(typeof(CFloat))]
    public class CFloat_Inspector : BasePropertyDrawer
    {
        public CFloat_Inspector()
        {
            DRAW_Type = PropertyDrawType.SKIP_ROOTDRAW;
        }

        CFloat script = new CFloat();
        int[] data1 = new int[4];
        int[] data2 = new int[4];

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return OneLineHeight;
        }

        protected override void OnGUIDrawProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty propData1 = property.FindPropertyRelative("data.save1");
            SerializedProperty propData2 = property.FindPropertyRelative("data.save2");

            script.data.Init(propData1.intValue, propData2.intValue);

            float prevValue = script.GetValue();
            float nextValue = EditorGUI.FloatField(position, label, prevValue);
            if (prevValue != nextValue)
            {
                script = nextValue;
                propData1.intValue = script.data.save1;
                propData2.intValue = script.data.save2;
            }
        }
    }
}

