using System;

namespace ZXing.Common.Detector
{
	// Token: 0x02000090 RID: 144
	public static class MathUtils
	{
		// Token: 0x06000526 RID: 1318 RVA: 0x0002A890 File Offset: 0x00028A90
		public static float distance(float aX, float aY, float bX, float bY)
		{
			float arg_07_0 = aX - bX;
			float num = aY - bY;
			double arg_0C_0 = (double)(arg_07_0 * arg_07_0);
			float expr_0A = num;
			return (float)Math.Sqrt(arg_0C_0 + (double)(expr_0A * expr_0A));
		}

		// Token: 0x06000527 RID: 1319 RVA: 0x0002A8B4 File Offset: 0x00028AB4
		public static float distance(int aX, int aY, int bX, int bY)
		{
			int arg_07_0 = aX - bX;
			int num = aY - bY;
			double arg_0C_0 = (double)(arg_07_0 * arg_07_0);
			int expr_0A = num;
			return (float)Math.Sqrt(arg_0C_0 + (double)(expr_0A * expr_0A));
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x0002A884 File Offset: 0x00028A84
		public static int round(float d)
		{
			return (int)(d + 0.5f);
		}
	}
}
