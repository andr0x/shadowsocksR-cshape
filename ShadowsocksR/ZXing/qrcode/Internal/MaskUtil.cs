using System;

namespace ZXing.QrCode.Internal
{
	// Token: 0x02000080 RID: 128
	public static class MaskUtil
	{
		// Token: 0x0600047C RID: 1148 RVA: 0x00026213 File Offset: 0x00024413
		public static int applyMaskPenaltyRule1(ByteMatrix matrix)
		{
			return MaskUtil.applyMaskPenaltyRule1Internal(matrix, true) + MaskUtil.applyMaskPenaltyRule1Internal(matrix, false);
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x0002657C File Offset: 0x0002477C
		private static int applyMaskPenaltyRule1Internal(ByteMatrix matrix, bool isHorizontal)
		{
			int num = 0;
			int num2 = isHorizontal ? matrix.Height : matrix.Width;
			int num3 = isHorizontal ? matrix.Width : matrix.Height;
			byte[][] array = matrix.Array;
			for (int i = 0; i < num2; i++)
			{
				int num4 = 0;
				int num5 = -1;
				for (int j = 0; j < num3; j++)
				{
					int num6 = (int)(isHorizontal ? array[i][j] : array[j][i]);
					if (num6 == num5)
					{
						num4++;
					}
					else
					{
						if (num4 >= 5)
						{
							num += 3 + (num4 - 5);
						}
						num4 = 1;
						num5 = num6;
					}
				}
				if (num4 >= 5)
				{
					num += 3 + (num4 - 5);
				}
			}
			return num;
		}

		// Token: 0x0600047D RID: 1149 RVA: 0x00026224 File Offset: 0x00024424
		public static int applyMaskPenaltyRule2(ByteMatrix matrix)
		{
			int num = 0;
			byte[][] array = matrix.Array;
			int width = matrix.Width;
			int height = matrix.Height;
			for (int i = 0; i < height - 1; i++)
			{
				for (int j = 0; j < width - 1; j++)
				{
					int num2 = (int)array[i][j];
					if (num2 == (int)array[i][j + 1] && num2 == (int)array[i + 1][j] && num2 == (int)array[i + 1][j + 1])
					{
						num++;
					}
				}
			}
			return 3 * num;
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x000262A8 File Offset: 0x000244A8
		public static int applyMaskPenaltyRule3(ByteMatrix matrix)
		{
			int num = 0;
			byte[][] array = matrix.Array;
			int width = matrix.Width;
			int height = matrix.Height;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					byte[] array2 = array[i];
					if (j + 6 < width && array2[j] == 1 && array2[j + 1] == 0 && array2[j + 2] == 1 && array2[j + 3] == 1 && array2[j + 4] == 1 && array2[j + 5] == 0 && array2[j + 6] == 1 && (MaskUtil.isWhiteHorizontal(array2, j - 4, j) || MaskUtil.isWhiteHorizontal(array2, j + 7, j + 11)))
					{
						num++;
					}
					if (i + 6 < height && array[i][j] == 1 && array[i + 1][j] == 0 && array[i + 2][j] == 1 && array[i + 3][j] == 1 && array[i + 4][j] == 1 && array[i + 5][j] == 0 && array[i + 6][j] == 1 && (MaskUtil.isWhiteVertical(array, j, i - 4, i) || MaskUtil.isWhiteVertical(array, j, i + 7, i + 11)))
					{
						num++;
					}
				}
			}
			return num * 40;
		}

		// Token: 0x06000481 RID: 1153 RVA: 0x0002644C File Offset: 0x0002464C
		public static int applyMaskPenaltyRule4(ByteMatrix matrix)
		{
			int num = 0;
			byte[][] array = matrix.Array;
			int width = matrix.Width;
			int height = matrix.Height;
			for (int i = 0; i < height; i++)
			{
				byte[] array2 = array[i];
				for (int j = 0; j < width; j++)
				{
					if (array2[j] == 1)
					{
						num++;
					}
				}
			}
			int num2 = matrix.Height * matrix.Width;
			return (int)(Math.Abs((double)num / (double)num2 - 0.5) * 20.0) * 10;
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x000264D4 File Offset: 0x000246D4
		public static bool getDataMaskBit(int maskPattern, int x, int y)
		{
			int num;
			switch (maskPattern)
			{
			case 0:
				num = (y + x & 1);
				break;
			case 1:
				num = (y & 1);
				break;
			case 2:
				num = x % 3;
				break;
			case 3:
				num = (y + x) % 3;
				break;
			case 4:
				num = (int)(((uint)y >> 1) + (uint)(x / 3) & 1u);
				break;
			case 5:
			{
				int num2 = y * x;
				num = (num2 & 1) + num2 % 3;
				break;
			}
			case 6:
			{
				int num2 = y * x;
				num = ((num2 & 1) + num2 % 3 & 1);
				break;
			}
			case 7:
			{
				int num2 = y * x;
				num = (num2 % 3 + (y + x & 1) & 1);
				break;
			}
			default:
				throw new ArgumentException("Invalid mask pattern: " + maskPattern);
			}
			return num == 0;
		}

		// Token: 0x0600047F RID: 1151 RVA: 0x000263F0 File Offset: 0x000245F0
		private static bool isWhiteHorizontal(byte[] rowArray, int from, int to)
		{
			for (int i = from; i < to; i++)
			{
				if (i >= 0 && i < rowArray.Length && rowArray[i] == 1)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000480 RID: 1152 RVA: 0x0002641C File Offset: 0x0002461C
		private static bool isWhiteVertical(byte[][] array, int col, int from, int to)
		{
			for (int i = from; i < to; i++)
			{
				if (i >= 0 && i < array.Length && array[i][col] == 1)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0400031F RID: 799
		private const int N1 = 3;

		// Token: 0x04000320 RID: 800
		private const int N2 = 3;

		// Token: 0x04000321 RID: 801
		private const int N3 = 40;

		// Token: 0x04000322 RID: 802
		private const int N4 = 10;
	}
}
