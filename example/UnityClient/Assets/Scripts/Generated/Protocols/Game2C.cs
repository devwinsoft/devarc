using System.Collections.Generic;
using MessagePack;
using Devarc;

namespace Game2C
{
	[MessagePackObject]
	public partial class NotifyLogin : BaseTableElement<NotifyLogin>, ITableElement<NotifyLogin>
	{
		[Key(0)]
		public ErrorType            errorCode;
	}

};
