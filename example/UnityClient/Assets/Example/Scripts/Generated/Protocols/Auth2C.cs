using System.Collections.Generic;
using MessagePack;
using Devarc;

namespace Auth2C
{
	[MessagePackObject]
	public class NotifySession
	{
		[Key(0)]
		public string               sessionID;
		[Key(1)]
		public int                  secret;
		[Key(2)]
		public ErrorType            errorCode;
	}

	[MessagePackObject]
	public class NotifyLogin
	{
		[Key(0)]
		public string               sessionID;
		[Key(1)]
		public int                  secret;
		[Key(2)]
		public ErrorType            errorCode;
	}

	[MessagePackObject]
	public class NotifyLogout
	{
		[Key(0)]
		public ErrorType            errorCode;
	}

	[MessagePackObject]
	public class NotifySignin
	{
		[Key(0)]
		public string               sessionID;
		[Key(1)]
		public int                  secret;
		[Key(2)]
		public ErrorType            errorCode;
	}

	[MessagePackObject]
	public class NotifyError
	{
		[Key(0)]
		public ErrorType            errorCode;
	}

};
