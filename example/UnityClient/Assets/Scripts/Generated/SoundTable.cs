using MessagePack;

namespace Devarc
{
	[System.Serializable]
	public partial class RawTableData_SOUND_BUNDLE : RawTableData
	{
		public string               index;
		public string               sound_id;
		public string               path;
		public string               loop;
		public string               cooltime;
		public string               area_close;
		public string               area_far;

		public virtual int          get_index() => GetInt(index);
		public virtual string       get_sound_id() => (sound_id);
		public virtual string       get_path() => (path);
		public virtual bool         get_loop() => GetBool(loop);
		public virtual float        get_cooltime() => GetFloat(cooltime);
		public virtual float        get_area_close() => GetFloat(area_close);
		public virtual float        get_area_far() => GetFloat(area_far);
	}

	[System.Serializable]
	public partial class _SOUND_BUNDLE : RawTableData_SOUND_BUNDLE
	{
	}

	[System.Serializable]
	[MessagePackObject]
	public partial class SOUND_BUNDLE : ITableData<_SOUND_BUNDLE, int>
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
		public float                cooltime;
		[Key(5)]
		public float                area_close;
		[Key(6)]
		public float                area_far;

		public void Initialize(_SOUND_BUNDLE data)
		{
			index = data.get_index();
			sound_id = data.get_sound_id();
			path = data.get_path();
			loop = data.get_loop();
			cooltime = data.get_cooltime();
			area_close = data.get_area_close();
			area_far = data.get_area_far();
		}
	}

	[System.Serializable]
	public partial class RawTableData_SOUND_RESOURCE : RawTableData
	{
		public string               index;
		public string               key;
		public string               sound_id;
		public string               path;
		public string               loop;
		public string               cooltime;
		public string               area_close;
		public string               area_far;

		public virtual int          get_index() => GetInt(index);
		public virtual string       get_key() => (key);
		public virtual string       get_sound_id() => (sound_id);
		public virtual string       get_path() => (path);
		public virtual bool         get_loop() => GetBool(loop);
		public virtual float        get_cooltime() => GetFloat(cooltime);
		public virtual float        get_area_close() => GetFloat(area_close);
		public virtual float        get_area_far() => GetFloat(area_far);
	}

	[System.Serializable]
	public partial class _SOUND_RESOURCE : RawTableData_SOUND_RESOURCE
	{
	}

	[System.Serializable]
	[MessagePackObject]
	public partial class SOUND_RESOURCE : ITableData<_SOUND_RESOURCE, int>
	{
		public int GetKey() { return index; }
		[Key(0)]
		public int                  index;
		[Key(1)]
		public string               key;
		[Key(2)]
		public string               sound_id;
		[Key(3)]
		public string               path;
		[Key(4)]
		public bool                 loop;
		[Key(5)]
		public float                cooltime;
		[Key(6)]
		public float                area_close;
		[Key(7)]
		public float                area_far;

		public void Initialize(_SOUND_RESOURCE data)
		{
			index = data.get_index();
			key = data.get_key();
			sound_id = data.get_sound_id();
			path = data.get_path();
			loop = data.get_loop();
			cooltime = data.get_cooltime();
			area_close = data.get_area_close();
			area_far = data.get_area_far();
		}
	}

}
