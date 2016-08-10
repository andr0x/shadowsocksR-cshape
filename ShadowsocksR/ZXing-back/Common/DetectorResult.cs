using System;

namespace ZXing.Common
{
	// Token: 0x02000086 RID: 134
	public class DetectorResult
	{
		// Token: 0x060004E1 RID: 1249 RVA: 0x00028B5E File Offset: 0x00026D5E
		public DetectorResult(BitMatrix bits, ResultPoint[] points)
		{
			this.Bits = bits;
			this.Points = points;
		}

		// Token: 0x17000078 RID: 120
		public BitMatrix Bits
		{
			// Token: 0x060004DD RID: 1245 RVA: 0x00028B3C File Offset: 0x00026D3C
			get;
			// Token: 0x060004DE RID: 1246 RVA: 0x00028B44 File Offset: 0x00026D44
			private set;
		}

		// Token: 0x17000079 RID: 121
		public ResultPoint[] Points
		{
			// Token: 0x060004DF RID: 1247 RVA: 0x00028B4D File Offset: 0x00026D4D
			get;
			// Token: 0x060004E0 RID: 1248 RVA: 0x00028B55 File Offset: 0x00026D55
			private set;
		}
	}
}
