using System;

namespace ZXing.QrCode.Internal
{
	// Token: 0x02000075 RID: 117
	public sealed class QRCodeDecoderMetaData
	{
		// Token: 0x0600041D RID: 1053 RVA: 0x00022023 File Offset: 0x00020223
		public QRCodeDecoderMetaData(bool mirrored)
		{
			this.mirrored = mirrored;
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x0002203C File Offset: 0x0002023C
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

		// Token: 0x1700004E RID: 78
		public bool IsMirrored
		{
			// Token: 0x0600041E RID: 1054 RVA: 0x00022032 File Offset: 0x00020232
			get
			{
				return this.mirrored;
			}
		}

		// Token: 0x040002F7 RID: 759
		private readonly bool mirrored;
	}
}
