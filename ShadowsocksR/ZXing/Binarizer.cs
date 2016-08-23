using System;
using ZXing.Common;

namespace ZXing
{
	// Token: 0x02000061 RID: 97
	public abstract class Binarizer
	{
		// Token: 0x06000398 RID: 920 RVA: 0x0001FAC0 File Offset: 0x0001DCC0
		protected internal Binarizer(LuminanceSource source)
		{
			if (source == null)
			{
				throw new ArgumentException("Source must be non-null.");
			}
			this.source = source;
		}

		// Token: 0x0600039C RID: 924
		public abstract Binarizer createBinarizer(LuminanceSource source);

		// Token: 0x0600039A RID: 922
		public abstract BitArray getBlackRow(int y, BitArray row);

		// Token: 0x17000030 RID: 48
		public abstract BitMatrix BlackMatrix
		{
			// Token: 0x0600039B RID: 923
			get;
		}

		// Token: 0x17000032 RID: 50
		public int Height
		{
			// Token: 0x0600039E RID: 926 RVA: 0x0001FAF2 File Offset: 0x0001DCF2
			get
			{
				return this.source.Height;
			}
		}

		// Token: 0x1700002F RID: 47
		public virtual LuminanceSource LuminanceSource
		{
			// Token: 0x06000399 RID: 921 RVA: 0x0001FADD File Offset: 0x0001DCDD
			get
			{
				return this.source;
			}
		}

		// Token: 0x17000031 RID: 49
		public int Width
		{
			// Token: 0x0600039D RID: 925 RVA: 0x0001FAE5 File Offset: 0x0001DCE5
			get
			{
				return this.source.Width;
			}
		}

		// Token: 0x04000293 RID: 659
		private readonly LuminanceSource source;
	}
}
