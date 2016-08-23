using System;

namespace ZXing.QrCode.Internal
{
	// Token: 0x0200007C RID: 124
	public sealed class FinderPatternInfo
	{
		// Token: 0x06000459 RID: 1113 RVA: 0x000258B0 File Offset: 0x00023AB0
		public FinderPatternInfo(FinderPattern[] patternCenters)
		{
			this.bottomLeft = patternCenters[0];
			this.topLeft = patternCenters[1];
			this.topRight = patternCenters[2];
		}

		// Token: 0x1700005A RID: 90
		public FinderPattern BottomLeft
		{
			// Token: 0x0600045A RID: 1114 RVA: 0x000258D3 File Offset: 0x00023AD3
			get
			{
				return this.bottomLeft;
			}
		}

		// Token: 0x1700005B RID: 91
		public FinderPattern TopLeft
		{
			// Token: 0x0600045B RID: 1115 RVA: 0x000258DB File Offset: 0x00023ADB
			get
			{
				return this.topLeft;
			}
		}

		// Token: 0x1700005C RID: 92
		public FinderPattern TopRight
		{
			// Token: 0x0600045C RID: 1116 RVA: 0x000258E3 File Offset: 0x00023AE3
			get
			{
				return this.topRight;
			}
		}

		// Token: 0x04000315 RID: 789
		private readonly FinderPattern bottomLeft;

		// Token: 0x04000316 RID: 790
		private readonly FinderPattern topLeft;

		// Token: 0x04000317 RID: 791
		private readonly FinderPattern topRight;
	}
}
