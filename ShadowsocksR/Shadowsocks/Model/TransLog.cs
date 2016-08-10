using System;

namespace Shadowsocks.Model
{
	// Token: 0x0200002C RID: 44
	public class TransLog
	{
		// Token: 0x0600018C RID: 396 RVA: 0x0001253F File Offset: 0x0001073F
		public TransLog(int s, DateTime t)
		{
			this.size = s;
			this.recvTime = t;
		}

		// Token: 0x04000146 RID: 326
		public DateTime recvTime;

		// Token: 0x04000145 RID: 325
		public int size;
	}
}
