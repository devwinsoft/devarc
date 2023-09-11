using System.Collections.Generic;
using MessagePack;
using Devarc;

namespace C2Game
{
	[MessagePackObject]
	public class RequestLogin
	{
		[Key(0)]
		public string               sessionID;
		[Key(1)]
		public int                  secret;
	}

};
