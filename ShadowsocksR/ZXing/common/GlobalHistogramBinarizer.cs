using System;

namespace ZXing.Common
{
	// Token: 0x02000089 RID: 137
	public class GlobalHistogramBinarizer : Binarizer
	{
		// Token: 0x060004EB RID: 1259 RVA: 0x00027DBC File Offset: 0x00025FBC
		public GlobalHistogramBinarizer(LuminanceSource source) : base(source)
		{
			this.luminances = GlobalHistogramBinarizer.EMPTY;
			this.buckets = new int[32];
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x00027FB7 File Offset: 0x000261B7
		public override Binarizer createBinarizer(LuminanceSource source)
		{
			return new GlobalHistogramBinarizer(source);
		}

		// Token: 0x060004F0 RID: 1264 RVA: 0x00027FFC File Offset: 0x000261FC
		private static bool estimateBlackPoint(int[] buckets, out int blackPoint)
		{
			blackPoint = 0;
			int num = buckets.Length;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < num; i++)
			{
				if (buckets[i] > num4)
				{
					num3 = i;
					num4 = buckets[i];
				}
				if (buckets[i] > num2)
				{
					num2 = buckets[i];
				}
			}
			int num5 = 0;
			int num6 = 0;
			for (int j = 0; j < num; j++)
			{
				int num7 = j - num3;
				int num8 = buckets[j] * num7 * num7;
				if (num8 > num6)
				{
					num5 = j;
					num6 = num8;
				}
			}
			if (num3 > num5)
			{
				int arg_77_0 = num3;
				num3 = num5;
				num5 = arg_77_0;
			}
			if (num5 - num3 <= num >> 4)
			{
				return false;
			}
			int num9 = num5 - 1;
			int num10 = -1;
			for (int k = num5 - 1; k > num3; k--)
			{
				int expr_99 = k - num3;
				int num11 = expr_99 * expr_99 * (num5 - k) * (num2 - buckets[k]);
				if (num11 > num10)
				{
					num9 = k;
					num10 = num11;
				}
			}
			blackPoint = num9 << 3;
			return true;
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x00027DE0 File Offset: 0x00025FE0
		public override BitArray getBlackRow(int y, BitArray row)
		{
			LuminanceSource expr_06 = this.LuminanceSource;
			int width = expr_06.Width;
			if (row == null || row.Size < width)
			{
				row = new BitArray(width);
			}
			else
			{
				row.clear();
			}
			this.initArrays(width);
			byte[] row2 = expr_06.getRow(y, this.luminances);
			int[] array = this.buckets;
			for (int i = 0; i < width; i++)
			{
				int num = (int)(row2[i] & 255);
				array[num >> 3]++;
			}
			int num2;
			if (!GlobalHistogramBinarizer.estimateBlackPoint(array, out num2))
			{
				return null;
			}
			int num3 = (int)(row2[0] & 255);
			int num4 = (int)(row2[1] & 255);
			for (int j = 1; j < width - 1; j++)
			{
				int num5 = (int)(row2[j + 1] & 255);
				int num6 = (num4 << 2) - num3 - num5 >> 1;
				row[j] = (num6 < num2);
				num3 = num4;
				num4 = num5;
			}
			return row;
		}

		// Token: 0x060004EF RID: 1263 RVA: 0x00027FC0 File Offset: 0x000261C0
		private void initArrays(int luminanceSize)
		{
			if (this.luminances.Length < luminanceSize)
			{
				this.luminances = new byte[luminanceSize];
			}
			for (int i = 0; i < 32; i++)
			{
				this.buckets[i] = 0;
			}
		}

		// Token: 0x1700007D RID: 125
		public override BitMatrix BlackMatrix
		{
			// Token: 0x060004ED RID: 1261 RVA: 0x00027EC4 File Offset: 0x000260C4
			get
			{
				LuminanceSource luminanceSource = this.LuminanceSource;
				int width = luminanceSource.Width;
				int height = luminanceSource.Height;
				BitMatrix bitMatrix = new BitMatrix(width, height);
				this.initArrays(width);
				int[] array = this.buckets;
				byte[] array2;
				for (int i = 1; i < 5; i++)
				{
					int y = height * i / 5;
					array2 = luminanceSource.getRow(y, this.luminances);
					int num = (width << 2) / 5;
					for (int j = width / 5; j < num; j++)
					{
						int num2 = (int)(array2[j] & 255);
						array[num2 >> 3]++;
					}
				}
				int num3;
				if (!GlobalHistogramBinarizer.estimateBlackPoint(array, out num3))
				{
					return null;
				}
				array2 = luminanceSource.Matrix;
				for (int k = 0; k < height; k++)
				{
					int num4 = k * width;
					for (int l = 0; l < width; l++)
					{
						int num5 = (int)(array2[num4 + l] & 255);
						bitMatrix[l, k] = (num5 < num3);
					}
				}
				return bitMatrix;
			}
		}

		// Token: 0x0400034A RID: 842
		private readonly int[] buckets;

		// Token: 0x04000348 RID: 840
		private static readonly byte[] EMPTY = new byte[0];

		// Token: 0x04000349 RID: 841
		private byte[] luminances;

		// Token: 0x04000345 RID: 837
		private const int LUMINANCE_BITS = 5;

		// Token: 0x04000347 RID: 839
		private const int LUMINANCE_BUCKETS = 32;

		// Token: 0x04000346 RID: 838
		private const int LUMINANCE_SHIFT = 3;
	}
}
