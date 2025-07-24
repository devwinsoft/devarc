using System.Collections.Generic;
using MessagePack;
using Devarc;

namespace C2Auth
{
	[MessagePackObject]
	public partial class RequestSession : BaseTableElement<RequestSession>, ITableElement<RequestSession>
	{
	}

	[MessagePackObject]
	public partial class RequestLogin : BaseTableElement<RequestLogin>, ITableElement<RequestLogin>
	{
		[Key(0)]
		public string               accountID;
		[Key(1)]
		public string               password;
	}

	[MessagePackObject]
	public partial class RequestLogout : BaseTableElement<RequestLogout>, ITableElement<RequestLogout>
	{
	}

	[MessagePackObject]
	public partial class RequestSignin : BaseTableElement<RequestSignin>, ITableElement<RequestSignin>
	{
		[Key(0)]
		public string               accountID;
		[Key(1)]
		public string               password;
	}

};
