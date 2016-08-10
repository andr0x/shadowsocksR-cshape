using System;

namespace ZXing.QrCode.Internal
{
	// Token: 0x0200007B RID: 123
	internal sealed class BlockPair
	{
		// Token: 0x06000454 RID: 1108 RVA: 0x000266A3 File Offset: 0x000248A3
		public BlockPair(byte[] data, byte[] errorCorrection)
		{
			this.dataBytes = data;
			this.errorCorrectionBytes = errorCorrection;
		}

		// Token: 0x1700005A RID: 90
		public byte[] DataBytes
		{
			// Token: 0x06000455 RID: 1109 RVA: 0x000266B9 File Offset: 0x000248B9
			get
			{
				return this.dataBytes;
			}
		}

		// Token: 0x1700005B RID: 91
		public byte[] ErrorCorrectionBytes
		{
			// Token: 0x06000456 RID: 1110 RVA: 0x000266C1 File Offset: 0x000248C1
			get
			{
				return this.errorCorrectionBytes;
			}
		}

		// Token: 0x0400031B RID: 795
		private readonly byte[] dataBytes;

		// Token: 0x0400031C RID: 796
		private readonly byte[] errorCorrectionBytes;
	}
}
