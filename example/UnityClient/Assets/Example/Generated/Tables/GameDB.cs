using MessagePack;

namespace Devarc
{
	[MessagePackObject]
	public class Character
	{
		[Key(0)]
		public int                  index;
		[Key(1)]
		public string               charName;
		[Key(2)]
		public int                  age;
		[Key(3)]
		public GenderType           gender;
	}
}
