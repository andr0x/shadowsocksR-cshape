using System;
using ZXing.Common;

namespace ZXing.QrCode.Internal
{
	// Token: 0x0200006D RID: 109
	internal sealed class BitMatrixParser
	{
		// Token: 0x060003E7 RID: 999 RVA: 0x000209B3 File Offset: 0x0001EBB3
		private BitMatrixParser(BitMatrix bitMatrix)
		{
			this.bitMatrix = bitMatrix;
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x00020B94 File Offset: 0x0001ED94
		private int copyBit(int i, int j, int versionBits)
		{
			if (!(this.mirrored ? this.bitMatrix[j, i] : this.bitMatrix[i, j]))
			{
				return versionBits << 1;
			}
			return versionBits << 1 | 1;
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x0002098C File Offset: 0x0001EB8C
		internal static BitMatrixParser createBitMatrixParser(BitMatrix bitMatrix)
		{
			int height = bitMatrix.Height;
			if (height < 21 || (height & 3) != 1)
			{
				return null;
			}
			return new BitMatrixParser(bitMatrix);
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x00020D40 File Offset: 0x0001EF40
		internal void mirror()
		{
			for (int i = 0; i < this.bitMatrix.Width; i++)
			{
				for (int j = i + 1; j < this.bitMatrix.Height; j++)
				{
					if (this.bitMatrix[i, j] != this.bitMatrix[j, i])
					{
						this.bitMatrix.flip(j, i);
						this.bitMatrix.flip(i, j);
					}
				}
			}
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x00020BC8 File Offset: 0x0001EDC8
		internal byte[] readCodewords()
		{
			FormatInformation formatInformation = this.readFormatInformation();
			if (formatInformation == null)
			{
				return null;
			}
			Version version = this.readVersion();
			if (version == null)
			{
				return null;
			}
			DataMask arg_36_0 = DataMask.forReference((int)formatInformation.DataMask);
			int height = this.bitMatrix.Height;
			arg_36_0.unmaskBitMatrix(this.bitMatrix, height);
			BitMatrix bitMatrix = version.buildFunctionPattern();
			bool flag = true;
			byte[] array = new byte[version.TotalCodewords];
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (int i = height - 1; i > 0; i -= 2)
			{
				if (i == 6)
				{
					i--;
				}
				for (int j = 0; j < height; j++)
				{
					int y = flag ? (height - 1 - j) : j;
					for (int k = 0; k < 2; k++)
					{
						if (!bitMatrix[i - k, y])
						{
							num3++;
							num2 <<= 1;
							if (this.bitMatrix[i - k, y])
							{
								num2 |= 1;
							}
							if (num3 == 8)
							{
								array[num++] = (byte)num2;
								num3 = 0;
								num2 = 0;
							}
						}
					}
				}
				flag = !flag;
			}
			if (num != version.TotalCodewords)
			{
				return null;
			}
			return array;
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x000209C4 File Offset: 0x0001EBC4
		internal FormatInformation readFormatInformation()
		{
			if (this.parsedFormatInfo != null)
			{
				return this.parsedFormatInfo;
			}
			int num = 0;
			for (int i = 0; i < 6; i++)
			{
				num = this.copyBit(i, 8, num);
			}
			num = this.copyBit(7, 8, num);
			num = this.copyBit(8, 8, num);
			num = this.copyBit(8, 7, num);
			for (int j = 5; j >= 0; j--)
			{
				num = this.copyBit(8, j, num);
			}
			int height = this.bitMatrix.Height;
			int num2 = 0;
			int num3 = height - 7;
			for (int k = height - 1; k >= num3; k--)
			{
				num2 = this.copyBit(8, k, num2);
			}
			for (int l = height - 8; l < height; l++)
			{
				num2 = this.copyBit(l, 8, num2);
			}
			this.parsedFormatInfo = FormatInformation.decodeFormatInformation(num, num2);
			if (this.parsedFormatInfo != null)
			{
				return this.parsedFormatInfo;
			}
			return null;
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x00020AA0 File Offset: 0x0001ECA0
		internal Version readVersion()
		{
			if (this.parsedVersion != null)
			{
				return this.parsedVersion;
			}
			int height = this.bitMatrix.Height;
			int num = height - 17 >> 2;
			if (num <= 6)
			{
				return Version.getVersionForNumber(num);
			}
			int versionBits = 0;
			int num2 = height - 11;
			for (int i = 5; i >= 0; i--)
			{
				for (int j = height - 9; j >= num2; j--)
				{
					versionBits = this.copyBit(j, i, versionBits);
				}
			}
			this.parsedVersion = Version.decodeVersionInformation(versionBits);
			if (this.parsedVersion != null && this.parsedVersion.DimensionForVersion == height)
			{
				return this.parsedVersion;
			}
			versionBits = 0;
			for (int k = 5; k >= 0; k--)
			{
				for (int l = height - 9; l >= num2; l--)
				{
					versionBits = this.copyBit(k, l, versionBits);
				}
			}
			this.parsedVersion = Version.decodeVersionInformation(versionBits);
			if (this.parsedVersion != null && this.parsedVersion.DimensionForVersion == height)
			{
				return this.parsedVersion;
			}
			return null;
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x00020CE8 File Offset: 0x0001EEE8
		internal void remask()
		{
			if (this.parsedFormatInfo == null)
			{
				return;
			}
			DataMask arg_2C_0 = DataMask.forReference((int)this.parsedFormatInfo.DataMask);
			int height = this.bitMatrix.Height;
			arg_2C_0.unmaskBitMatrix(this.bitMatrix, height);
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x00020D26 File Offset: 0x0001EF26
		internal void setMirror(bool mirror)
		{
			this.parsedVersion = null;
			this.parsedFormatInfo = null;
			this.mirrored = mirror;
		}

		// Token: 0x040002D3 RID: 723
		private readonly BitMatrix bitMatrix;

		// Token: 0x040002D6 RID: 726
		private bool mirrored;

		// Token: 0x040002D5 RID: 725
		private FormatInformation parsedFormatInfo;

		// Token: 0x040002D4 RID: 724
		private Version parsedVersion;
	}
}
