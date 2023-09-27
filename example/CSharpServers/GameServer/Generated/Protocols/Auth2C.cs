using System.Collections.Generic;
using MessagePack;
using Devarc;

namespace Auth2C
{
	[MessagePackObject]
	public class NotifyLogin
	{
		[Key(0)]
		public ErrorType            errorCode;
		[Key(1)]
		public string               sessionID;
		[Key(2)]
		public int                  secret;
	}

	[MessagePackObject]
	public class NotifyLogout
	{
		[Key(0)]
		public ErrorType            errorCode;
	}

};
