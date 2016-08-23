using System;

namespace Shadowsocks.Model
{
	// Token: 0x0200002B RID: 43
	public class ErrorLog
	{
		// Token: 0x06000179 RID: 377 RVA: 0x00011BCC File Offset: 0x0000FDCC
		public ErrorLog(int no)
		{
			this.errno = no;
			this.time = DateTime.Now;
		}

		// Token: 0x0400013D RID: 317
		public int errno;

		// Token: 0x0400013E RID: 318
		public DateTime time;
	}
}
