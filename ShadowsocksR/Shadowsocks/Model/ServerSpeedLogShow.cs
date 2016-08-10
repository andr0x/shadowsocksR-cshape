using System;

namespace Shadowsocks.Model
{
	// Token: 0x0200002E RID: 46
	[Serializable]
	public class ServerSpeedLogShow
	{
		// Token: 0x04000155 RID: 341
		public long avgConnectTime;

		// Token: 0x04000156 RID: 342
		public long avgDownloadBytes;

		// Token: 0x04000158 RID: 344
		public long avgUploadBytes;

		// Token: 0x0400014B RID: 331
		public long errorConnectTimes;

		// Token: 0x0400014F RID: 335
		public long errorContinurousTimes;

		// Token: 0x0400014D RID: 333
		public long errorDecodeTimes;

		// Token: 0x0400014E RID: 334
		public long errorEmptyTimes;

		// Token: 0x04000150 RID: 336
		public long errorLogTimes;

		// Token: 0x0400014C RID: 332
		public long errorTimeoutTimes;

		// Token: 0x04000157 RID: 343
		public long maxDownloadBytes;

		// Token: 0x04000159 RID: 345
		public long maxUploadBytes;

		// Token: 0x04000154 RID: 340
		public int sumConnectTime;

		// Token: 0x04000149 RID: 329
		public long totalConnectTimes;

		// Token: 0x0400014A RID: 330
		public long totalDisconnectTimes;

		// Token: 0x04000152 RID: 338
		public long totalDownloadBytes;

		// Token: 0x04000153 RID: 339
		public long totalDownloadRawBytes;

		// Token: 0x04000151 RID: 337
		public long totalUploadBytes;
	}
}
