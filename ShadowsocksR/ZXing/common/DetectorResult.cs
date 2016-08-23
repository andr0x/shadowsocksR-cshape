using System;

namespace ZXing.Common
{
	// Token: 0x02000088 RID: 136
	public class DetectorResult
	{
		// Token: 0x060004EA RID: 1258 RVA: 0x00027DA6 File Offset: 0x00025FA6
		public DetectorResult(BitMatrix bits, ResultPoint[] points)
		{
			this.Bits = bits;
			this.Points = points;
		}

		// Token: 0x1700007B RID: 123
		public BitMatrix Bits
		{
			// Token: 0x060004E6 RID: 1254 RVA: 0x00027D84 File Offset: 0x00025F84
			get;
			// Token: 0x060004E7 RID: 1255 RVA: 0x00027D8C File Offset: 0x00025F8C
			private set;
		}

		// Token: 0x1700007C RID: 124
		public ResultPoint[] Points
		{
			// Token: 0x060004E8 RID: 1256 RVA: 0x00027D95 File Offset: 0x00025F95
			get;
			// Token: 0x060004E9 RID: 1257 RVA: 0x00027D9D File Offset: 0x00025F9D
			private set;
		}
	}
}
