using System;

namespace ZXing.Common
{
	// Token: 0x02000089 RID: 137
	public sealed class HybridBinarizer : GlobalHistogramBinarizer
	{
		// Token: 0x060004F1 RID: 1265 RVA: 0x00028FF5 File Offset: 0x000271F5
		public HybridBinarizer(LuminanceSource source) : base(source)
		{
		}

		// Token: 0x060004F3 RID: 1267 RVA: 0x00029008 File Offset: 0x00027208
		private void binarizeEntireImage()
		{
			if (this.matrix == null)
			{
				LuminanceSource luminanceSource = this.LuminanceSource;
				int width = luminanceSource.Width;
				int height = luminanceSource.Height;
				if (width >= 40 && height >= 40)
				{
					byte[] arg_4A_0 = luminanceSource.Matrix;
					int num = width >> 3;
					if ((width & 7) != 0)
					{
						num++;
					}
					int num2 = height >> 3;
					if ((height & 7) != 0)
					{
						num2++;
					}
					int[][] blackPoints = HybridBinarizer.calculateBlackPoints(arg_4A_0, num, num2, width, height);
					BitMatrix bitMatrix = new BitMatrix(width, height);
					HybridBinarizer.calculateThresholdForBlock(arg_4A_0, num, num2, width, height, blackPoints, bitMatrix);
					this.matrix = bitMatrix;
					return;
				}
				this.matrix = base.BlackMatrix;
			}
		}

		// Token: 0x060004F7 RID: 1271 RVA: 0x000291D4 File Offset: 0x000273D4
		private static int[][] calculateBlackPoints(byte[] luminances, int subWidth, int subHeight, int width, int height)
		{
			int[][] array = new int[subHeight][];
			for (int i = 0; i < subHeight; i++)
			{
				array[i] = new int[subWidth];
			}
			for (int j = 0; j < subHeight; j++)
			{
				int num = j << 3;
				int num2 = height - 8;
				if (num > num2)
				{
					num = num2;
				}
				for (int k = 0; k < subWidth; k++)
				{
					int num3 = k << 3;
					int num4 = width - 8;
					if (num3 > num4)
					{
						num3 = num4;
					}
					int num5 = 0;
					int num6 = 255;
					int num7 = 0;
					int l = 0;
					int num8 = num * width + num3;
					while (l < 8)
					{
						for (int m = 0; m < 8; m++)
						{
							int num9 = (int)(luminances[num8 + m] & 255);
							num5 += num9;
							if (num9 < num6)
							{
								num6 = num9;
							}
							if (num9 > num7)
							{
								num7 = num9;
							}
						}
						if (num7 - num6 > 24)
						{
							l++;
							num8 += width;
							while (l < 8)
							{
								for (int n = 0; n < 8; n++)
								{
									num5 += (int)(luminances[num8 + n] & 255);
								}
								l++;
								num8 += width;
							}
						}
						l++;
						num8 += width;
					}
					int num10 = num5 >> 6;
					if (num7 - num6 <= 24)
					{
						num10 = num6 >> 1;
						if (j > 0 && k > 0)
						{
							int num11 = array[j - 1][k] + 2 * array[j][k - 1] + array[j - 1][k - 1] >> 2;
							if (num6 < num11)
							{
								num10 = num11;
							}
						}
					}
					array[j][k] = num10;
				}
			}
			return array;
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x00029098 File Offset: 0x00027298
		private static void calculateThresholdForBlock(byte[] luminances, int subWidth, int subHeight, int width, int height, int[][] blackPoints, BitMatrix matrix)
		{
			for (int i = 0; i < subHeight; i++)
			{
				int num = i << 3;
				int num2 = height - 8;
				if (num > num2)
				{
					num = num2;
				}
				for (int j = 0; j < subWidth; j++)
				{
					int num3 = j << 3;
					int num4 = width - 8;
					if (num3 > num4)
					{
						num3 = num4;
					}
					int num5 = HybridBinarizer.cap(j, 2, subWidth - 3);
					int num6 = HybridBinarizer.cap(i, 2, subHeight - 3);
					int num7 = 0;
					for (int k = -2; k <= 2; k++)
					{
						int[] array = blackPoints[num6 + k];
						num7 += array[num5 - 2];
						num7 += array[num5 - 1];
						num7 += array[num5];
						num7 += array[num5 + 1];
						num7 += array[num5 + 2];
					}
					int threshold = num7 / 25;
					HybridBinarizer.thresholdBlock(luminances, num3, num, threshold, width, matrix);
				}
			}
		}

		// Token: 0x060004F5 RID: 1269 RVA: 0x00029171 File Offset: 0x00027371
		private static int cap(int value, int min, int max)
		{
			if (value < min)
			{
				return min;
			}
			if (value <= max)
			{
				return value;
			}
			return max;
		}

		// Token: 0x060004F2 RID: 1266 RVA: 0x00028FFE File Offset: 0x000271FE
		public override Binarizer createBinarizer(LuminanceSource source)
		{
			return new HybridBinarizer(source);
		}

		// Token: 0x060004F6 RID: 1270 RVA: 0x00029180 File Offset: 0x00027380
		private static void thresholdBlock(byte[] luminances, int xoffset, int yoffset, int threshold, int stride, BitMatrix matrix)
		{
			int num = yoffset * stride + xoffset;
			int i = 0;
			while (i < 8)
			{
				for (int j = 0; j < 8; j++)
				{
					int num2 = (int)(luminances[num + j] & 255);
					matrix[xoffset + j, yoffset + i] = (num2 <= threshold);
				}
				i++;
				num += stride;
			}
		}

		// Token: 0x1700007C RID: 124
		public override BitMatrix BlackMatrix
		{
			// Token: 0x060004F0 RID: 1264 RVA: 0x00028FE7 File Offset: 0x000271E7
			get
			{
				this.binarizeEntireImage();
				return this.matrix;
			}
		}

		// Token: 0x04000350 RID: 848
		private const int BLOCK_SIZE = 8;

		// Token: 0x04000351 RID: 849
		private const int BLOCK_SIZE_MASK = 7;

		// Token: 0x0400034F RID: 847
		private const int BLOCK_SIZE_POWER = 3;

		// Token: 0x04000354 RID: 852
		private BitMatrix matrix;

		// Token: 0x04000352 RID: 850
		private const int MINIMUM_DIMENSION = 40;

		// Token: 0x04000353 RID: 851
		private const int MIN_DYNAMIC_RANGE = 24;
	}
}
