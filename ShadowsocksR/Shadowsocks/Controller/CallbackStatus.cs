using System;

namespace Shadowsocks.Controller
{
	// Token: 0x0200004E RID: 78
	internal class CallbackStatus
	{
		// Token: 0x060002BC RID: 700 RVA: 0x0001A168 File Offset: 0x00018368
		public CallbackStatus()
		{
			this.status = 0;
		}

		// Token: 0x060002BD RID: 701 RVA: 0x0001A178 File Offset: 0x00018378
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

		// Token: 0x17000020 RID: 32
		public int Status
		{
			// Token: 0x060002BE RID: 702 RVA: 0x0001A1C0 File Offset: 0x000183C0
			get
			{
				int result;
				lock (this)
				{
					result = this.status;
				}
				return result;
			}
			// Token: 0x060002BF RID: 703 RVA: 0x0001A200 File Offset: 0x00018400
			set
			{
				lock (this)
				{
					this.status = value;
				}
			}
		}

		// Token: 0x04000211 RID: 529
		protected int status;
	}
}
