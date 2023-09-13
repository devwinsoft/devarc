using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Devarc;


public abstract class EditorID_Selector<T> : ScriptableWizard
{
    public static EditorID_Selector<T> Instance { get { return msInstance; } }
    protected static EditorID_Selector<T> msInstance = null;

    const int COLS = 4;

    SerializedProperty mProperty;
    List<string> mCacheList = new List<string>();
    string[] mList;
    int mSelect = -1;
    string mSearchText = string.Empty;
    Vector2 scrollPosition = Vector2.zero;

    public abstract void Reload();

    private void OnDisable()
    {
        msInstance = null;
    }

    protected void add(string _value)
    {
        if (mCacheList.Contains(_value) == false)
        {
            mCacheList.Add(_value);
        }
    }

    public void Init(string _searchText)
    {
        mSelect = -1;
        if (string.IsNullOrEmpty(mSearchText))
        {
            mList = new string[mCacheList.Count + 1];
            mList[0] = "none";
            var enumer = mCacheList.GetEnumerator();
            for (int i = 1; enumer.MoveNext(); i++)
            {
                var temp = enumer.Current;
                mList[i] = temp;

                if (string.Equals(_searchText, temp))
                {
                    mSelect = i;
                }
            }
        }
        else
        {
            List<string> tmpList = new List<string>();
            int tmpCount = 0;

            var enumer = mCacheList.GetEnumerator();
            while (enumer.MoveNext())
            {
                var temp = enumer.Current;
                if (temp.Contains(mSearchText) == false)
                    continue;
                tmpList.Add(temp);
                if (string.Equals(_searchText, temp))
                {
                    mSelect = tmpCount;
                }
                tmpCount++;
            }

            mList = new string[tmpList.Count + 1];
            mList[0] = "none";
            for (int i = 0; i < tmpList.Count; i++)
            {
                mList[i + 1] = tmpList[i];
            }
        }
    }


    public void Show(SerializedProperty _property)
    {
        minSize = new Vector2(1200, 800);
        mProperty = _property;
        mCacheList.Clear();
    }


    public static void Hide()
    {
        if (msInstance != null)
        {
            msInstance.Close();
            msInstance = null;
        }
    }


    void OnGUI()
    {
        if (mList == null)
        {
            GUI.Label(new Rect(10, 15, 1000, 20), "Loading...");
            return;
        }

        Rect rtEditBox = new Rect(10, 15, 1000, 20);
        string oldText = mSearchText;
        mSearchText = GUI.TextField(rtEditBox, mSearchText);
        if (string.Equals(oldText, mSearchText) == false)
        {
            Init(mSearchText);
        }

        Rect rtScrollContainer = new Rect(0, 50, 1200, 850);
        Rect rtScrollView = new Rect(0, 0, 1160, 34 * ((mList.Length + COLS - 1) / COLS));
        Rect rtButtons = new Rect(10, 0, 1160, 32 * ((mList.Length + COLS - 1) / COLS));

        scrollPosition = GUI.BeginScrollView(rtScrollContainer, scrollPosition, rtScrollView, false, true);
        int curSelect = GUI.SelectionGrid(rtButtons, mSelect, mList, COLS);
        if (curSelect != mSelect)
        {
            mSelect = curSelect;
            if (mProperty != null)
            {
                mProperty.serializedObject.Update();
                string value;
                if (mSelect == 0)
                    value = "";
                else
                    value = mList[mSelect];
                var prop = mProperty.FindPropertyRelative("Value");
                switch (prop.propertyType)
                {
                    case SerializedPropertyType.Integer:
                        {
                            int temp;
                            int.TryParse(value, out temp);
                            prop.intValue = temp;
                        }
                        break;
                    case SerializedPropertyType.Enum:
                        for (int i = 0; i < prop.enumNames.Length; i++)
                        {
                            if (string.Equals(prop.enumNames[i], value))
                            {
                                // TODO:
                                prop.enumValueIndex = i;
                                break;
                            }
                        }
                        break;
                    case SerializedPropertyType.String:
                        {
                            prop.stringValue = value;
                        }
                        break;
                    default:
                        break;
                }
                mProperty.serializedObject.ApplyModifiedProperties();
                Hide();
            }
        }
        GUI.EndScrollView();
    }

}
