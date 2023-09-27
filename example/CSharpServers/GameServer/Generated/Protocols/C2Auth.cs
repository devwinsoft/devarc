using System.Collections.Generic;
using MessagePack;
using Devarc;

namespace C2Auth
{
	[MessagePackObject]
	public class RequestLogin
	{
		[Key(0)]
		public string               accountID;
		[Key(1)]
		public string               password;
	}

	[MessagePackObject]
	public class RequestLogout
	{
	}

};
