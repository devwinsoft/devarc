using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using MessagePack;
namespace Devarc
{
	public class _CHARACTER_TABLE : TableData<CHARACTER, _CHARACTER, int>
	{
		public _CHARACTER_TABLE()
		{
			TableManager.RegisterLoadTableBin("CHARACTER", (data, options) =>
			{
				LoadBin(data, options);
			});
			TableManager.RegisterLoadTableJson("CHARACTER", (textAsset) =>
			{
				LoadJson(textAsset.text);
			});
			TableManager.RegisterSaveTable("CHARACTER", (textAsset, isBundle, lang) =>
			{
				SaveBin(textAsset, isBundle, lang);
			});
			TableManager.RegisterUnloadTable("CHARACTER", () =>
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
				var obj = MessagePackSerializer.Deserialize<CHARACTER>(temp, options);
				Add(obj.GetKey(), obj);
			}
		}
		public void SaveBin(TextAsset textAsset, bool isBundle, SystemLanguage lang)
		{
#if UNITY_EDITOR
			Clear();
			LoadJson(textAsset.text);
			var saveAsset = new TextAsset(Convert.ToBase64String(GetBytes()));
			var filePath = Path.Combine(DEV_Settings.GetTablePath(isBundle, TableFormatType.BIN), "CHARACTER.asset");
			AssetDatabase.CreateAsset(saveAsset, filePath);
#endif
		}
	}
	public partial class Table
	{
		public static _CHARACTER_TABLE CHARACTER = new _CHARACTER_TABLE();
	}

	[System.Serializable]
	public class CHARACTER_ID
	{
		public string Value = string.Empty;
		public static implicit operator string(CHARACTER_ID obj)
		{
			if (obj == null) return string.Empty;
			return obj.Value;
		}
		public static implicit operator CHARACTER_ID(string value)
		{
			CHARACTER_ID obj = new CHARACTER_ID();
			obj.Value = value;
			return obj;
		}
	}

	public static class GameTable_Extension
	{
		public static bool IsValid(this CHARACTER_ID obj)
		{
			return obj != null && !string.IsNullOrEmpty(obj.Value);
		}
	}
}
