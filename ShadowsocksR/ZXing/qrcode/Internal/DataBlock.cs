using System;

namespace ZXing.QrCode.Internal
{
	// Token: 0x0200006E RID: 110
	internal sealed class DataBlock
	{
		// Token: 0x060003EF RID: 1007 RVA: 0x00020DB1 File Offset: 0x0001EFB1
		private DataBlock(int numDataCodewords, byte[] codewords)
		{
			this.numDataCodewords = numDataCodewords;
			this.codewords = codewords;
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x00020DC8 File Offset: 0x0001EFC8
		internal static DataBlock[] getDataBlocks(byte[] rawCodewords, Version version, ErrorCorrectionLevel ecLevel)
		{
			if (rawCodewords.Length != version.TotalCodewords)
			{
				throw new ArgumentException();
			}
			Version.ECBlocks eCBlocksForLevel = version.getECBlocksForLevel(ecLevel);
			int num = 0;
			Version.ECB[] eCBlocks = eCBlocksForLevel.getECBlocks();
			Version.ECB[] array = eCBlocks;
			for (int i = 0; i < array.Length; i++)
			{
				Version.ECB eCB = array[i];
				num += eCB.Count;
			}
			DataBlock[] array2 = new DataBlock[num];
			int num2 = 0;
			array = eCBlocks;
			for (int i = 0; i < array.Length; i++)
			{
				Version.ECB eCB2 = array[i];
				for (int j = 0; j < eCB2.Count; j++)
				{
					int dataCodewords = eCB2.DataCodewords;
					int num3 = eCBlocksForLevel.ECCodewordsPerBlock + dataCodewords;
					array2[num2++] = new DataBlock(dataCodewords, new byte[num3]);
				}
			}
			int num4 = array2[0].codewords.Length;
			int num5 = array2.Length - 1;
			while (num5 >= 0 && array2[num5].codewords.Length != num4)
			{
				num5--;
			}
			num5++;
			int num6 = num4 - eCBlocksForLevel.ECCodewordsPerBlock;
			int num7 = 0;
			for (int k = 0; k < num6; k++)
			{
				for (int l = 0; l < num2; l++)
				{
					array2[l].codewords[k] = rawCodewords[num7++];
				}
			}
			for (int m = num5; m < num2; m++)
			{
				array2[m].codewords[num6] = rawCodewords[num7++];
			}
			int num8 = array2[0].codewords.Length;
			for (int n = num6; n < num8; n++)
			{
				for (int num9 = 0; num9 < num2; num9++)
				{
					int num10 = (num9 < num5) ? n : (n + 1);
					array2[num9].codewords[num10] = rawCodewords[num7++];
				}
			}
			return array2;
		}

		// Token: 0x17000047 RID: 71
		internal byte[] Codewords
		{
			// Token: 0x060003F2 RID: 1010 RVA: 0x00020F84 File Offset: 0x0001F184
			get
			{
				return this.codewords;
			}
		}

		// Token: 0x17000046 RID: 70
		internal int NumDataCodewords
		{
			// Token: 0x060003F1 RID: 1009 RVA: 0x00020F7C File Offset: 0x0001F17C
			get
			{
				return this.numDataCodewords;
			}
		}

		// Token: 0x040002D8 RID: 728
		private readonly byte[] codewords;

		// Token: 0x040002D7 RID: 727
		private readonly int numDataCodewords;
	}
}
