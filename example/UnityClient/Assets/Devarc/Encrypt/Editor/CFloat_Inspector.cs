using UnityEngine;
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
            SerializedProperty propData1 = property.FindPropertyRelative("data1");
            SerializedProperty propData2 = property.FindPropertyRelative("data2");

            if (script.IsValid == false)
            {
                for (int i = 0; i < propData1.arraySize; i++)
                {
                    data1[i] = (byte)propData1.GetArrayElementAtIndex(i).intValue;
                }
                for (int i = 0; i < propData2.arraySize; i++)
                {
                    data2[i] = (byte)propData2.GetArrayElementAtIndex(i).intValue;
                }
                script.Init(data1, data2);
            }

            float prevValue = script.Value;
            float nextValue = EditorGUI.FloatField(position, label, prevValue);
            if (prevValue != nextValue)
            {
                script = nextValue;
                for (int i = 0; i < propData1.arraySize; i++)
                {
                    propData1.GetArrayElementAtIndex(i).intValue = script.data1[i];
                }
                for (int i = 0; i < propData2.arraySize; i++)
                {
                    propData2.GetArrayElementAtIndex(i).intValue = script.data2[i];
                }
            }
        }
    }
}

