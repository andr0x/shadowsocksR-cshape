using System;

namespace ZXing.QrCode.Internal
{
	// Token: 0x02000077 RID: 119
	public sealed class AlignmentPattern : ResultPoint
	{
		// Token: 0x0600042D RID: 1069 RVA: 0x00023EEE File Offset: 0x000220EE
		internal AlignmentPattern(float posX, float posY, float estimatedModuleSize) : base(posX, posY)
		{
			this.estimatedModuleSize = estimatedModuleSize;
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x00023F00 File Offset: 0x00022100
		internal bool aboutEquals(float moduleSize, float i, float j)
		{
			if (Math.Abs(i - this.Y) <= moduleSize && Math.Abs(j - this.X) <= moduleSize)
			{
				float num = Math.Abs(moduleSize - this.estimatedModuleSize);
				return num <= 1f || num <= this.estimatedModuleSize;
			}
			return false;
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x00023F54 File Offset: 0x00022154
		internal AlignmentPattern combineEstimate(float i, float j, float newModuleSize)
		{
			float arg_2E_0 = (this.X + j) / 2f;
			float posY = (this.Y + i) / 2f;
			float num = (this.estimatedModuleSize + newModuleSize) / 2f;
			return new AlignmentPattern(arg_2E_0, posY, num);
		}

		// Token: 0x040002FE RID: 766
		private float estimatedModuleSize;
	}
}
