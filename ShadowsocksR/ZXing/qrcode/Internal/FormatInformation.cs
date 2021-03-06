﻿using System;

namespace ZXing.QrCode.Internal
{
	// Token: 0x02000073 RID: 115
	internal sealed class FormatInformation
	{
		// Token: 0x06000415 RID: 1045 RVA: 0x00021B14 File Offset: 0x0001FD14
		static FormatInformation()
		{
			// Note: this type is marked as 'beforefieldinit'.
			int[][] expr_07 = new int[32][];
			int arg_17_1 = 0;
			int[] expr_0F = new int[2];
			expr_0F[0] = 21522;
			expr_07[arg_17_1] = expr_0F;
			expr_07[1] = new int[]
			{
				20773,
				1
			};
			expr_07[2] = new int[]
			{
				24188,
				2
			};
			expr_07[3] = new int[]
			{
				23371,
				3
			};
			expr_07[4] = new int[]
			{
				17913,
				4
			};
			expr_07[5] = new int[]
			{
				16590,
				5
			};
			expr_07[6] = new int[]
			{
				20375,
				6
			};
			expr_07[7] = new int[]
			{
				19104,
				7
			};
			expr_07[8] = new int[]
			{
				30660,
				8
			};
			expr_07[9] = new int[]
			{
				29427,
				9
			};
			expr_07[10] = new int[]
			{
				32170,
				10
			};
			expr_07[11] = new int[]
			{
				30877,
				11
			};
			expr_07[12] = new int[]
			{
				26159,
				12
			};
			expr_07[13] = new int[]
			{
				25368,
				13
			};
			expr_07[14] = new int[]
			{
				27713,
				14
			};
			expr_07[15] = new int[]
			{
				26998,
				15
			};
			expr_07[16] = new int[]
			{
				5769,
				16
			};
			expr_07[17] = new int[]
			{
				5054,
				17
			};
			expr_07[18] = new int[]
			{
				7399,
				18
			};
			expr_07[19] = new int[]
			{
				6608,
				19
			};
			expr_07[20] = new int[]
			{
				1890,
				20
			};
			expr_07[21] = new int[]
			{
				597,
				21
			};
			expr_07[22] = new int[]
			{
				3340,
				22
			};
			expr_07[23] = new int[]
			{
				2107,
				23
			};
			expr_07[24] = new int[]
			{
				13663,
				24
			};
			expr_07[25] = new int[]
			{
				12392,
				25
			};
			expr_07[26] = new int[]
			{
				16177,
				26
			};
			expr_07[27] = new int[]
			{
				14854,
				27
			};
			expr_07[28] = new int[]
			{
				9396,
				28
			};
			expr_07[29] = new int[]
			{
				8579,
				29
			};
			expr_07[30] = new int[]
			{
				11994,
				30
			};
			expr_07[31] = new int[]
			{
				11245,
				31
			};
			FormatInformation.FORMAT_INFO_DECODE_LOOKUP = expr_07;
			FormatInformation.BITS_SET_IN_HALF_BYTE = new int[]
			{
				0,
				1,
				1,
				2,
				1,
				2,
				2,
				3,
				1,
				2,
				2,
				3,
				2,
				3,
				3,
				4
			};
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x0002195C File Offset: 0x0001FB5C
		private FormatInformation(int formatInfo)
		{
			this.errorCorrectionLevel = ErrorCorrectionLevel.forBits(formatInfo >> 3 & 3);
			this.dataMask = (byte)(formatInfo & 7);
		}

		// Token: 0x0600040F RID: 1039 RVA: 0x000219FC File Offset: 0x0001FBFC
		internal static FormatInformation decodeFormatInformation(int maskedFormatInfo1, int maskedFormatInfo2)
		{
			FormatInformation formatInformation = FormatInformation.doDecodeFormatInformation(maskedFormatInfo1, maskedFormatInfo2);
			if (formatInformation != null)
			{
				return formatInformation;
			}
			return FormatInformation.doDecodeFormatInformation(maskedFormatInfo1 ^ 21522, maskedFormatInfo2 ^ 21522);
		}

		// Token: 0x06000410 RID: 1040 RVA: 0x00021A2C File Offset: 0x0001FC2C
		private static FormatInformation doDecodeFormatInformation(int maskedFormatInfo1, int maskedFormatInfo2)
		{
			int num = 2147483647;
			int formatInfo = 0;
			int[][] fORMAT_INFO_DECODE_LOOKUP = FormatInformation.FORMAT_INFO_DECODE_LOOKUP;
			for (int i = 0; i < fORMAT_INFO_DECODE_LOOKUP.Length; i++)
			{
				int[] array = fORMAT_INFO_DECODE_LOOKUP[i];
				int num2 = array[0];
				if (num2 == maskedFormatInfo1 || num2 == maskedFormatInfo2)
				{
					return new FormatInformation(array[1]);
				}
				int num3 = FormatInformation.numBitsDiffering(maskedFormatInfo1, num2);
				if (num3 < num)
				{
					formatInfo = array[1];
					num = num3;
				}
				if (maskedFormatInfo1 != maskedFormatInfo2)
				{
					num3 = FormatInformation.numBitsDiffering(maskedFormatInfo2, num2);
					if (num3 < num)
					{
						formatInfo = array[1];
						num = num3;
					}
				}
			}
			if (num <= 3)
			{
				return new FormatInformation(formatInfo);
			}
			return null;
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x00021AD8 File Offset: 0x0001FCD8
		public override bool Equals(object o)
		{
			if (!(o is FormatInformation))
			{
				return false;
			}
			FormatInformation formatInformation = (FormatInformation)o;
			return this.errorCorrectionLevel == formatInformation.errorCorrectionLevel && this.dataMask == formatInformation.dataMask;
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x00021AC2 File Offset: 0x0001FCC2
		public override int GetHashCode()
		{
			return this.errorCorrectionLevel.ordinal() << 3 | (int)this.dataMask;
		}

		// Token: 0x0600040E RID: 1038 RVA: 0x00021980 File Offset: 0x0001FB80
		internal static int numBitsDiffering(int a, int b)
		{
			a ^= b;
			return FormatInformation.BITS_SET_IN_HALF_BYTE[a & 15] + FormatInformation.BITS_SET_IN_HALF_BYTE[(int)((uint)a >> 4 & 15u)] + FormatInformation.BITS_SET_IN_HALF_BYTE[(int)((uint)a >> 8 & 15u)] + FormatInformation.BITS_SET_IN_HALF_BYTE[(int)((uint)a >> 12 & 15u)] + FormatInformation.BITS_SET_IN_HALF_BYTE[(int)((uint)a >> 16 & 15u)] + FormatInformation.BITS_SET_IN_HALF_BYTE[(int)((uint)a >> 20 & 15u)] + FormatInformation.BITS_SET_IN_HALF_BYTE[(int)((uint)a >> 24 & 15u)] + FormatInformation.BITS_SET_IN_HALF_BYTE[(int)((uint)a >> 28 & 15u)];
		}

		// Token: 0x1700004B RID: 75
		internal byte DataMask
		{
			// Token: 0x06000412 RID: 1042 RVA: 0x00021ABA File Offset: 0x0001FCBA
			get
			{
				return this.dataMask;
			}
		}

		// Token: 0x1700004A RID: 74
		internal ErrorCorrectionLevel ErrorCorrectionLevel
		{
			// Token: 0x06000411 RID: 1041 RVA: 0x00021AB2 File Offset: 0x0001FCB2
			get
			{
				return this.errorCorrectionLevel;
			}
		}

		// Token: 0x040002E7 RID: 743
		private static readonly int[] BITS_SET_IN_HALF_BYTE;

		// Token: 0x040002E9 RID: 745
		private readonly byte dataMask;

		// Token: 0x040002E8 RID: 744
		private readonly ErrorCorrectionLevel errorCorrectionLevel;

		// Token: 0x040002E6 RID: 742
		private static readonly int[][] FORMAT_INFO_DECODE_LOOKUP;

		// Token: 0x040002E5 RID: 741
		private const int FORMAT_INFO_MASK_QR = 21522;
	}
}
