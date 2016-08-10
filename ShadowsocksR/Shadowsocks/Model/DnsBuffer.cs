using System;
using System.Net;

namespace Shadowsocks.Model
{
	// Token: 0x02000022 RID: 34
	public class DnsBuffer
	{
		// Token: 0x06000149 RID: 329 RVA: 0x00010708 File Offset: 0x0000E908
		public bool isExpired(string host)
		{
			DateTime arg_06_0 = this.updateTime;
			return this.host != host || (DateTime.Now - this.updateTime).TotalMinutes > 10.0;
		}

		// Token: 0x0600014A RID: 330 RVA: 0x0001074F File Offset: 0x0000E94F
		public void UpdateDns(string host, IPAddress ip)
		{
			this.updateTime = DateTime.Now;
			this.ip = new IPAddress(ip.GetAddressBytes());
			this.host = (string)host.Clone();
		}

		// Token: 0x040000F9 RID: 249
		public string host;

		// Token: 0x040000F7 RID: 247
		public IPAddress ip;

		// Token: 0x040000F8 RID: 248
		public DateTime updateTime;
	}
}
