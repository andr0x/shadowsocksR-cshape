using System;

namespace Shadowsocks.Model
{
	// Token: 0x0200002A RID: 42
	public class TransLog
	{
		// Token: 0x06000178 RID: 376 RVA: 0x00011BAF File Offset: 0x0000FDAF
		public TransLog(int s, DateTime t)
		{
			this.size = s;
			this.recvTime = t;
			this.endTime = t;
		}

		// Token: 0x0400013C RID: 316
		public DateTime endTime;

		// Token: 0x0400013B RID: 315
		public DateTime recvTime;

		// Token: 0x0400013A RID: 314
		public int size;
	}
}
