using System;

namespace ZXing.QrCode.Internal
{
	// Token: 0x0200007A RID: 122
	public sealed class FinderPattern : ResultPoint
	{
		// Token: 0x06000444 RID: 1092 RVA: 0x00024A37 File Offset: 0x00022C37
		internal FinderPattern(float posX, float posY, float estimatedModuleSize) : this(posX, posY, estimatedModuleSize, 1)
		{
			this.estimatedModuleSize = estimatedModuleSize;
			this.count = 1;
		}

		// Token: 0x06000445 RID: 1093 RVA: 0x00024A51 File Offset: 0x00022C51
		internal FinderPattern(float posX, float posY, float estimatedModuleSize, int count) : base(posX, posY)
		{
			this.estimatedModuleSize = estimatedModuleSize;
			this.count = count;
		}

		// Token: 0x06000448 RID: 1096 RVA: 0x00024A7C File Offset: 0x00022C7C
		internal bool aboutEquals(float moduleSize, float i, float j)
		{
			if (Math.Abs(i - this.Y) <= moduleSize && Math.Abs(j - this.X) <= moduleSize)
			{
				float num = Math.Abs(moduleSize - this.estimatedModuleSize);
				return num <= 1f || num <= this.estimatedModuleSize;
			}
			return false;
		}

		// Token: 0x06000449 RID: 1097 RVA: 0x00024AD0 File Offset: 0x00022CD0
		internal FinderPattern combineEstimate(float i, float j, float newModuleSize)
		{
			int num = this.count + 1;
			float arg_47_0 = ((float)this.count * this.X + j) / (float)num;
			float posY = ((float)this.count * this.Y + i) / (float)num;
			float num2 = ((float)this.count * this.estimatedModuleSize + newModuleSize) / (float)num;
			return new FinderPattern(arg_47_0, posY, num2, num);
		}

		// Token: 0x17000056 RID: 86
		internal int Count
		{
			// Token: 0x06000447 RID: 1095 RVA: 0x00024A72 File Offset: 0x00022C72
			get
			{
				return this.count;
			}
		}

		// Token: 0x17000055 RID: 85
		public float EstimatedModuleSize
		{
			// Token: 0x06000446 RID: 1094 RVA: 0x00024A6A File Offset: 0x00022C6A
			get
			{
				return this.estimatedModuleSize;
			}
		}

		// Token: 0x0400030B RID: 779
		private int count;

		// Token: 0x0400030A RID: 778
		private readonly float estimatedModuleSize;
	}
}
