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
    Dictionary<string, string> mCacheList = new Dictionary<string, string>();
    string[] mKeyList;
    string[] mDisplayList;
    int mSelect = -1;
    string mSearchText = string.Empty;
    Vector2 scrollPosition = Vector2.zero;

    public abstract void Reload();

    private void OnDisable()
    {
        msInstance = null;
    }

    protected void add(string _value, string _display = null)
    {
        if (mCacheList.ContainsKey(_value) == false)
        {
            if (string.IsNullOrEmpty(_display))
                mCacheList.Add(_value, _value);
            else
                mCacheList.Add(_value, _display);
        }
    }

    protected void add(int _value)
    {
        string temp = _value.ToString();
        if (mCacheList.ContainsKey(temp) == false)
        {
            mCacheList.Add(temp, temp);
        }
    }

    public void Init(string _searchText)
    {
        mSelect = -1;
        if (string.IsNullOrEmpty(mSearchText))
        {
            mKeyList = new string[mCacheList.Count + 1];
            mDisplayList = new string[mCacheList.Count + 1];
            mKeyList[0] = "";
            mDisplayList[0] = "none";
            var enumer = mCacheList.GetEnumerator();
            for (int i = 1; enumer.MoveNext(); i++)
            {
                mKeyList[i] = enumer.Current.Key;
                mDisplayList[i] = enumer.Current.Value;

                if (string.Equals(_searchText, enumer.Current.Key))
                {
                    mSelect = i;
                }
            }
        }
        else
        {
            List<KeyValuePair<string,string>> tmpList = new List<KeyValuePair<string, string>>();
            int tmpCount = 0;

            var enumer = mCacheList.GetEnumerator();
            while (enumer.MoveNext())
            {
                var temp = enumer.Current;
                if (temp.Key.Contains(mSearchText) == false)
                    continue;
                tmpList.Add(new KeyValuePair<string, string>(temp.Key, temp.Value));
                if (string.Equals(_searchText, temp.Key))
                {
                    mSelect = tmpCount;
                }
                tmpCount++;
            }

            mKeyList = new string[tmpList.Count + 1];
            mDisplayList = new string[tmpList.Count + 1];
            mKeyList[0] = "";
            mDisplayList[0] = "none";
            for (int i = 0; i < tmpList.Count; i++)
            {
                mKeyList[i + 1] = tmpList[i].Key;
                mDisplayList[i + 1] = tmpList[i].Value;
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
        if (mDisplayList == null)
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
        Rect rtScrollView = new Rect(0, 0, 1160, 34 * ((mDisplayList.Length + COLS - 1) / COLS));
        Rect rtButtons = new Rect(10, 0, 1160, 32 * ((mDisplayList.Length + COLS - 1) / COLS));

        scrollPosition = GUI.BeginScrollView(rtScrollContainer, scrollPosition, rtScrollView, false, true);
        int curSelect = GUI.SelectionGrid(rtButtons, mSelect, mDisplayList, COLS);
        if (curSelect != mSelect)
        {
            mSelect = curSelect;
            if (mProperty != null)
            {
                mProperty.serializedObject.Update();
                string key;
                if (mSelect == 0)
                    key = "";
                else
                    key = mKeyList[mSelect];
                var prop = mProperty.FindPropertyRelative("Value");
                switch (prop.propertyType)
                {
                    case SerializedPropertyType.Integer:
                        {
                            int temp;
                            int.TryParse(key, out temp);
                            prop.intValue = temp;
                        }
                        break;
                    case SerializedPropertyType.Enum:
                        for (int i = 0; i < prop.enumNames.Length; i++)
                        {
                            if (string.Equals(prop.enumNames[i], key))
                            {
                                // TODO:
                                prop.enumValueIndex = i;
                                break;
                            }
                        }
                        break;
                    case SerializedPropertyType.String:
                        {
                            prop.stringValue = key;
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
