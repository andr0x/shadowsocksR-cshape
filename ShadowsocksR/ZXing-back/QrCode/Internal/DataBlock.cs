using System;

namespace ZXing.QrCode.Internal
{
	// Token: 0x0200006C RID: 108
	internal sealed class DataBlock
	{
		// Token: 0x060003E6 RID: 998 RVA: 0x00021B69 File Offset: 0x0001FD69
		private DataBlock(int numDataCodewords, byte[] codewords)
		{
			this.numDataCodewords = numDataCodewords;
			this.codewords = codewords;
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x00021B80 File Offset: 0x0001FD80
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

		// Token: 0x17000044 RID: 68
		internal byte[] Codewords
		{
			// Token: 0x060003E9 RID: 1001 RVA: 0x00021D3C File Offset: 0x0001FF3C
			get
			{
				return this.codewords;
			}
		}

		// Token: 0x17000043 RID: 67
		internal int NumDataCodewords
		{
			// Token: 0x060003E8 RID: 1000 RVA: 0x00021D34 File Offset: 0x0001FF34
			get
			{
				return this.numDataCodewords;
			}
		}

		// Token: 0x040002DB RID: 731
		private readonly byte[] codewords;

		// Token: 0x040002DA RID: 730
		private readonly int numDataCodewords;
	}
}
