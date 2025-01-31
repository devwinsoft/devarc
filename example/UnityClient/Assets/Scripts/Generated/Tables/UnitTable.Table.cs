using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using MessagePack;
namespace Devarc
{
	public class _UNIT_HERO_TABLE : TableData<UNIT_HERO, _UNIT_HERO, string>
	{
		public _UNIT_HERO_TABLE()
		{
			TableManager.RegisterLoadTableBin("UNIT_HERO", (data, options) =>
			{
				LoadBin(data, options);
			});
			TableManager.RegisterLoadTableJson("UNIT_HERO", (textAsset) =>
			{
				LoadJson(textAsset.text);
			});
			TableManager.RegisterSaveTable("UNIT_HERO", (textAsset, isBundle, lang) =>
			{
				SaveBin(textAsset, isBundle, lang);
			});
			TableManager.RegisterUnloadTable("UNIT_HERO", () =>
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
				var obj = MessagePackSerializer.Deserialize<UNIT_HERO>(temp, options);
				Add(obj.GetKey(), obj);
			}
		}
		public void SaveBin(TextAsset textAsset, bool isBundle, SystemLanguage lang)
		{
#if UNITY_EDITOR
			Clear();
			LoadJson(textAsset.text);
			var saveAsset = new TextAsset(Convert.ToBase64String(GetBytes()));
			var filePath = Path.Combine(DEV_Settings.GetTablePath(isBundle, TableFormatType.BIN), "UNIT_HERO.asset");
			AssetDatabase.CreateAsset(saveAsset, filePath);
#endif
		}
	}
	public class _UNIT_MONSTER_TABLE : TableData<UNIT_MONSTER, _UNIT_MONSTER, string>
	{
		public _UNIT_MONSTER_TABLE()
		{
			TableManager.RegisterLoadTableBin("UNIT_MONSTER", (data, options) =>
			{
				LoadBin(data, options);
			});
			TableManager.RegisterLoadTableJson("UNIT_MONSTER", (textAsset) =>
			{
				LoadJson(textAsset.text);
			});
			TableManager.RegisterSaveTable("UNIT_MONSTER", (textAsset, isBundle, lang) =>
			{
				SaveBin(textAsset, isBundle, lang);
			});
			TableManager.RegisterUnloadTable("UNIT_MONSTER", () =>
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
				var obj = MessagePackSerializer.Deserialize<UNIT_MONSTER>(temp, options);
				Add(obj.GetKey(), obj);
			}
		}
		public void SaveBin(TextAsset textAsset, bool isBundle, SystemLanguage lang)
		{
#if UNITY_EDITOR
			Clear();
			LoadJson(textAsset.text);
			var saveAsset = new TextAsset(Convert.ToBase64String(GetBytes()));
			var filePath = Path.Combine(DEV_Settings.GetTablePath(isBundle, TableFormatType.BIN), "UNIT_MONSTER.asset");
			AssetDatabase.CreateAsset(saveAsset, filePath);
#endif
		}
	}
	public class _UNIT_LEVEL_TABLE : TableData<UNIT_LEVEL, _UNIT_LEVEL, int>
	{
		public _UNIT_LEVEL_TABLE()
		{
			TableManager.RegisterLoadTableBin("UNIT_LEVEL", (data, options) =>
			{
				LoadBin(data, options);
			});
			TableManager.RegisterLoadTableJson("UNIT_LEVEL", (textAsset) =>
			{
				LoadJson(textAsset.text);
			});
			TableManager.RegisterSaveTable("UNIT_LEVEL", (textAsset, isBundle, lang) =>
			{
				SaveBin(textAsset, isBundle, lang);
			});
			TableManager.RegisterUnloadTable("UNIT_LEVEL", () =>
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
				var obj = MessagePackSerializer.Deserialize<UNIT_LEVEL>(temp, options);
				Add(obj.GetKey(), obj);
			}
		}
		public void SaveBin(TextAsset textAsset, bool isBundle, SystemLanguage lang)
		{
#if UNITY_EDITOR
			Clear();
			LoadJson(textAsset.text);
			var saveAsset = new TextAsset(Convert.ToBase64String(GetBytes()));
			var filePath = Path.Combine(DEV_Settings.GetTablePath(isBundle, TableFormatType.BIN), "UNIT_LEVEL.asset");
			AssetDatabase.CreateAsset(saveAsset, filePath);
#endif
		}
	}
	public partial class Table
	{
		public static _UNIT_HERO_TABLE UNIT_HERO = new _UNIT_HERO_TABLE();
		public static _UNIT_MONSTER_TABLE UNIT_MONSTER = new _UNIT_MONSTER_TABLE();
		public static _UNIT_LEVEL_TABLE UNIT_LEVEL = new _UNIT_LEVEL_TABLE();
	}

	[System.Serializable]
	public class UNIT_HERO_ID
	{
		public string Value = string.Empty;
		public static implicit operator string(UNIT_HERO_ID obj)
		{
			if (obj == null) return string.Empty;
			return obj.Value;
		}
		public static implicit operator UNIT_HERO_ID(string value)
		{
			UNIT_HERO_ID obj = new UNIT_HERO_ID();
			obj.Value = value;
			return obj;
		}
	}

	[System.Serializable]
	public class UNIT_MONSTER_ID
	{
		public string Value = string.Empty;
		public static implicit operator string(UNIT_MONSTER_ID obj)
		{
			if (obj == null) return string.Empty;
			return obj.Value;
		}
		public static implicit operator UNIT_MONSTER_ID(string value)
		{
			UNIT_MONSTER_ID obj = new UNIT_MONSTER_ID();
			obj.Value = value;
			return obj;
		}
	}

	[System.Serializable]
	public class UNIT_LEVEL_ID
	{
		public string Value = string.Empty;
		public static implicit operator string(UNIT_LEVEL_ID obj)
		{
			if (obj == null) return string.Empty;
			return obj.Value;
		}
		public static implicit operator UNIT_LEVEL_ID(string value)
		{
			UNIT_LEVEL_ID obj = new UNIT_LEVEL_ID();
			obj.Value = value;
			return obj;
		}
	}

	public static class UnitTable_Extension
	{
		public static bool IsValid(this UNIT_HERO_ID obj)
		{
			return obj != null && !string.IsNullOrEmpty(obj.Value);
		}
		public static bool IsValid(this UNIT_MONSTER_ID obj)
		{
			return obj != null && !string.IsNullOrEmpty(obj.Value);
		}
		public static bool IsValid(this UNIT_LEVEL_ID obj)
		{
			return obj != null && !string.IsNullOrEmpty(obj.Value);
		}
	}
}
