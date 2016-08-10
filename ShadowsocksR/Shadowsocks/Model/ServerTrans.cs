using System;

namespace Shadowsocks.Model
{
	// Token: 0x0200002A RID: 42
	[Serializable]
	public class ServerTrans
	{
		// Token: 0x06000183 RID: 387 RVA: 0x0001222C File Offset: 0x0001042C
		private void AddDownload(long bytes)
		{
			this.totalDownloadBytes += bytes;
		}

		// Token: 0x06000182 RID: 386 RVA: 0x0001221C File Offset: 0x0001041C
		private void AddUpload(long bytes)
		{
			this.totalUploadBytes += bytes;
		}

		// Token: 0x04000140 RID: 320
		public long totalDownloadBytes;

		// Token: 0x0400013F RID: 319
		public long totalUploadBytes;
	}
}
