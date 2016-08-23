using System;

namespace ZXing.Common.Detector
{
	// Token: 0x02000092 RID: 146
	public static class MathUtils
	{
		// Token: 0x0600052F RID: 1327 RVA: 0x00029AD8 File Offset: 0x00027CD8
		public static float distance(float aX, float aY, float bX, float bY)
		{
			float arg_07_0 = aX - bX;
			float num = aY - bY;
			double arg_0C_0 = (double)(arg_07_0 * arg_07_0);
			float expr_0A = num;
			return (float)Math.Sqrt(arg_0C_0 + (double)(expr_0A * expr_0A));
		}

		// Token: 0x06000530 RID: 1328 RVA: 0x00029AFC File Offset: 0x00027CFC
		public static float distance(int aX, int aY, int bX, int bY)
		{
			int arg_07_0 = aX - bX;
			int num = aY - bY;
			double arg_0C_0 = (double)(arg_07_0 * arg_07_0);
			int expr_0A = num;
			return (float)Math.Sqrt(arg_0C_0 + (double)(expr_0A * expr_0A));
		}

		// Token: 0x0600052E RID: 1326 RVA: 0x00029ACC File Offset: 0x00027CCC
		public static int round(float d)
		{
			return (int)(d + 0.5f);
		}
	}
}
