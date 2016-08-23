using System;

namespace ZXing.QrCode.Internal
{
	// Token: 0x0200007D RID: 125
	internal sealed class BlockPair
	{
		// Token: 0x0600045D RID: 1117 RVA: 0x000258EB File Offset: 0x00023AEB
		public BlockPair(byte[] data, byte[] errorCorrection)
		{
			this.dataBytes = data;
			this.errorCorrectionBytes = errorCorrection;
		}

		// Token: 0x1700005D RID: 93
		public byte[] DataBytes
		{
			// Token: 0x0600045E RID: 1118 RVA: 0x00025901 File Offset: 0x00023B01
			get
			{
				return this.dataBytes;
			}
		}

		// Token: 0x1700005E RID: 94
		public byte[] ErrorCorrectionBytes
		{
			// Token: 0x0600045F RID: 1119 RVA: 0x00025909 File Offset: 0x00023B09
			get
			{
				return this.errorCorrectionBytes;
			}
		}

		// Token: 0x04000318 RID: 792
		private readonly byte[] dataBytes;

		// Token: 0x04000319 RID: 793
		private readonly byte[] errorCorrectionBytes;
	}
}
