using System;

namespace ZXing.QrCode.Internal
{
	// Token: 0x0200007A RID: 122
	public sealed class FinderPatternInfo
	{
		// Token: 0x06000450 RID: 1104 RVA: 0x00026668 File Offset: 0x00024868
		public FinderPatternInfo(FinderPattern[] patternCenters)
		{
			this.bottomLeft = patternCenters[0];
			this.topLeft = patternCenters[1];
			this.topRight = patternCenters[2];
		}

		// Token: 0x17000057 RID: 87
		public FinderPattern BottomLeft
		{
			// Token: 0x06000451 RID: 1105 RVA: 0x0002668B File Offset: 0x0002488B
			get
			{
				return this.bottomLeft;
			}
		}

		// Token: 0x17000058 RID: 88
		public FinderPattern TopLeft
		{
			// Token: 0x06000452 RID: 1106 RVA: 0x00026693 File Offset: 0x00024893
			get
			{
				return this.topLeft;
			}
		}

		// Token: 0x17000059 RID: 89
		public FinderPattern TopRight
		{
			// Token: 0x06000453 RID: 1107 RVA: 0x0002669B File Offset: 0x0002489B
			get
			{
				return this.topRight;
			}
		}

		// Token: 0x04000318 RID: 792
		private readonly FinderPattern bottomLeft;

		// Token: 0x04000319 RID: 793
		private readonly FinderPattern topLeft;

		// Token: 0x0400031A RID: 794
		private readonly FinderPattern topRight;
	}
}
