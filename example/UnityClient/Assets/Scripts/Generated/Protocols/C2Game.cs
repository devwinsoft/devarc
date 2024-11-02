using System.Collections.Generic;
using MessagePack;
using Devarc;

namespace C2Game
{
	[MessagePackObject]
	public partial class RequestLogin : BaseTableElement<RequestLogin>, ITableElement<RequestLogin>
	{
		[Key(0)]
		public string               sessionID;
		[Key(1)]
		public int                  secret;
	}

};
