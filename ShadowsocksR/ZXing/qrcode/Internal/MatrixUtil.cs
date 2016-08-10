using System;
using ZXing.Common;

namespace ZXing.QrCode.Internal
{
	// Token: 0x0200007F RID: 127
	public static class MatrixUtil
	{
		// Token: 0x0600048E RID: 1166 RVA: 0x00027A0C File Offset: 0x00025C0C
		static MatrixUtil()
		{
			// Note: this type is marked as 'beforefieldinit'.
			int[][] expr_44C = new int[15][];
			int arg_458_1 = 0;
			int[] expr_454 = new int[2];
			expr_454[0] = 8;
			expr_44C[arg_458_1] = expr_454;
			expr_44C[1] = new int[]
			{
				8,
				1
			};
			expr_44C[2] = new int[]
			{
				8,
				2
			};
			expr_44C[3] = new int[]
			{
				8,
				3
			};
			expr_44C[4] = new int[]
			{
				8,
				4
			};
			expr_44C[5] = new int[]
			{
				8,
				5
			};
			expr_44C[6] = new int[]
			{
				8,
				7
			};
			expr_44C[7] = new int[]
			{
				8,
				8
			};
			expr_44C[8] = new int[]
			{
				7,
				8
			};
			expr_44C[9] = new int[]
			{
				5,
				8
			};
			expr_44C[10] = new int[]
			{
				4,
				8
			};
			expr_44C[11] = new int[]
			{
				3,
				8
			};
			expr_44C[12] = new int[]
			{
				2,
				8
			};
			expr_44C[13] = new int[]
			{
				1,
				8
			};
			expr_44C[14] = new int[]
			{
				0,
				8
			};
			MatrixUtil.TYPE_INFO_COORDINATES = expr_44C;
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x000273E4 File Offset: 0x000255E4
		public static void buildMatrix(BitArray dataBits, ErrorCorrectionLevel ecLevel, Version version, int maskPattern, ByteMatrix matrix)
		{
			MatrixUtil.clearMatrix(matrix);
			MatrixUtil.embedBasicPatterns(version, matrix);
			MatrixUtil.embedTypeInfo(ecLevel, maskPattern, matrix);
			MatrixUtil.maybeEmbedVersionInfo(version, matrix);
			MatrixUtil.embedDataBits(dataBits, maskPattern, matrix);
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x00027668 File Offset: 0x00025868
		public static int calculateBCHCode(int value, int poly)
		{
			int num = MatrixUtil.findMSBSet(poly);
			value <<= num - 1;
			while (MatrixUtil.findMSBSet(value) >= num)
			{
				value ^= poly << MatrixUtil.findMSBSet(value) - num;
			}
			return value;
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x000273DB File Offset: 0x000255DB
		public static void clearMatrix(ByteMatrix matrix)
		{
			matrix.clear(2);
		}

		// Token: 0x0600047D RID: 1149 RVA: 0x0002740F File Offset: 0x0002560F
		public static void embedBasicPatterns(Version version, ByteMatrix matrix)
		{
			MatrixUtil.embedPositionDetectionPatternsAndSeparators(matrix);
			MatrixUtil.embedDarkDotAtLeftBottomCorner(matrix);
			MatrixUtil.maybeEmbedPositionAdjustmentPatterns(version, matrix);
			MatrixUtil.embedTimingPatterns(matrix);
		}

		// Token: 0x06000487 RID: 1159 RVA: 0x000277E2 File Offset: 0x000259E2
		private static void embedDarkDotAtLeftBottomCorner(ByteMatrix matrix)
		{
			if (matrix[8, matrix.Height - 8] == 0)
			{
				throw new WriterException();
			}
			matrix[8, matrix.Height - 8] = 1;
		}

		// Token: 0x06000480 RID: 1152 RVA: 0x00027548 File Offset: 0x00025748
		public static void embedDataBits(BitArray dataBits, int maskPattern, ByteMatrix matrix)
		{
			int num = 0;
			int num2 = -1;
			int i = matrix.Width - 1;
			int num3 = matrix.Height - 1;
			while (i > 0)
			{
				if (i == 6)
				{
					i--;
				}
				while (num3 >= 0 && num3 < matrix.Height)
				{
					for (int j = 0; j < 2; j++)
					{
						int x = i - j;
						if (MatrixUtil.isEmpty(matrix[x, num3]))
						{
							int num4;
							if (num < dataBits.Size)
							{
								num4 = (dataBits[num] ? 1 : 0);
								num++;
							}
							else
							{
								num4 = 0;
							}
							if (maskPattern != -1 && MaskUtil.getDataMaskBit(maskPattern, x, num3))
							{
								num4 ^= 1;
							}
							matrix[x, num3] = num4;
						}
					}
					num3 += num2;
				}
				num2 = -num2;
				num3 += num2;
				i -= 2;
			}
			if (num != dataBits.Size)
			{
				throw new WriterException(string.Concat(new object[]
				{
					"Not all bits consumed: ",
					num,
					"/",
					dataBits.Size
				}));
			}
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x0002780C File Offset: 0x00025A0C
		private static void embedHorizontalSeparationPattern(int xStart, int yStart, ByteMatrix matrix)
		{
			for (int i = 0; i < 8; i++)
			{
				if (!MatrixUtil.isEmpty(matrix[xStart + i, yStart]))
				{
					throw new WriterException();
				}
				matrix[xStart + i, yStart] = 0;
			}
		}

		// Token: 0x0600048A RID: 1162 RVA: 0x00027884 File Offset: 0x00025A84
		private static void embedPositionAdjustmentPattern(int xStart, int yStart, ByteMatrix matrix)
		{
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					matrix[xStart + j, yStart + i] = MatrixUtil.POSITION_ADJUSTMENT_PATTERN[i][j];
				}
			}
		}

		// Token: 0x0600048B RID: 1163 RVA: 0x000278C0 File Offset: 0x00025AC0
		private static void embedPositionDetectionPattern(int xStart, int yStart, ByteMatrix matrix)
		{
			for (int i = 0; i < 7; i++)
			{
				for (int j = 0; j < 7; j++)
				{
					matrix[xStart + j, yStart + i] = MatrixUtil.POSITION_DETECTION_PATTERN[i][j];
				}
			}
		}

		// Token: 0x0600048C RID: 1164 RVA: 0x000278FC File Offset: 0x00025AFC
		private static void embedPositionDetectionPatternsAndSeparators(ByteMatrix matrix)
		{
			int num = MatrixUtil.POSITION_DETECTION_PATTERN[0].Length;
			MatrixUtil.embedPositionDetectionPattern(0, 0, matrix);
			MatrixUtil.embedPositionDetectionPattern(matrix.Width - num, 0, matrix);
			MatrixUtil.embedPositionDetectionPattern(0, matrix.Width - num, matrix);
			MatrixUtil.embedHorizontalSeparationPattern(0, 7, matrix);
			MatrixUtil.embedHorizontalSeparationPattern(matrix.Width - 8, 7, matrix);
			MatrixUtil.embedHorizontalSeparationPattern(0, matrix.Width - 8, matrix);
			MatrixUtil.embedVerticalSeparationPattern(7, 0, matrix);
			MatrixUtil.embedVerticalSeparationPattern(matrix.Height - 7 - 1, 0, matrix);
			MatrixUtil.embedVerticalSeparationPattern(7, matrix.Height - 7, matrix);
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x0002778C File Offset: 0x0002598C
		private static void embedTimingPatterns(ByteMatrix matrix)
		{
			for (int i = 8; i < matrix.Width - 8; i++)
			{
				int value = (i + 1) % 2;
				if (MatrixUtil.isEmpty(matrix[i, 6]))
				{
					matrix[i, 6] = value;
				}
				if (MatrixUtil.isEmpty(matrix[6, i]))
				{
					matrix[6, i] = value;
				}
			}
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x0002742C File Offset: 0x0002562C
		public static void embedTypeInfo(ErrorCorrectionLevel ecLevel, int maskPattern, ByteMatrix matrix)
		{
			BitArray bitArray = new BitArray();
			MatrixUtil.makeTypeInfoBits(ecLevel, maskPattern, bitArray);
			for (int i = 0; i < bitArray.Size; i++)
			{
				BitArray expr_13 = bitArray;
				int value = expr_13[expr_13.Size - 1 - i] ? 1 : 0;
				int x = MatrixUtil.TYPE_INFO_COORDINATES[i][0];
				int y = MatrixUtil.TYPE_INFO_COORDINATES[i][1];
				matrix[x, y] = value;
				if (i < 8)
				{
					int x2 = matrix.Width - i - 1;
					int y2 = 8;
					matrix[x2, y2] = value;
				}
				else
				{
					int x3 = 8;
					int y3 = matrix.Height - 7 + (i - 8);
					matrix[x3, y3] = value;
				}
			}
		}

		// Token: 0x06000489 RID: 1161 RVA: 0x00027848 File Offset: 0x00025A48
		private static void embedVerticalSeparationPattern(int xStart, int yStart, ByteMatrix matrix)
		{
			for (int i = 0; i < 7; i++)
			{
				if (!MatrixUtil.isEmpty(matrix[xStart, yStart + i]))
				{
					throw new WriterException();
				}
				matrix[xStart, yStart + i] = 0;
			}
		}

		// Token: 0x06000481 RID: 1153 RVA: 0x00027648 File Offset: 0x00025848
		public static int findMSBSet(int value_Renamed)
		{
			int num = 0;
			while (value_Renamed != 0)
			{
				value_Renamed = (int)((uint)value_Renamed >> 1);
				num++;
			}
			return num;
		}

		// Token: 0x06000485 RID: 1157 RVA: 0x00027785 File Offset: 0x00025985
		private static bool isEmpty(int value)
		{
			return value == 2;
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x000276A4 File Offset: 0x000258A4
		public static void makeTypeInfoBits(ErrorCorrectionLevel ecLevel, int maskPattern, BitArray bits)
		{
			if (!QRCode.isValidMaskPattern(maskPattern))
			{
				throw new WriterException("Invalid mask pattern");
			}
			int value = ecLevel.Bits << 3 | maskPattern;
			bits.appendBits(value, 5);
			int value2 = MatrixUtil.calculateBCHCode(value, 1335);
			bits.appendBits(value2, 10);
			BitArray bitArray = new BitArray();
			bitArray.appendBits(21522, 15);
			bits.xor(bitArray);
			if (bits.Size != 15)
			{
				throw new WriterException("should not happen but we got: " + bits.Size);
			}
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x0002772C File Offset: 0x0002592C
		public static void makeVersionInfoBits(Version version, BitArray bits)
		{
			bits.appendBits(version.VersionNumber, 6);
			int value = MatrixUtil.calculateBCHCode(version.VersionNumber, 7973);
			bits.appendBits(value, 12);
			if (bits.Size != 18)
			{
				throw new WriterException("should not happen but we got: " + bits.Size);
			}
		}

		// Token: 0x0600048D RID: 1165 RVA: 0x00027988 File Offset: 0x00025B88
		private static void maybeEmbedPositionAdjustmentPatterns(Version version, ByteMatrix matrix)
		{
			if (version.VersionNumber < 2)
			{
				return;
			}
			int num = version.VersionNumber - 1;
			int[] array = MatrixUtil.POSITION_ADJUSTMENT_PATTERN_COORDINATE_TABLE[num];
			int num2 = MatrixUtil.POSITION_ADJUSTMENT_PATTERN_COORDINATE_TABLE[num].Length;
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					int num3 = array[i];
					int num4 = array[j];
					if (num4 != -1 && num3 != -1 && MatrixUtil.isEmpty(matrix[num4, num3]))
					{
						MatrixUtil.embedPositionAdjustmentPattern(num4 - 2, num3 - 2, matrix);
					}
				}
			}
		}

		// Token: 0x0600047F RID: 1151 RVA: 0x000274CC File Offset: 0x000256CC
		public static void maybeEmbedVersionInfo(Version version, ByteMatrix matrix)
		{
			if (version.VersionNumber < 7)
			{
				return;
			}
			BitArray bitArray = new BitArray();
			MatrixUtil.makeVersionInfoBits(version, bitArray);
			int num = 17;
			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					int value = bitArray[num] ? 1 : 0;
					num--;
					matrix[i, matrix.Height - 11 + j] = value;
					matrix[matrix.Height - 11 + j, i] = value;
				}
			}
		}

		// Token: 0x04000327 RID: 807
		private static readonly int[][] POSITION_ADJUSTMENT_PATTERN = new int[][]
		{
			new int[]
			{
				1,
				1,
				1,
				1,
				1
			},
			new int[]
			{
				1,
				0,
				0,
				0,
				1
			},
			new int[]
			{
				1,
				0,
				1,
				0,
				1
			},
			new int[]
			{
				1,
				0,
				0,
				0,
				1
			},
			new int[]
			{
				1,
				1,
				1,
				1,
				1
			}
		};

		// Token: 0x04000328 RID: 808
		private static readonly int[][] POSITION_ADJUSTMENT_PATTERN_COORDINATE_TABLE = new int[][]
		{
			new int[]
			{
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1
			},
			new int[]
			{
				6,
				18,
				-1,
				-1,
				-1,
				-1,
				-1
			},
			new int[]
			{
				6,
				22,
				-1,
				-1,
				-1,
				-1,
				-1
			},
			new int[]
			{
				6,
				26,
				-1,
				-1,
				-1,
				-1,
				-1
			},
			new int[]
			{
				6,
				30,
				-1,
				-1,
				-1,
				-1,
				-1
			},
			new int[]
			{
				6,
				34,
				-1,
				-1,
				-1,
				-1,
				-1
			},
			new int[]
			{
				6,
				22,
				38,
				-1,
				-1,
				-1,
				-1
			},
			new int[]
			{
				6,
				24,
				42,
				-1,
				-1,
				-1,
				-1
			},
			new int[]
			{
				6,
				26,
				46,
				-1,
				-1,
				-1,
				-1
			},
			new int[]
			{
				6,
				28,
				50,
				-1,
				-1,
				-1,
				-1
			},
			new int[]
			{
				6,
				30,
				54,
				-1,
				-1,
				-1,
				-1
			},
			new int[]
			{
				6,
				32,
				58,
				-1,
				-1,
				-1,
				-1
			},
			new int[]
			{
				6,
				34,
				62,
				-1,
				-1,
				-1,
				-1
			},
			new int[]
			{
				6,
				26,
				46,
				66,
				-1,
				-1,
				-1
			},
			new int[]
			{
				6,
				26,
				48,
				70,
				-1,
				-1,
				-1
			},
			new int[]
			{
				6,
				26,
				50,
				74,
				-1,
				-1,
				-1
			},
			new int[]
			{
				6,
				30,
				54,
				78,
				-1,
				-1,
				-1
			},
			new int[]
			{
				6,
				30,
				56,
				82,
				-1,
				-1,
				-1
			},
			new int[]
			{
				6,
				30,
				58,
				86,
				-1,
				-1,
				-1
			},
			new int[]
			{
				6,
				34,
				62,
				90,
				-1,
				-1,
				-1
			},
			new int[]
			{
				6,
				28,
				50,
				72,
				94,
				-1,
				-1
			},
			new int[]
			{
				6,
				26,
				50,
				74,
				98,
				-1,
				-1
			},
			new int[]
			{
				6,
				30,
				54,
				78,
				102,
				-1,
				-1
			},
			new int[]
			{
				6,
				28,
				54,
				80,
				106,
				-1,
				-1
			},
			new int[]
			{
				6,
				32,
				58,
				84,
				110,
				-1,
				-1
			},
			new int[]
			{
				6,
				30,
				58,
				86,
				114,
				-1,
				-1
			},
			new int[]
			{
				6,
				34,
				62,
				90,
				118,
				-1,
				-1
			},
			new int[]
			{
				6,
				26,
				50,
				74,
				98,
				122,
				-1
			},
			new int[]
			{
				6,
				30,
				54,
				78,
				102,
				126,
				-1
			},
			new int[]
			{
				6,
				26,
				52,
				78,
				104,
				130,
				-1
			},
			new int[]
			{
				6,
				30,
				56,
				82,
				108,
				134,
				-1
			},
			new int[]
			{
				6,
				34,
				60,
				86,
				112,
				138,
				-1
			},
			new int[]
			{
				6,
				30,
				58,
				86,
				114,
				142,
				-1
			},
			new int[]
			{
				6,
				34,
				62,
				90,
				118,
				146,
				-1
			},
			new int[]
			{
				6,
				30,
				54,
				78,
				102,
				126,
				150
			},
			new int[]
			{
				6,
				24,
				50,
				76,
				102,
				128,
				154
			},
			new int[]
			{
				6,
				28,
				54,
				80,
				106,
				132,
				158
			},
			new int[]
			{
				6,
				32,
				58,
				84,
				110,
				136,
				162
			},
			new int[]
			{
				6,
				26,
				54,
				82,
				110,
				138,
				166
			},
			new int[]
			{
				6,
				30,
				58,
				86,
				114,
				142,
				170
			}
		};

		// Token: 0x04000326 RID: 806
		private static readonly int[][] POSITION_DETECTION_PATTERN = new int[][]
		{
			new int[]
			{
				1,
				1,
				1,
				1,
				1,
				1,
				1
			},
			new int[]
			{
				1,
				0,
				0,
				0,
				0,
				0,
				1
			},
			new int[]
			{
				1,
				0,
				1,
				1,
				1,
				0,
				1
			},
			new int[]
			{
				1,
				0,
				1,
				1,
				1,
				0,
				1
			},
			new int[]
			{
				1,
				0,
				1,
				1,
				1,
				0,
				1
			},
			new int[]
			{
				1,
				0,
				0,
				0,
				0,
				0,
				1
			},
			new int[]
			{
				1,
				1,
				1,
				1,
				1,
				1,
				1
			}
		};

		// Token: 0x04000329 RID: 809
		private static readonly int[][] TYPE_INFO_COORDINATES;

		// Token: 0x0400032C RID: 812
		private const int TYPE_INFO_MASK_PATTERN = 21522;

		// Token: 0x0400032B RID: 811
		private const int TYPE_INFO_POLY = 1335;

		// Token: 0x0400032A RID: 810
		private const int VERSION_INFO_POLY = 7973;
	}
}
