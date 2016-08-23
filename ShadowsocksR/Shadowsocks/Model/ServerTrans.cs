using System;

namespace Shadowsocks.Model
{
	// Token: 0x02000028 RID: 40
	[Serializable]
	public class ServerTrans
	{
		// Token: 0x0600016F RID: 367 RVA: 0x0001189C File Offset: 0x0000FA9C
		private void AddDownload(long bytes)
		{
			this.totalDownloadBytes += bytes;
		}

		// Token: 0x0600016E RID: 366 RVA: 0x0001188C File Offset: 0x0000FA8C
		private void AddUpload(long bytes)
		{
			this.totalUploadBytes += bytes;
		}

		// Token: 0x04000135 RID: 309
		public long totalDownloadBytes;

		// Token: 0x04000134 RID: 308
		public long totalUploadBytes;
	}
}
