using System;
using System.Collections.Generic;
using Shadowsocks.Model;

namespace Shadowsocks.Controller
{
	// Token: 0x0200004A RID: 74
	internal class SpeedTester
	{
		// Token: 0x06000294 RID: 660 RVA: 0x00018FDC File Offset: 0x000171DC
		public void AddDownloadSize(int size)
		{
			if (this.sizeDownloadList.Count == 2)
			{
				this.sizeDownloadList[1] = new TransLog(size, DateTime.Now);
			}
			else
			{
				this.sizeDownloadList.Add(new TransLog(size, DateTime.Now));
			}
			this.sizeDownload += (long)size;
			if (this.transfer != null && this.server != null)
			{
				this.transfer.AddDownload(this.server, (long)size);
			}
		}

		// Token: 0x06000295 RID: 661 RVA: 0x00019058 File Offset: 0x00017258
		public void AddRecvSize(int size)
		{
			this.sizeRecv += (long)size;
		}

		// Token: 0x06000296 RID: 662 RVA: 0x00019069 File Offset: 0x00017269
		public void AddUploadSize(int size)
		{
			this.sizeUpload += (long)size;
			if (this.transfer != null && this.server != null)
			{
				this.transfer.AddUpload(this.server, (long)size);
			}
		}

		// Token: 0x06000290 RID: 656 RVA: 0x00018F80 File Offset: 0x00017180
		public void BeginConnect()
		{
			this.timeConnectBegin = DateTime.Now;
		}

		// Token: 0x06000293 RID: 659 RVA: 0x00018FA8 File Offset: 0x000171A8
		public bool BeginDownload()
		{
			if (this.timeBeginDownload == default(DateTime))
			{
				this.timeBeginDownload = DateTime.Now;
				return true;
			}
			return false;
		}

		// Token: 0x06000292 RID: 658 RVA: 0x00018F9A File Offset: 0x0001719A
		public void BeginUpload()
		{
			this.timeBeginUpload = DateTime.Now;
		}

		// Token: 0x06000291 RID: 657 RVA: 0x00018F8D File Offset: 0x0001718D
		public void EndConnect()
		{
			this.timeConnectEnd = DateTime.Now;
		}

		// Token: 0x06000298 RID: 664 RVA: 0x00019164 File Offset: 0x00017364
		public int GetActionType()
		{
			int num = 0;
			if (this.sizeDownload > 1048576L)
			{
				num |= 1;
			}
			if (this.sizeUpload > 1048576L)
			{
				num |= 2;
			}
			double totalSeconds = (DateTime.Now - this.timeConnectEnd).TotalSeconds;
			if (totalSeconds > 5.0 && (double)(this.sizeDownload + this.sizeUpload) / totalSeconds > 16384.0)
			{
				num |= 4;
			}
			return num;
		}

		// Token: 0x06000297 RID: 663 RVA: 0x000190A0 File Offset: 0x000172A0
		public long GetAvgDownloadSpeed()
		{
			if (this.sizeDownloadList == null || this.sizeDownloadList.Count < 2 || (this.sizeDownloadList[this.sizeDownloadList.Count - 1].recvTime - this.sizeDownloadList[0].recvTime).TotalSeconds <= 0.001)
			{
				return 0L;
			}
			return (long)((double)(this.sizeDownload - (long)this.sizeDownloadList[0].size) / (this.sizeDownloadList[this.sizeDownloadList.Count - 1].recvTime - this.sizeDownloadList[0].recvTime).TotalSeconds);
		}

		// Token: 0x040001F9 RID: 505
		public string server;

		// Token: 0x040001F6 RID: 502
		public long sizeDownload;

		// Token: 0x040001F8 RID: 504
		private List<TransLog> sizeDownloadList = new List<TransLog>();

		// Token: 0x040001F7 RID: 503
		public long sizeRecv;

		// Token: 0x040001F5 RID: 501
		public long sizeUpload;

		// Token: 0x040001F4 RID: 500
		public DateTime timeBeginDownload;

		// Token: 0x040001F3 RID: 499
		public DateTime timeBeginUpload;

		// Token: 0x040001F1 RID: 497
		public DateTime timeConnectBegin;

		// Token: 0x040001F2 RID: 498
		public DateTime timeConnectEnd;

		// Token: 0x040001FA RID: 506
		public ServerTransferTotal transfer;
	}
}
