using System.Collections.Generic;
using MessagePack;
using Devarc;

namespace Auth2C
{
	[MessagePackObject]
	public partial class NotifySession : BaseTableElement<NotifySession>, ITableElement<NotifySession>
	{
		[Key(0)]
		public string               sessionID;
		[Key(1)]
		public int                  secret;
		[Key(2)]
		public ErrorType            errorCode;
	}

	[MessagePackObject]
	public partial class NotifyLogin : BaseTableElement<NotifyLogin>, ITableElement<NotifyLogin>
	{
		[Key(0)]
		public string               sessionID;
		[Key(1)]
		public int                  secret;
		[Key(2)]
		public ErrorType            errorCode;
	}

	[MessagePackObject]
	public partial class NotifyLogout : BaseTableElement<NotifyLogout>, ITableElement<NotifyLogout>
	{
		[Key(0)]
		public ErrorType            errorCode;
	}

	[MessagePackObject]
	public partial class NotifySignin : BaseTableElement<NotifySignin>, ITableElement<NotifySignin>
	{
		[Key(0)]
		public string               sessionID;
		[Key(1)]
		public int                  secret;
		[Key(2)]
		public ErrorType            errorCode;
	}

	[MessagePackObject]
	public partial class NotifyError : BaseTableElement<NotifyError>, ITableElement<NotifyError>
	{
		[Key(0)]
		public ErrorType            errorCode;
	}

};
