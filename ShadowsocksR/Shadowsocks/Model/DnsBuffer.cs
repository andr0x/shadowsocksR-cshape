using System;
using System.Net;

namespace Shadowsocks.Model
{
	// Token: 0x02000020 RID: 32
	public class DnsBuffer
	{
		// Token: 0x06000135 RID: 309 RVA: 0x0000FCC4 File Offset: 0x0000DEC4
		public bool isExpired(string host)
		{
			DateTime arg_06_0 = this.updateTime;
			return this.host != host || (DateTime.Now - this.updateTime).TotalMinutes > 10.0;
		}

		// Token: 0x06000136 RID: 310 RVA: 0x0000FD0B File Offset: 0x0000DF0B
		public void UpdateDns(string host, IPAddress ip)
		{
			this.updateTime = DateTime.Now;
			this.ip = new IPAddress(ip.GetAddressBytes());
			this.host = (string)host.Clone();
		}

		// Token: 0x040000EE RID: 238
		public string host;

		// Token: 0x040000EC RID: 236
		public IPAddress ip;

		// Token: 0x040000ED RID: 237
		public DateTime updateTime;
	}
}
