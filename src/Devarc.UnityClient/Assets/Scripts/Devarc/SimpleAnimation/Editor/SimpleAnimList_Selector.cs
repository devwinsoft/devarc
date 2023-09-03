using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SimpleAnimList_Selector : ScriptableWizard
{
    public static SimpleAnimList_Selector Instance
    {
        get
        {
            if (msInstance != null)
            {
                return msInstance;
            }

            msInstance = ScriptableWizard.DisplayWizard<SimpleAnimList_Selector>("Select Animation Clip");
            return msInstance;
        }
    }
    static SimpleAnimList_Selector msInstance;

    const int COLS = 5;
    RuntimeAnimatorController mAnimController;
    SerializedProperty mPropClip;
    List<string> mList = new List<string>();
    string[] mKeys;
    int mSelect = -1;
    string mSearchText = string.Empty;
    Vector2 scrollPosition = Vector2.zero;

    private void OnDisable()
    {
        msInstance = null;
    }

    void OnGUI()
    {
        Rect rtEditBox = new Rect(10, 15, 1000, 20);
        string oldText = mSearchText;
        mSearchText = GUI.TextField(rtEditBox, mSearchText);
        if (string.Equals(oldText, mSearchText) == false)
        {
            Init(string.Empty);
        }

        Rect rtScrollContainer = new Rect(0, 50, 1200, 850);
        Rect rtScrollView = new Rect(0, 0, 1160, 32 * ((mKeys.Length + COLS - 1) / COLS));
        Rect rtButtons = new Rect(10, 0, 1160, 32 * ((mKeys.Length + COLS - 1) / COLS));

        scrollPosition = GUI.BeginScrollView(rtScrollContainer, scrollPosition, rtScrollView, false, true);
        int curSelect = GUI.SelectionGrid(rtButtons, mSelect, mKeys, COLS);
        if (curSelect != mSelect)
        {
            mSelect = curSelect;
            if (mPropClip != null)
            {
                mPropClip.serializedObject.Update();
                string value;
                if (mSelect == 0 && string.IsNullOrEmpty(mSearchText))
                    value = "";
                else
                    value = mKeys[mSelect];

                mPropClip.objectReferenceValue = null;
                foreach (var temp in mAnimController.animationClips)
                {
                    if (string.Equals(temp.name, value))
                    {
                        mPropClip.objectReferenceValue = temp;
                        break;
                    }
                }
                mPropClip.serializedObject.ApplyModifiedProperties();
                msInstance.Close();
                msInstance = null;
            }
        }
        GUI.EndScrollView();
    }

    void Init(string _value)
    {
        mSelect = -1;
        if (string.IsNullOrEmpty(mSearchText))
        {
            mKeys = new string[mList.Count + 1];
            mKeys[0] = "none";
            var enumer = mList.GetEnumerator();
            for (int i = 1; enumer.MoveNext(); i++)
            {
                var temp = enumer.Current;
                mKeys[i] = temp;
                if (string.Equals(_value, temp))
                {
                    mSelect = i;
                }
            }
        }
        else
        {
            List<string> tmpList = new List<string>();
            int tmpCount = 0;

            var enumer = mList.GetEnumerator();
            while (enumer.MoveNext())
            {
                var temp = enumer.Current;
                if (temp.Contains(mSearchText) == false)
                    continue;
                tmpList.Add(temp);
                if (string.Equals(_value, temp))
                {
                    mSelect = tmpCount;
                }
                tmpCount++;
            }

            mKeys = new string[tmpList.Count + 1];
            mKeys[0] = "none";
            for (int i = 0; i < tmpList.Count; i++)
            {
                mKeys[i] = tmpList[i];
            }
        }
    }


    public void Show(RuntimeAnimatorController _anim, SerializedProperty _propClip, string _value, string[] _list)
    {
        minSize = new Vector2(1200, 800);
        mAnimController = _anim;
        mPropClip = _propClip;
        mList.Clear();
        mList.AddRange(_list);
        Init(_value);
    }
}
