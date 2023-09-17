using UnityEngine;
using UnityEditor;

namespace Devarc
{
    [CustomPropertyDrawer(typeof(CInt))]
    public class CInt_Inspector : BasePropertyDrawer
    {
        public CInt_Inspector()
        {
            DRAW_Type = PropertyDrawType.SKIP_ROOTDRAW;
        }

        CInt script = new CInt();
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
                script.Init(propData1.intValue, propData2.intValue);
            }

            int prevValue = script.Value;
            int nextValue = EditorGUI.IntField(position, label, prevValue);
            if (prevValue != nextValue)
            {
                script = nextValue;
                propData1.intValue = script.data1;
                propData2.intValue = script.data2;
            }
        }
    }
}

