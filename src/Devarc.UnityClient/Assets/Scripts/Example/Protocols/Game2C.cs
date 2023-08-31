using System.Collections.Generic;
using MessagePack;
using Devarc;

namespace Game2C
{
	[MessagePackObject]
	public class NotifyLogin
	{
		[Key(0)]
		public ErrorType            errorCode;
		[Key(1)]
		public Character            character = new Character();
	}

};
