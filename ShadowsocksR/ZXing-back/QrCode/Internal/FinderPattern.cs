using System;

namespace ZXing.QrCode.Internal
{
	// Token: 0x02000078 RID: 120
	public sealed class FinderPattern : ResultPoint
	{
		// Token: 0x0600043B RID: 1083 RVA: 0x000257EF File Offset: 0x000239EF
		internal FinderPattern(float posX, float posY, float estimatedModuleSize) : this(posX, posY, estimatedModuleSize, 1)
		{
			this.estimatedModuleSize = estimatedModuleSize;
			this.count = 1;
		}

		// Token: 0x0600043C RID: 1084 RVA: 0x00025809 File Offset: 0x00023A09
		internal FinderPattern(float posX, float posY, float estimatedModuleSize, int count) : base(posX, posY)
		{
			this.estimatedModuleSize = estimatedModuleSize;
			this.count = count;
		}

		// Token: 0x0600043F RID: 1087 RVA: 0x00025834 File Offset: 0x00023A34
		internal bool aboutEquals(float moduleSize, float i, float j)
		{
			if (Math.Abs(i - this.Y) <= moduleSize && Math.Abs(j - this.X) <= moduleSize)
			{
				float num = Math.Abs(moduleSize - this.estimatedModuleSize);
				return num <= 1f || num <= this.estimatedModuleSize;
			}
			return false;
		}

		// Token: 0x06000440 RID: 1088 RVA: 0x00025888 File Offset: 0x00023A88
		internal FinderPattern combineEstimate(float i, float j, float newModuleSize)
		{
			int num = this.count + 1;
			float arg_47_0 = ((float)this.count * this.X + j) / (float)num;
			float posY = ((float)this.count * this.Y + i) / (float)num;
			float num2 = ((float)this.count * this.estimatedModuleSize + newModuleSize) / (float)num;
			return new FinderPattern(arg_47_0, posY, num2, num);
		}

		// Token: 0x17000053 RID: 83
		internal int Count
		{
			// Token: 0x0600043E RID: 1086 RVA: 0x0002582A File Offset: 0x00023A2A
			get
			{
				return this.count;
			}
		}

		// Token: 0x17000052 RID: 82
		public float EstimatedModuleSize
		{
			// Token: 0x0600043D RID: 1085 RVA: 0x00025822 File Offset: 0x00023A22
			get
			{
				return this.estimatedModuleSize;
			}
		}

		// Token: 0x0400030E RID: 782
		private int count;

		// Token: 0x0400030D RID: 781
		private readonly float estimatedModuleSize;
	}
}
