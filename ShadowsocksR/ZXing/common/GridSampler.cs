using System;

namespace ZXing.Common
{
	// Token: 0x0200008A RID: 138
	public abstract class GridSampler
	{
		// Token: 0x060004F6 RID: 1270 RVA: 0x00028100 File Offset: 0x00026300
		protected internal static bool checkAndNudgePoints(BitMatrix image, float[] points)
		{
			int width = image.Width;
			int height = image.Height;
			bool flag = true;
			int num = 0;
			while (num < points.Length & flag)
			{
				int num2 = (int)points[num];
				int num3 = (int)points[num + 1];
				if (num2 < -1 || num2 > width || num3 < -1 || num3 > height)
				{
					return false;
				}
				flag = false;
				if (num2 == -1)
				{
					points[num] = 0f;
					flag = true;
				}
				else if (num2 == width)
				{
					points[num] = (float)(width - 1);
					flag = true;
				}
				if (num3 == -1)
				{
					points[num + 1] = 0f;
					flag = true;
				}
				else if (num3 == height)
				{
					points[num + 1] = (float)(height - 1);
					flag = true;
				}
				num += 2;
			}
			flag = true;
			int num4 = points.Length - 2;
			while (num4 >= 0 & flag)
			{
				int num5 = (int)points[num4];
				int num6 = (int)points[num4 + 1];
				if (num5 < -1 || num5 > width || num6 < -1 || num6 > height)
				{
					return false;
				}
				flag = false;
				if (num5 == -1)
				{
					points[num4] = 0f;
					flag = true;
				}
				else if (num5 == width)
				{
					points[num4] = (float)(width - 1);
					flag = true;
				}
				if (num6 == -1)
				{
					points[num4 + 1] = 0f;
					flag = true;
				}
				else if (num6 == height)
				{
					points[num4 + 1] = (float)(height - 1);
					flag = true;
				}
				num4 -= 2;
			}
			return true;
		}

		// Token: 0x060004F5 RID: 1269 RVA: 0x000280F8 File Offset: 0x000262F8
		public virtual BitMatrix sampleGrid(BitMatrix image, int dimensionX, int dimensionY, PerspectiveTransform transform)
		{
			throw new NotSupportedException();
		}

		// Token: 0x060004F4 RID: 1268
		public abstract BitMatrix sampleGrid(BitMatrix image, int dimensionX, int dimensionY, float p1ToX, float p1ToY, float p2ToX, float p2ToY, float p3ToX, float p3ToY, float p4ToX, float p4ToY, float p1FromX, float p1FromY, float p2FromX, float p2FromY, float p3FromX, float p3FromY, float p4FromX, float p4FromY);

		// Token: 0x060004F3 RID: 1267 RVA: 0x000280E7 File Offset: 0x000262E7
		public static void setGridSampler(GridSampler newGridSampler)
		{
			if (newGridSampler == null)
			{
				throw new ArgumentException();
			}
			GridSampler.gridSampler = newGridSampler;
		}

		// Token: 0x1700007E RID: 126
		public static GridSampler Instance
		{
			// Token: 0x060004F2 RID: 1266 RVA: 0x000280E0 File Offset: 0x000262E0
			get
			{
				return GridSampler.gridSampler;
			}
		}

		// Token: 0x0400034B RID: 843
		private static GridSampler gridSampler = new DefaultGridSampler();
	}
}
