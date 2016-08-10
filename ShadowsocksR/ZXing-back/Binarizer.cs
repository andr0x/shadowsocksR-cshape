using System;
using ZXing.Common;

namespace ZXing
{
	// Token: 0x0200005F RID: 95
	public abstract class Binarizer
	{
		// Token: 0x0600038F RID: 911 RVA: 0x00020878 File Offset: 0x0001EA78
		protected internal Binarizer(LuminanceSource source)
		{
			if (source == null)
			{
				throw new ArgumentException("Source must be non-null.");
			}
			this.source = source;
		}

		// Token: 0x06000393 RID: 915
		public abstract Binarizer createBinarizer(LuminanceSource source);

		// Token: 0x06000391 RID: 913
		public abstract BitArray getBlackRow(int y, BitArray row);

		// Token: 0x1700002D RID: 45
		public abstract BitMatrix BlackMatrix
		{
			// Token: 0x06000392 RID: 914
			get;
		}

		// Token: 0x1700002F RID: 47
		public int Height
		{
			// Token: 0x06000395 RID: 917 RVA: 0x000208AA File Offset: 0x0001EAAA
			get
			{
				return this.source.Height;
			}
		}

		// Token: 0x1700002C RID: 44
		public virtual LuminanceSource LuminanceSource
		{
			// Token: 0x06000390 RID: 912 RVA: 0x00020895 File Offset: 0x0001EA95
			get
			{
				return this.source;
			}
		}

		// Token: 0x1700002E RID: 46
		public int Width
		{
			// Token: 0x06000394 RID: 916 RVA: 0x0002089D File Offset: 0x0001EA9D
			get
			{
				return this.source.Width;
			}
		}

		// Token: 0x04000296 RID: 662
		private readonly LuminanceSource source;
	}
}
