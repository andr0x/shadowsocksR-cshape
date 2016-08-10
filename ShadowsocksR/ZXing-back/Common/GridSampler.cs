using System;

namespace ZXing.Common
{
	// Token: 0x02000088 RID: 136
	public abstract class GridSampler
	{
		// Token: 0x060004ED RID: 1261 RVA: 0x00028EB8 File Offset: 0x000270B8
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

		// Token: 0x060004EC RID: 1260 RVA: 0x00028EB0 File Offset: 0x000270B0
		public virtual BitMatrix sampleGrid(BitMatrix image, int dimensionX, int dimensionY, PerspectiveTransform transform)
		{
			throw new NotSupportedException();
		}

		// Token: 0x060004EB RID: 1259
		public abstract BitMatrix sampleGrid(BitMatrix image, int dimensionX, int dimensionY, float p1ToX, float p1ToY, float p2ToX, float p2ToY, float p3ToX, float p3ToY, float p4ToX, float p4ToY, float p1FromX, float p1FromY, float p2FromX, float p2FromY, float p3FromX, float p3FromY, float p4FromX, float p4FromY);

		// Token: 0x060004EA RID: 1258 RVA: 0x00028E9F File Offset: 0x0002709F
		public static void setGridSampler(GridSampler newGridSampler)
		{
			if (newGridSampler == null)
			{
				throw new ArgumentException();
			}
			GridSampler.gridSampler = newGridSampler;
		}

		// Token: 0x1700007B RID: 123
		public static GridSampler Instance
		{
			// Token: 0x060004E9 RID: 1257 RVA: 0x00028E98 File Offset: 0x00027098
			get
			{
				return GridSampler.gridSampler;
			}
		}

		// Token: 0x0400034E RID: 846
		private static GridSampler gridSampler = new DefaultGridSampler();
	}
}
