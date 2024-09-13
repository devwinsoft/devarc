using UnityEngine;
using UnityEditor;

namespace Devarc
{
    [CustomPropertyDrawer(typeof(SFloat))]
    public class SFloat_Inspector : BasePropertyDrawer
    {
        public SFloat_Inspector()
        {
            DRAW_Type = PropertyDrawType.SKIP_ROOTDRAW;
        }

        SFloat script = new SFloat();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return OneLineHeight;
        }

        protected override void OnGUIDrawProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty propData1 = property.FindPropertyRelative("save1");
            SerializedProperty propData2 = property.FindPropertyRelative("save2");

            script.LoadData(propData1.intValue, propData2.intValue);

            float prevValue = script.Value;
            float nextValue = EditorGUI.FloatField(position, label, prevValue);
            if (prevValue != nextValue)
            {
                script = nextValue;
                propData1.intValue = script.save1;
                propData2.intValue = script.save2;
            }
        }
    }
}

