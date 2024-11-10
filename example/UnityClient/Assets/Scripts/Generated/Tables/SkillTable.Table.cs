using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using MessagePack;
namespace Devarc
{
	public class _SKILL_TABLE : TableData<SKILL, _SKILL, string>
	{
		public _SKILL_TABLE()
		{
			TableManager.RegisterLoadTableBin("SKILL", (data, options) =>
			{
				LoadBin(data, options);
			});
			TableManager.RegisterLoadTableJson("SKILL", (textAsset) =>
			{
				LoadJson(textAsset.text);
			});
			TableManager.RegisterSaveTable("SKILL", (textAsset, isBundle, lang) =>
			{
				SaveBin(textAsset, isBundle, lang);
			});
			TableManager.RegisterUnloadTable("SKILL", () =>
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
				var obj = MessagePackSerializer.Deserialize<SKILL>(temp, options);
				Add(obj.GetKey(), obj);
			}
		}
		public void SaveBin(TextAsset textAsset, bool isBundle, SystemLanguage lang)
		{
#if UNITY_EDITOR
			Clear();
			LoadJson(textAsset.text);
			var saveAsset = new TextAsset(Convert.ToBase64String(GetBytes()));
			var filePath = Path.Combine(DEV_Settings.GetTablePath(isBundle, TableFormatType.BIN), "SKILL.asset");
			AssetDatabase.CreateAsset(saveAsset, filePath);
#endif
		}
	}
	public class _PROJECTILE_TABLE : TableData<PROJECTILE, _PROJECTILE, string>
	{
		public _PROJECTILE_TABLE()
		{
			TableManager.RegisterLoadTableBin("PROJECTILE", (data, options) =>
			{
				LoadBin(data, options);
			});
			TableManager.RegisterLoadTableJson("PROJECTILE", (textAsset) =>
			{
				LoadJson(textAsset.text);
			});
			TableManager.RegisterSaveTable("PROJECTILE", (textAsset, isBundle, lang) =>
			{
				SaveBin(textAsset, isBundle, lang);
			});
			TableManager.RegisterUnloadTable("PROJECTILE", () =>
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
				var obj = MessagePackSerializer.Deserialize<PROJECTILE>(temp, options);
				Add(obj.GetKey(), obj);
			}
		}
		public void SaveBin(TextAsset textAsset, bool isBundle, SystemLanguage lang)
		{
#if UNITY_EDITOR
			Clear();
			LoadJson(textAsset.text);
			var saveAsset = new TextAsset(Convert.ToBase64String(GetBytes()));
			var filePath = Path.Combine(DEV_Settings.GetTablePath(isBundle, TableFormatType.BIN), "PROJECTILE.asset");
			AssetDatabase.CreateAsset(saveAsset, filePath);
#endif
		}
	}
	public class _AFFECT_TABLE : TableData<AFFECT, _AFFECT, string>
	{
		public _AFFECT_TABLE()
		{
			TableManager.RegisterLoadTableBin("AFFECT", (data, options) =>
			{
				LoadBin(data, options);
			});
			TableManager.RegisterLoadTableJson("AFFECT", (textAsset) =>
			{
				LoadJson(textAsset.text);
			});
			TableManager.RegisterSaveTable("AFFECT", (textAsset, isBundle, lang) =>
			{
				SaveBin(textAsset, isBundle, lang);
			});
			TableManager.RegisterUnloadTable("AFFECT", () =>
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
				var obj = MessagePackSerializer.Deserialize<AFFECT>(temp, options);
				Add(obj.GetKey(), obj);
			}
		}
		public void SaveBin(TextAsset textAsset, bool isBundle, SystemLanguage lang)
		{
#if UNITY_EDITOR
			Clear();
			LoadJson(textAsset.text);
			var saveAsset = new TextAsset(Convert.ToBase64String(GetBytes()));
			var filePath = Path.Combine(DEV_Settings.GetTablePath(isBundle, TableFormatType.BIN), "AFFECT.asset");
			AssetDatabase.CreateAsset(saveAsset, filePath);
#endif
		}
	}
	public partial class Table
	{
		public static _SKILL_TABLE SKILL = new _SKILL_TABLE();
		public static _PROJECTILE_TABLE PROJECTILE = new _PROJECTILE_TABLE();
		public static _AFFECT_TABLE AFFECT = new _AFFECT_TABLE();
	}

	[System.Serializable]
	public class SKILL_ID
	{
		public string Value = string.Empty;
		public static implicit operator string(SKILL_ID obj)
		{
			if (obj == null) return string.Empty;
			return obj.Value;
		}
		public static implicit operator SKILL_ID(string value)
		{
			SKILL_ID obj = new SKILL_ID();
			obj.Value = value;
			return obj;
		}
	}

	[System.Serializable]
	public class PROJECTILE_ID
	{
		public string Value = string.Empty;
		public static implicit operator string(PROJECTILE_ID obj)
		{
			if (obj == null) return string.Empty;
			return obj.Value;
		}
		public static implicit operator PROJECTILE_ID(string value)
		{
			PROJECTILE_ID obj = new PROJECTILE_ID();
			obj.Value = value;
			return obj;
		}
	}

	[System.Serializable]
	public class AFFECT_ID
	{
		public string Value = string.Empty;
		public static implicit operator string(AFFECT_ID obj)
		{
			if (obj == null) return string.Empty;
			return obj.Value;
		}
		public static implicit operator AFFECT_ID(string value)
		{
			AFFECT_ID obj = new AFFECT_ID();
			obj.Value = value;
			return obj;
		}
	}

	public static class SkillTable_Extension
	{
		public static bool IsValid(this SKILL_ID obj)
		{
			return obj != null && !string.IsNullOrEmpty(obj.Value);
		}
		public static bool IsValid(this PROJECTILE_ID obj)
		{
			return obj != null && !string.IsNullOrEmpty(obj.Value);
		}
		public static bool IsValid(this AFFECT_ID obj)
		{
			return obj != null && !string.IsNullOrEmpty(obj.Value);
		}
	}
}
