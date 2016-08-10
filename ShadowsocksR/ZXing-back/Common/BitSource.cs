using System;

namespace ZXing.Common
{
	// Token: 0x02000083 RID: 131
	public sealed class BitSource
	{
		// Token: 0x060004C0 RID: 1216 RVA: 0x000287B3 File Offset: 0x000269B3
		public BitSource(byte[] bytes)
		{
			this.bytes = bytes;
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x000288FE File Offset: 0x00026AFE
		public int available()
		{
			return 8 * (this.bytes.Length - this.byteOffset) - this.bitOffset;
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x000287D4 File Offset: 0x000269D4
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

		// Token: 0x1700006C RID: 108
		public int BitOffset
		{
			// Token: 0x060004C1 RID: 1217 RVA: 0x000287C2 File Offset: 0x000269C2
			get
			{
				return this.bitOffset;
			}
		}

		// Token: 0x1700006D RID: 109
		public int ByteOffset
		{
			// Token: 0x060004C2 RID: 1218 RVA: 0x000287CA File Offset: 0x000269CA
			get
			{
				return this.byteOffset;
			}
		}

		// Token: 0x0400033C RID: 828
		private int bitOffset;

		// Token: 0x0400033B RID: 827
		private int byteOffset;

		// Token: 0x0400033A RID: 826
		private readonly byte[] bytes;
	}
}
