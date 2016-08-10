using System;

namespace Shadowsocks.Controller
{
	// Token: 0x0200004C RID: 76
	internal class CallbackStatus
	{
		// Token: 0x06000298 RID: 664 RVA: 0x000188BC File Offset: 0x00016ABC
		public CallbackStatus()
		{
			this.status = 0;
		}

		// Token: 0x06000299 RID: 665 RVA: 0x000188CC File Offset: 0x00016ACC
		public void SetIfEqu(int newStatus, int oldStatus)
		{
			lock (this)
			{
				if (this.status == oldStatus)
				{
					this.status = newStatus;
				}
			}
		}

		// Token: 0x1700001C RID: 28
		public int Status
		{
			// Token: 0x0600029A RID: 666 RVA: 0x00018914 File Offset: 0x00016B14
			get
			{
				int result;
				lock (this)
				{
					result = this.status;
				}
				return result;
			}
			// Token: 0x0600029B RID: 667 RVA: 0x00018954 File Offset: 0x00016B54
			set
			{
				lock (this)
				{
					this.status = value;
				}
			}
		}

		// Token: 0x040001FD RID: 509
		protected int status;
	}
}
