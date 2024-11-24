using System.IO;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using MessagePack;

namespace Devarc
{
    public class _LString_TABLE : TableData<LString, _LString, string>
    {
        public _LString_TABLE()
        {
            TableManager.RegisterLoadStringBin("LString", (data, options) =>
            {
                LoadBin(data, options);
            });
            TableManager.RegisterLoadStringJson("LString", (textAsset) =>
            {
                LoadJson(textAsset.text);
            });
            TableManager.RegisterSaveString("LString", (textAsset, isBundle, lang) =>
            {
                SaveBin(textAsset, isBundle, lang);
            });
            TableManager.RegisterUnloadString("LString", () =>
            {
                Clear();
            });
        }
        public void LoadBin(byte[] data, MessagePackSerializerOptions options)
        {
            InitLoad(data);
            int count = ReadInt();
            for (int i = 0; i < count; i++)
            {
                int size = ReadInt();
                var temp = ReadBytes(size);
                var obj = MessagePackSerializer.Deserialize<LString>(temp, options);
                Add(obj.GetKey(), obj);
            }
        }
        public void SaveBin(TextAsset textAsset, bool isBundle, SystemLanguage lang)
        {
#if UNITY_EDITOR
            Clear();
            LoadJson(textAsset.text);
            var saveAsset = new TextAsset(Convert.ToBase64String(GetBytes()));
            var filePath = Path.Combine(DEV_Settings.GetStringPath(lang, isBundle, TableFormatType.BIN), "LString.asset");
            AssetDatabase.CreateAsset(saveAsset, filePath);
#endif
        }
    }
    public partial class Table
	{
		public static _LString_TABLE LString = new _LString_TABLE();
	}

	[System.Serializable]
	public class STRING_ID
	{
		public string Value = string.Empty;
		public static implicit operator string(STRING_ID obj)
		{
			if (obj == null) return string.Empty;
			return obj.Value;
		}
		public static implicit operator STRING_ID(string value)
		{
			STRING_ID obj = new STRING_ID();
			obj.Value = value;
			return obj;
		}
	}

	public static class StringTable_Extension
	{
		public static bool IsValid(this STRING_ID obj)
		{
			return obj != null && !string.IsNullOrEmpty(obj.Value);
		}
	}
}
