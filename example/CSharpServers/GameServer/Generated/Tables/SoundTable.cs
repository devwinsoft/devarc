using MessagePack;

namespace Devarc
{
	[System.Serializable]
	public partial class RawTableData_SOUND : RawTableData
	{
		public string               index;
		public string               sound_id;
		public string               path;
		public string               loop;
		public string               volume;

		public virtual int          get_index() => GetInt(index);
		public virtual string       get_sound_id() => (sound_id);
		public virtual string       get_path() => (path);
		public virtual bool         get_loop() => GetBool(loop);
		public virtual float        get_volume() => GetFloat(volume);
	}

	[System.Serializable]
	public partial class _SOUND : RawTableData_SOUND
	{
	}

	[System.Serializable]
	[MessagePackObject]
	public class SOUND : ITableData<_SOUND, int>
	{
		public int GetKey() { return index; }
		[Key(0)]
		public int                  index;
		[Key(1)]
		public string               sound_id;
		[Key(2)]
		public string               path;
		[Key(3)]
		public bool                 loop;
		[Key(4)]
		public float                volume;

		public void Initialize(_SOUND data)
		{
			index = data.get_index();
			sound_id = data.get_sound_id();
			path = data.get_path();
			loop = data.get_loop();
			volume = data.get_volume();
		}
	}

}
