using System;
using System.Collections.Generic;
using Shadowsocks.Model;

namespace Shadowsocks.Controller
{
	// Token: 0x02000048 RID: 72
	internal class SpeedTester
	{
		// Token: 0x06000275 RID: 629 RVA: 0x00017A00 File Offset: 0x00015C00
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

		// Token: 0x06000276 RID: 630 RVA: 0x00017A7C File Offset: 0x00015C7C
		public void AddRecvSize(int size)
		{
			this.sizeRecv += (long)size;
		}

		// Token: 0x06000277 RID: 631 RVA: 0x00017A8D File Offset: 0x00015C8D
		public void AddUploadSize(int size)
		{
			this.sizeUpload += (long)size;
			if (this.transfer != null && this.server != null)
			{
				this.transfer.AddUpload(this.server, (long)size);
			}
		}

		// Token: 0x06000271 RID: 625 RVA: 0x000179A3 File Offset: 0x00015BA3
		public void BeginConnect()
		{
			this.timeConnectBegin = DateTime.Now;
		}

		// Token: 0x06000274 RID: 628 RVA: 0x000179CC File Offset: 0x00015BCC
		public bool BeginDownload()
		{
			if (this.timeBeginDownload == default(DateTime))
			{
				this.timeBeginDownload = DateTime.Now;
				return true;
			}
			return false;
		}

		// Token: 0x06000273 RID: 627 RVA: 0x000179BD File Offset: 0x00015BBD
		public void BeginUpload()
		{
			this.timeBeginUpload = DateTime.Now;
		}

		// Token: 0x06000272 RID: 626 RVA: 0x000179B0 File Offset: 0x00015BB0
		public void EndConnect()
		{
			this.timeConnectEnd = DateTime.Now;
		}

		// Token: 0x06000279 RID: 633 RVA: 0x00017B88 File Offset: 0x00015D88
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

		// Token: 0x06000278 RID: 632 RVA: 0x00017AC4 File Offset: 0x00015CC4
		public long GetAvgDownloadSpeed()
		{
			if (this.sizeDownloadList == null || this.sizeDownloadList.Count < 2 || (this.sizeDownloadList[this.sizeDownloadList.Count - 1].recvTime - this.sizeDownloadList[0].recvTime).TotalSeconds <= 0.001)
			{
				return 0L;
			}
			return (long)((double)(this.sizeDownload - (long)this.sizeDownloadList[0].size) / (this.sizeDownloadList[this.sizeDownloadList.Count - 1].recvTime - this.sizeDownloadList[0].recvTime).TotalSeconds);
		}

		// Token: 0x040001E9 RID: 489
		public string server;

		// Token: 0x040001E6 RID: 486
		public long sizeDownload;

		// Token: 0x040001E8 RID: 488
		private List<TransLog> sizeDownloadList = new List<TransLog>();

		// Token: 0x040001E7 RID: 487
		public long sizeRecv;

		// Token: 0x040001E5 RID: 485
		public long sizeUpload;

		// Token: 0x040001E4 RID: 484
		public DateTime timeBeginDownload;

		// Token: 0x040001E3 RID: 483
		public DateTime timeBeginUpload;

		// Token: 0x040001E1 RID: 481
		public DateTime timeConnectBegin;

		// Token: 0x040001E2 RID: 482
		public DateTime timeConnectEnd;

		// Token: 0x040001EA RID: 490
		public ServerTransferTotal transfer;
	}
}
