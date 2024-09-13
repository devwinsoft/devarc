using UnityEngine;
using UnityEditor;

namespace Devarc
{
    [CustomPropertyDrawer(typeof(SInt))]
    public class SInt_Inspector : BasePropertyDrawer
    {
        public SInt_Inspector()
        {
            DRAW_Type = PropertyDrawType.SKIP_ROOTDRAW;
        }

        SInt script = new SInt();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return OneLineHeight;
        }

        protected override void OnGUIDrawProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty propData1 = property.FindPropertyRelative("save1");
            SerializedProperty propData2 = property.FindPropertyRelative("save2");

            script.LoadData(propData1.intValue, propData2.intValue);

            int prevValue = script.Value;
            int nextValue = EditorGUI.IntField(position, label, prevValue);
            if (prevValue != nextValue)
            {
                script = nextValue;
                propData1.intValue = script.save1;
                propData2.intValue = script.save2;
            }
        }
    }
}

