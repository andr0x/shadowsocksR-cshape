using System;

namespace ZXing.QrCode.Internal
{
	// Token: 0x02000073 RID: 115
	public sealed class QRCodeDecoderMetaData
	{
		// Token: 0x06000414 RID: 1044 RVA: 0x00022DDB File Offset: 0x00020FDB
		public QRCodeDecoderMetaData(bool mirrored)
		{
			this.mirrored = mirrored;
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x00022DF4 File Offset: 0x00020FF4
		public void applyMirroredCorrection(ResultPoint[] points)
		{
			if (!this.mirrored || points == null || points.Length < 3)
			{
				return;
			}
			ResultPoint resultPoint = points[0];
			points[0] = points[2];
			points[2] = resultPoint;
		}

		// Token: 0x1700004B RID: 75
		public bool IsMirrored
		{
			// Token: 0x06000415 RID: 1045 RVA: 0x00022DEA File Offset: 0x00020FEA
			get
			{
				return this.mirrored;
			}
		}

		// Token: 0x040002FA RID: 762
		private readonly bool mirrored;
	}
}
