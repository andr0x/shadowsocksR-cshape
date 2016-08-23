using System;

namespace Shadowsocks.Controller
{
	// Token: 0x02000050 RID: 80
	internal class HandlerConfig
	{
		// Token: 0x0400021F RID: 543
		public bool autoSwitchOff = true;

		// Token: 0x04000217 RID: 535
		public string dns_servers;

		// Token: 0x04000222 RID: 546
		public bool forceRandom;

		// Token: 0x04000218 RID: 536
		public bool fouce_local_dns_query;

		// Token: 0x04000219 RID: 537
		public int proxyType;

		// Token: 0x0400021E RID: 542
		public string proxyUserAgent;

		// Token: 0x04000221 RID: 545
		public int reconnectTimes;

		// Token: 0x04000220 RID: 544
		public int reconnectTimesRemain;

		// Token: 0x0400021A RID: 538
		public string socks5RemoteHost;

		// Token: 0x0400021D RID: 541
		public string socks5RemotePassword;

		// Token: 0x0400021B RID: 539
		public int socks5RemotePort;

		// Token: 0x0400021C RID: 540
		public string socks5RemoteUsername;

		// Token: 0x04000214 RID: 532
		public string targetHost;

		// Token: 0x04000216 RID: 534
		public int try_keep_alive;

		// Token: 0x04000215 RID: 533
		public double TTL;
	}
}
