using MessagePack;

namespace Devarc
{
	[System.Serializable]
	[MessagePackObject]
	public class SOUND : ITableData<string>
	{
		public string GetKey() { return sound_id; }
		[Key(0)]
		public string               sound_id;
		[Key(1)]
		public string               path;
		[Key(2)]
		public bool                 loop;
		[Key(3)]
		public float                volume;
		[Key(4)]
		public bool                 bundle;
	}

}
