using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using MessagePack;
namespace Devarc
{
	public class _ITEM_EQUIP_TABLE : TableData<ITEM_EQUIP, _ITEM_EQUIP, string>
	{
		public _ITEM_EQUIP_TABLE()
		{
			TableManager.RegisterLoadTableBin("ITEM_EQUIP", (data, options) =>
			{
				LoadBin(data, options);
			});
			TableManager.RegisterLoadTableJson("ITEM_EQUIP", (textAsset) =>
			{
				LoadJson(textAsset.text);
			});
			TableManager.RegisterSaveTable("ITEM_EQUIP", (textAsset, isBundle, lang) =>
			{
				SaveBin(textAsset, isBundle, lang);
			});
			TableManager.RegisterUnloadTable("ITEM_EQUIP", () =>
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
				var obj = MessagePackSerializer.Deserialize<ITEM_EQUIP>(temp, options);
				Add(obj.GetKey(), obj);
			}
		}
		public void SaveBin(TextAsset textAsset, bool isBundle, SystemLanguage lang)
		{
#if UNITY_EDITOR
			Clear();
			LoadJson(textAsset.text);
			var saveAsset = new TextAsset(Convert.ToBase64String(GetBytes()));
			var filePath = Path.Combine(DEV_Settings.GetTablePath(isBundle, TableFormatType.BIN), "ITEM_EQUIP.asset");
			AssetDatabase.CreateAsset(saveAsset, filePath);
#endif
		}
	}
	public class _ITEM_RELIC_TABLE : TableData<ITEM_RELIC, _ITEM_RELIC, string>
	{
		public _ITEM_RELIC_TABLE()
		{
			TableManager.RegisterLoadTableBin("ITEM_RELIC", (data, options) =>
			{
				LoadBin(data, options);
			});
			TableManager.RegisterLoadTableJson("ITEM_RELIC", (textAsset) =>
			{
				LoadJson(textAsset.text);
			});
			TableManager.RegisterSaveTable("ITEM_RELIC", (textAsset, isBundle, lang) =>
			{
				SaveBin(textAsset, isBundle, lang);
			});
			TableManager.RegisterUnloadTable("ITEM_RELIC", () =>
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
				var obj = MessagePackSerializer.Deserialize<ITEM_RELIC>(temp, options);
				Add(obj.GetKey(), obj);
			}
		}
		public void SaveBin(TextAsset textAsset, bool isBundle, SystemLanguage lang)
		{
#if UNITY_EDITOR
			Clear();
			LoadJson(textAsset.text);
			var saveAsset = new TextAsset(Convert.ToBase64String(GetBytes()));
			var filePath = Path.Combine(DEV_Settings.GetTablePath(isBundle, TableFormatType.BIN), "ITEM_RELIC.asset");
			AssetDatabase.CreateAsset(saveAsset, filePath);
#endif
		}
	}
	public class _ITEM_MATERIAL_TABLE : TableData<ITEM_MATERIAL, _ITEM_MATERIAL, string>
	{
		public _ITEM_MATERIAL_TABLE()
		{
			TableManager.RegisterLoadTableBin("ITEM_MATERIAL", (data, options) =>
			{
				LoadBin(data, options);
			});
			TableManager.RegisterLoadTableJson("ITEM_MATERIAL", (textAsset) =>
			{
				LoadJson(textAsset.text);
			});
			TableManager.RegisterSaveTable("ITEM_MATERIAL", (textAsset, isBundle, lang) =>
			{
				SaveBin(textAsset, isBundle, lang);
			});
			TableManager.RegisterUnloadTable("ITEM_MATERIAL", () =>
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
				var obj = MessagePackSerializer.Deserialize<ITEM_MATERIAL>(temp, options);
				Add(obj.GetKey(), obj);
			}
		}
		public void SaveBin(TextAsset textAsset, bool isBundle, SystemLanguage lang)
		{
#if UNITY_EDITOR
			Clear();
			LoadJson(textAsset.text);
			var saveAsset = new TextAsset(Convert.ToBase64String(GetBytes()));
			var filePath = Path.Combine(DEV_Settings.GetTablePath(isBundle, TableFormatType.BIN), "ITEM_MATERIAL.asset");
			AssetDatabase.CreateAsset(saveAsset, filePath);
#endif
		}
	}
	public partial class Table
	{
		public static _ITEM_EQUIP_TABLE ITEM_EQUIP = new _ITEM_EQUIP_TABLE();
		public static _ITEM_RELIC_TABLE ITEM_RELIC = new _ITEM_RELIC_TABLE();
		public static _ITEM_MATERIAL_TABLE ITEM_MATERIAL = new _ITEM_MATERIAL_TABLE();
	}

	[System.Serializable]
	public class ITEM_EQUIP_ID
	{
		public string Value = string.Empty;
		public static implicit operator string(ITEM_EQUIP_ID obj)
		{
			if (obj == null) return string.Empty;
			return obj.Value;
		}
		public static implicit operator ITEM_EQUIP_ID(string value)
		{
			ITEM_EQUIP_ID obj = new ITEM_EQUIP_ID();
			obj.Value = value;
			return obj;
		}
	}

	[System.Serializable]
	public class ITEM_RELIC_ID
	{
		public string Value = string.Empty;
		public static implicit operator string(ITEM_RELIC_ID obj)
		{
			if (obj == null) return string.Empty;
			return obj.Value;
		}
		public static implicit operator ITEM_RELIC_ID(string value)
		{
			ITEM_RELIC_ID obj = new ITEM_RELIC_ID();
			obj.Value = value;
			return obj;
		}
	}

	[System.Serializable]
	public class ITEM_MATERIAL_ID
	{
		public string Value = string.Empty;
		public static implicit operator string(ITEM_MATERIAL_ID obj)
		{
			if (obj == null) return string.Empty;
			return obj.Value;
		}
		public static implicit operator ITEM_MATERIAL_ID(string value)
		{
			ITEM_MATERIAL_ID obj = new ITEM_MATERIAL_ID();
			obj.Value = value;
			return obj;
		}
	}

	public static class ItemTable_Extension
	{
		public static bool IsValid(this ITEM_EQUIP_ID obj)
		{
			return obj != null && !string.IsNullOrEmpty(obj.Value);
		}
		public static bool IsValid(this ITEM_RELIC_ID obj)
		{
			return obj != null && !string.IsNullOrEmpty(obj.Value);
		}
		public static bool IsValid(this ITEM_MATERIAL_ID obj)
		{
			return obj != null && !string.IsNullOrEmpty(obj.Value);
		}
	}
}
