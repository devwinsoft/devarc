using System.Collections.Generic;
using MessagePack;
using Devarc;

namespace C2Auth
{
	[MessagePackObject]
	public class RequestSession
	{
		[Key(0)]
		public ErrorType            errorCode;
	}

	[MessagePackObject]
	public class RequestLogin
	{
		[Key(0)]
		public string               accountID;
		[Key(1)]
		public string               password;
		[Key(2)]
		public ErrorType            errorCode;
	}

	[MessagePackObject]
	public class RequestLogout
	{
		[Key(0)]
		public ErrorType            errorCode;
	}

	[MessagePackObject]
	public class RequestSignin
	{
		[Key(0)]
		public string               accountID;
		[Key(1)]
		public string               password;
		[Key(2)]
		public ErrorType            errorCode;
	}

};
