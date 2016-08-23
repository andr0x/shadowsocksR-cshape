using System;

namespace OpenDNS
{
	// Token: 0x0200009C RID: 156
	public class SOA : ResourceRecord
	{
		// Token: 0x06000552 RID: 1362 RVA: 0x0002AD34 File Offset: 0x00028F34
		public SOA(string _Name, Types _Type, Classes _Class, int _TimeToLive, string _Server, string _Email, long _Serial, long _Refresh, long _Retry, long _Expire, long _Minimum) : base(_Name, _Type, _Class, _TimeToLive)
		{
			this.Server = _Server;
			this.Email = _Email;
			this.Serial = _Serial;
			this.Refresh = _Refresh;
			this.Retry = _Retry;
			this.Expire = _Expire;
			this.Minimum = _Minimum;
		}

		// Token: 0x040003B5 RID: 949
		public string Email;

		// Token: 0x040003B9 RID: 953
		public long Expire;

		// Token: 0x040003BA RID: 954
		public long Minimum;

		// Token: 0x040003B7 RID: 951
		public long Refresh;

		// Token: 0x040003B8 RID: 952
		public long Retry;

		// Token: 0x040003B6 RID: 950
		public long Serial;

		// Token: 0x040003B4 RID: 948
		public string Server;
	}
}
