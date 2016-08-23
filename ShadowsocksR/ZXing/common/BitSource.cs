using System;

namespace ZXing.Common
{
	// Token: 0x02000085 RID: 133
	public sealed class BitSource
	{
		// Token: 0x060004C9 RID: 1225 RVA: 0x000279FB File Offset: 0x00025BFB
		public BitSource(byte[] bytes)
		{
			this.bytes = bytes;
		}

		// Token: 0x060004CD RID: 1229 RVA: 0x00027B46 File Offset: 0x00025D46
		public int available()
		{
			return 8 * (this.bytes.Length - this.byteOffset) - this.bitOffset;
		}

		// Token: 0x060004CC RID: 1228 RVA: 0x00027A1C File Offset: 0x00025C1C
		public int readBits(int numBits)
		{
			if (numBits < 1 || numBits > 32 || numBits > this.available())
			{
				throw new ArgumentException(numBits.ToString(), "numBits");
			}
			int num = 0;
			if (this.bitOffset > 0)
			{
				int num2 = 8 - this.bitOffset;
				int num3 = (numBits < num2) ? numBits : num2;
				int num4 = num2 - num3;
				int num5 = 255 >> 8 - num3 << num4;
				num = ((int)this.bytes[this.byteOffset] & num5) >> num4;
				numBits -= num3;
				this.bitOffset += num3;
				if (this.bitOffset == 8)
				{
					this.bitOffset = 0;
					this.byteOffset++;
				}
			}
			if (numBits > 0)
			{
				while (numBits >= 8)
				{
					num = (num << 8 | (int)(this.bytes[this.byteOffset] & 255));
					this.byteOffset++;
					numBits -= 8;
				}
				if (numBits > 0)
				{
					int num6 = 8 - numBits;
					int num7 = 255 >> num6 << num6;
					num = (num << numBits | ((int)this.bytes[this.byteOffset] & num7) >> num6);
					this.bitOffset += numBits;
				}
			}
			return num;
		}

		// Token: 0x1700006F RID: 111
		public int BitOffset
		{
			// Token: 0x060004CA RID: 1226 RVA: 0x00027A0A File Offset: 0x00025C0A
			get
			{
				return this.bitOffset;
			}
		}

		// Token: 0x17000070 RID: 112
		public int ByteOffset
		{
			// Token: 0x060004CB RID: 1227 RVA: 0x00027A12 File Offset: 0x00025C12
			get
			{
				return this.byteOffset;
			}
		}

		// Token: 0x04000339 RID: 825
		private int bitOffset;

		// Token: 0x04000338 RID: 824
		private int byteOffset;

		// Token: 0x04000337 RID: 823
		private readonly byte[] bytes;
	}
}
