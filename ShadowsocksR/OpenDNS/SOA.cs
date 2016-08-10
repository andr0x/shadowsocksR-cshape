using System;

namespace OpenDNS
{
	// Token: 0x0200009A RID: 154
	public class SOA : ResourceRecord
	{
		// Token: 0x06000549 RID: 1353 RVA: 0x0002BAEC File Offset: 0x00029CEC
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

		// Token: 0x040003B8 RID: 952
		public string Email;

		// Token: 0x040003BC RID: 956
		public long Expire;

		// Token: 0x040003BD RID: 957
		public long Minimum;

		// Token: 0x040003BA RID: 954
		public long Refresh;

		// Token: 0x040003BB RID: 955
		public long Retry;

		// Token: 0x040003B9 RID: 953
		public long Serial;

		// Token: 0x040003B7 RID: 951
		public string Server;
	}
}
