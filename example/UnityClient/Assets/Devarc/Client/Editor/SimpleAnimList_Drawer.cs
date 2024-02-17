using UnityEngine;
using UnityEditor;

namespace Devarc
{
    [CustomPropertyDrawer(typeof(SimpleAnimList))]
    public class SimpleAnimList_Drawer : PropertyDrawer
    {
        bool mOpened = true;
        int mAnimatorID = 0;
        string[] mAnimList;

        int getLineCount(SerializedProperty propData)
        {
            if (mOpened == false)
                return 1;
            else
                return 2 + propData.arraySize;
        }

        void updateAnimList(RuntimeAnimatorController anim)
        {
            if (anim == null)
            {
                mAnimList = new string[0];
            }
            else
            {
                var clips = anim.animationClips;
                mAnimList = new string[clips.Length];
                for (int i = 0; i < clips.Length; i++)
                {
                    mAnimList[i] = clips[i].name;
                }
            }
        }

        int getAnimIndex(AnimationClip clip)
        {
            if (clip == null)
            {
                return 0;
            }
            for (int i = 1; i < mAnimList.Length; i++)
            {
                if (string.Equals(clip.name, mAnimList[i]))
                    return i;
            }
            return 0;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty propDatas = property.FindPropertyRelative("list");
            float baseHeight = base.GetPropertyHeight(property, label);
            return (float)getLineCount(propDatas) * baseHeight;
        }

        RuntimeAnimatorController findRuntimeController(SerializedProperty property)
        {
            var script = property.serializedObject.targetObject as MonoBehaviour;
            var anim = script.GetComponentInChildren<Animator>();
            if (anim == null)
                anim = script.GetComponentInParent<Animator>();
            if (anim == null)
                anim = script.GetComponentInChildren<Animator>();
            if (anim == null)
                return null;
            return anim.runtimeAnimatorController;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty propDatas = property.FindPropertyRelative("list");

            RuntimeAnimatorController anim = findRuntimeController(property);
            int newAnimatorID = anim != null ? anim.GetInstanceID() : 0;
            if (mAnimList == null || newAnimatorID != mAnimatorID)
            {
                mAnimatorID = newAnimatorID;
                updateAnimList(anim);
            }

            int lineCount = getLineCount(propDatas);
            Rect[] rtLine;
            CustomEditorUtil.SplitRect(position, lineCount, out rtLine);

            mOpened = EditorGUI.Foldout(rtLine[0], mOpened, label);
            if (mOpened && lineCount > 1)
            {
                EditorGUI.indentLevel++;
                int newSize = EditorGUI.IntField(rtLine[1], "Size", propDatas.arraySize);
                if (newSize != propDatas.arraySize)
                {
                    propDatas.arraySize = newSize;
                    return;
                }

                for (int i = 0; i < propDatas.arraySize; i++)
                {
                    SerializedProperty element = propDatas.GetArrayElementAtIndex(i);
                    SerializedProperty propClip = element.FindPropertyRelative("Clip");
                    SerializedProperty propSpeed = element.FindPropertyRelative("Speed");
                    SerializedProperty propRepeat = element.FindPropertyRelative("Repeat");
                    SerializedProperty propFadeTime = element.FindPropertyRelative("FadeTime");

                    Rect rtLabel = rtLine[2 + i];
                    EditorGUI.LabelField(rtLabel, "");

                    Rect lastRect = GUILayoutUtility.GetLastRect();
                    float contentWidth = lastRect.width;
                    Rect rtContent = rtLine[2 + i];
                    float indent = EditorGUIUtility.labelWidth + EditorGUI.indentLevel * 20;
                    Rect[] rects;
                    CustomEditorUtil.SplitRect(rtContent, indent + 1, out rects, 0.6f, 0.1f, 0.2f, 0.1f);

                    if (anim != null)
                    {
                        AnimationClip tempClip = propClip.objectReferenceValue as AnimationClip;
                        string clipName = tempClip == null ? "none" : tempClip.name;
                        if (GUI.Button(rects[0], new GUIContent(clipName)))
                        {
                            SimpleAnimList_Selector.Instance.Show(anim, propClip, clipName, mAnimList);
                        }
                    }
                    else
                    {
                        if (GUI.Button(rects[0], new GUIContent("null")))
                        {
                        }
                    }

                    propSpeed.floatValue = EditorGUI.FloatField(rects[1], propSpeed.floatValue);
                    propRepeat.intValue = System.Convert.ToInt32(EditorGUI.EnumPopup(rects[2], (ANIM_PLAY_COUNT)propRepeat.intValue));
                    propFadeTime.floatValue = EditorGUI.FloatField(rects[3], propFadeTime.floatValue);
                }
                EditorGUI.indentLevel--;
            }
        }
    }

} // end of namespace
