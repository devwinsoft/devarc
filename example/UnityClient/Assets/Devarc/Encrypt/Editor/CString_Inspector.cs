using UnityEngine;
using UnityEditor;

namespace Devarc
{
    [CustomPropertyDrawer(typeof(CString))]
    public class CString_Inspector : BasePropertyDrawer
    {
        public CString_Inspector()
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
            SerializedProperty propData = property.FindPropertyRelative("data");
            string prevValue = EncryptUtil.Decrypt_Base64(propData.stringValue);
            string nextValue = EditorGUI.TextField(position, label.text, prevValue);
            if (string.Equals(prevValue, nextValue) == false)
            {
                propData.stringValue = EncryptUtil.Encrypt_Base64(nextValue);
            }
        }
    }
}
