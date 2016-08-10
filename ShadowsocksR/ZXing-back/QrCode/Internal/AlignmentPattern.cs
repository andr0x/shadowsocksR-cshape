using System;

namespace ZXing.QrCode.Internal
{
	// Token: 0x02000075 RID: 117
	public sealed class AlignmentPattern : ResultPoint
	{
		// Token: 0x06000424 RID: 1060 RVA: 0x00024CA6 File Offset: 0x00022EA6
		internal AlignmentPattern(float posX, float posY, float estimatedModuleSize) : base(posX, posY)
		{
			this.estimatedModuleSize = estimatedModuleSize;
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x00024CB8 File Offset: 0x00022EB8
		internal bool aboutEquals(float moduleSize, float i, float j)
		{
			if (Math.Abs(i - this.Y) <= moduleSize && Math.Abs(j - this.X) <= moduleSize)
			{
				float num = Math.Abs(moduleSize - this.estimatedModuleSize);
				return num <= 1f || num <= this.estimatedModuleSize;
			}
			return false;
		}

		// Token: 0x06000426 RID: 1062 RVA: 0x00024D0C File Offset: 0x00022F0C
		internal AlignmentPattern combineEstimate(float i, float j, float newModuleSize)
		{
			float arg_2E_0 = (this.X + j) / 2f;
			float posY = (this.Y + i) / 2f;
			float num = (this.estimatedModuleSize + newModuleSize) / 2f;
			return new AlignmentPattern(arg_2E_0, posY, num);
		}

		// Token: 0x04000301 RID: 769
		private float estimatedModuleSize;
	}
}
