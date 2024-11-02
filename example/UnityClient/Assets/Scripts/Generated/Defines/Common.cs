using System.Collections.Generic;
using MessagePack;

namespace Devarc
{
	public enum ErrorType
	{
		SUCCESS              = 0,
		UNKNOWN              = 1,
		DATABASE_ERROR       = 2,
		PROTOCOL_ERROR       = 3,
		SERVER_ERROR         = 4,
		SESSION_EXPIRED      = 5,
		SESSION_REMAIN       = 6,
		INVALID_PASSWORD     = 7,
		INVALID_SECRET       = 8,
	}

	[MessagePackObject]
	public partial class CommonResult : BaseTableElement<CommonResult>, ITableElement<CommonResult>
	{
		[Key(0)]
		public ErrorType            errorCode;
	}

	[MessagePackObject]
	public partial class CustomSigninResult : BaseTableElement<CustomSigninResult>, ITableElement<CustomSigninResult>
	{
		[Key(0)]
		public ErrorType            errorCode;
		[Key(1)]
		public string               secret;
	}

	[MessagePackObject]
	public partial class GoogleCodeResult : BaseTableElement<GoogleCodeResult>, ITableElement<GoogleCodeResult>
	{
		[Key(0)]
		public ErrorType            errorCode;
		[Key(1)]
		public string               code;
	}

	[MessagePackObject]
	public partial class GoogleSigninResult : BaseTableElement<GoogleSigninResult>, ITableElement<GoogleSigninResult>
	{
		[Key(0)]
		public ErrorType            errorCode;
		[Key(1)]
		public string               account_id;
		[Key(2)]
		public string               access_token;
		[Key(3)]
		public string               refresh_token;
		[Key(4)]
		public int                  expires_in;
		[Key(5)]
		public string               secret;
	}

	[MessagePackObject]
	public partial class GoogleRefreshResult : BaseTableElement<GoogleRefreshResult>, ITableElement<GoogleRefreshResult>
	{
		[Key(0)]
		public ErrorType            errorCode;
		[Key(1)]
		public string               access_token;
		[Key(2)]
		public string               refresh_token;
		[Key(3)]
		public int                  expires_in;
	}

	public enum GenderType
	{
		None                 = 0,
		Male                 = 1,
		Female               = 2,
	}

	[MessagePackObject]
	public partial class Account : BaseTableElement<Account>, ITableElement<Account>
	{
		[Key(0)]
		public string               nickName;
		[Key(1)]
		public int                  level;
	}

};
