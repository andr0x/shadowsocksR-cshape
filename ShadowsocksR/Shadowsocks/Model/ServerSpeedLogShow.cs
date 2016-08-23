using System;

namespace Shadowsocks.Model
{
	// Token: 0x0200002C RID: 44
	[Serializable]
	public class ServerSpeedLogShow
	{
		// Token: 0x0400014B RID: 331
		public long avgConnectTime;

		// Token: 0x0400014C RID: 332
		public long avgDownloadBytes;

		// Token: 0x0400014E RID: 334
		public long avgUploadBytes;

		// Token: 0x04000141 RID: 321
		public long errorConnectTimes;

		// Token: 0x04000145 RID: 325
		public long errorContinurousTimes;

		// Token: 0x04000143 RID: 323
		public long errorDecodeTimes;

		// Token: 0x04000144 RID: 324
		public long errorEmptyTimes;

		// Token: 0x04000146 RID: 326
		public long errorLogTimes;

		// Token: 0x04000142 RID: 322
		public long errorTimeoutTimes;

		// Token: 0x0400014D RID: 333
		public long maxDownloadBytes;

		// Token: 0x0400014F RID: 335
		public long maxUploadBytes;

		// Token: 0x0400014A RID: 330
		public int sumConnectTime;

		// Token: 0x0400013F RID: 319
		public long totalConnectTimes;

		// Token: 0x04000140 RID: 320
		public long totalDisconnectTimes;

		// Token: 0x04000148 RID: 328
		public long totalDownloadBytes;

		// Token: 0x04000149 RID: 329
		public long totalDownloadRawBytes;

		// Token: 0x04000147 RID: 327
		public long totalUploadBytes;
	}
}
