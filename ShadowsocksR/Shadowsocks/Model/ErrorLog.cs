using System;

namespace Shadowsocks.Model
{
	// Token: 0x0200002D RID: 45
	public class ErrorLog
	{
		// Token: 0x0600018D RID: 397 RVA: 0x00012555 File Offset: 0x00010755
		public ErrorLog(int no)
		{
			this.errno = no;
			this.time = DateTime.Now;
		}

		// Token: 0x04000147 RID: 327
		public int errno;

		// Token: 0x04000148 RID: 328
		public DateTime time;
	}
}
