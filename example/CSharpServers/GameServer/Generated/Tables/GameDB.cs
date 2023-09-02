using MessagePack;

namespace Devarc
{
	[System.Serializable]
	[MessagePackObject]
	public class CHARACTER : ITableData<int>
	{
		public int GetKey() { return character_id; }
		[Key(0)]
		public int                  character_id;
		[Key(1)]
		public string               charName;
		[Key(2)]
		public int                  age;
		[Key(3)]
		public GenderType           gender;
	}

	[System.Serializable]
	[MessagePackObject]
	public class SKILL : ITableData<string>
	{
		public string GetKey() { return skill_id; }
		[Key(0)]
		public string               skill_id;
		[Key(1)]
		public string               skillName;
		[Key(2)]
		public int                  level;
		[Key(3)]
		public int                  power;
	}

}
