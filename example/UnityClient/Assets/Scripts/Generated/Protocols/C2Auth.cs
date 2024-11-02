using System.Collections.Generic;
using MessagePack;
using Devarc;

namespace C2Auth
{
	[MessagePackObject]
	public partial class RequestSession : BaseTableElement<RequestSession>, ITableElement<RequestSession>
	{
		[Key(0)]
		public ErrorType            errorCode;
	}

	[MessagePackObject]
	public partial class RequestLogin : BaseTableElement<RequestLogin>, ITableElement<RequestLogin>
	{
		[Key(0)]
		public string               accountID;
		[Key(1)]
		public string               password;
		[Key(2)]
		public ErrorType            errorCode;
	}

	[MessagePackObject]
	public partial class RequestLogout : BaseTableElement<RequestLogout>, ITableElement<RequestLogout>
	{
		[Key(0)]
		public ErrorType            errorCode;
	}

	[MessagePackObject]
	public partial class RequestSignin : BaseTableElement<RequestSignin>, ITableElement<RequestSignin>
	{
		[Key(0)]
		public string               accountID;
		[Key(1)]
		public string               password;
		[Key(2)]
		public ErrorType            errorCode;
	}

};
