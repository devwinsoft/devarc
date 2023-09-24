using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using MessagePack;
namespace Devarc
{
	public class _SOUND_BUNDLE_TABLE : TableData<SOUND_BUNDLE, _SOUND_BUNDLE, int>
	{
		public _SOUND_BUNDLE_TABLE()
		{
			TableManager.RegisterLoadTableBin("SOUND_BUNDLE", (data, options) =>
			{
				LoadBin(data, options);
			});
			TableManager.RegisterLoadTableJson("SOUND_BUNDLE", (textAsset) =>
			{
				LoadJson(textAsset.text);
			});
			TableManager.RegisterSaveTable("SOUND_BUNDLE", (textAsset, isBundle, lang) =>
			{
				SaveBin(textAsset, isBundle, lang);
			});
			TableManager.RegisterUnloadTable("SOUND_BUNDLE", () =>
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
                var obj = MessagePackSerializer.Deserialize<SOUND_BUNDLE>(temp, options);
				Add(obj.GetKey(), obj);
			}
		}
		public void SaveBin(TextAsset textAsset, bool isBundle, SystemLanguage lang)
		{
#if UNITY_EDITOR
			Clear();
			LoadJson(textAsset.text);
			var saveAsset = new TextAsset(Convert.ToBase64String(GetBytes()));
			var filePath = Path.Combine(DEV_Settings.GetTablePath(isBundle, TableFormatType.BIN), "SOUND_BUNDLE.asset");
			AssetDatabase.CreateAsset(saveAsset, filePath);
#endif
		}
	}
	public class _SOUND_RESOURCE_TABLE : TableData<SOUND_RESOURCE, _SOUND_RESOURCE, int>
	{
		public _SOUND_RESOURCE_TABLE()
		{
			TableManager.RegisterLoadTableBin("SOUND_RESOURCE", (data, options) =>
			{
				LoadBin(data, options);
			});
			TableManager.RegisterLoadTableJson("SOUND_RESOURCE", (textAsset) =>
			{
				LoadJson(textAsset.text);
			});
			TableManager.RegisterSaveTable("SOUND_RESOURCE", (textAsset, isBundle, lang) =>
			{
				SaveBin(textAsset, isBundle, lang);
			});
			TableManager.RegisterUnloadTable("SOUND_RESOURCE", () =>
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
				var obj = MessagePackSerializer.Deserialize<SOUND_RESOURCE>(temp, options);
				Add(obj.GetKey(), obj);
			}
		}
		public void SaveBin(TextAsset textAsset, bool isBundle, SystemLanguage lang)
		{
#if UNITY_EDITOR
			Clear();
			LoadJson(textAsset.text);
			var saveAsset = new TextAsset(Convert.ToBase64String(GetBytes()));
			var filePath = Path.Combine(DEV_Settings.GetTablePath(isBundle, TableFormatType.BIN), "SOUND_RESOURCE.asset");
			AssetDatabase.CreateAsset(saveAsset, filePath);
#endif
		}
	}
	public partial class Table
	{
		public static _SOUND_BUNDLE_TABLE SOUND_BUNDLE = new _SOUND_BUNDLE_TABLE();
		public static _SOUND_RESOURCE_TABLE SOUND_RESOURCE = new _SOUND_RESOURCE_TABLE();
	}

	[System.Serializable]
	public class SOUND_BUNDLE_ID
	{
		public string Value = string.Empty;
		public static implicit operator string(SOUND_BUNDLE_ID obj)
		{
			if (obj == null) return string.Empty;
			return obj.Value;
		}
		public static implicit operator SOUND_BUNDLE_ID(string value)
		{
			SOUND_BUNDLE_ID obj = new SOUND_BUNDLE_ID();
			obj.Value = value;
			return obj;
		}
	}

	[System.Serializable]
	public class SOUND_RESOURCE_ID
	{
		public string Value = string.Empty;
		public static implicit operator string(SOUND_RESOURCE_ID obj)
		{
			if (obj == null) return string.Empty;
			return obj.Value;
		}
		public static implicit operator SOUND_RESOURCE_ID(string value)
		{
			SOUND_RESOURCE_ID obj = new SOUND_RESOURCE_ID();
			obj.Value = value;
			return obj;
		}
	}

	public static class SoundTable_Extension
	{
		public static bool IsValid(this SOUND_BUNDLE_ID obj)
		{
			return obj != null && !string.IsNullOrEmpty(obj.Value);
		}
		public static bool IsValid(this SOUND_RESOURCE_ID obj)
		{
			return obj != null && !string.IsNullOrEmpty(obj.Value);
		}
	}
}
