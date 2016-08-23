using System;
using System.Text;

namespace ZXing.QrCode.Internal
{
	// Token: 0x0200007E RID: 126
	public sealed class ByteMatrix
	{
		// Token: 0x06000460 RID: 1120 RVA: 0x00025914 File Offset: 0x00023B14
		public ByteMatrix(int width, int height)
		{
			this.bytes = new byte[height][];
			for (int i = 0; i < height; i++)
			{
				this.bytes[i] = new byte[width];
			}
			this.width = width;
			this.height = height;
		}

		// Token: 0x06000468 RID: 1128 RVA: 0x000259B0 File Offset: 0x00023BB0
		public void clear(byte value)
		{
			for (int i = 0; i < this.height; i++)
			{
				for (int j = 0; j < this.width; j++)
				{
					this.bytes[i][j] = value;
				}
			}
		}

		// Token: 0x06000466 RID: 1126 RVA: 0x0002598D File Offset: 0x00023B8D
		public void set(int x, int y, byte value)
		{
			this.bytes[y][x] = value;
		}

		// Token: 0x06000467 RID: 1127 RVA: 0x0002599A File Offset: 0x00023B9A
		public void set(int x, int y, bool value)
		{
			this.bytes[y][x] = (byte)(value ? 1 : 0);
		}

		// Token: 0x06000469 RID: 1129 RVA: 0x000259EC File Offset: 0x00023BEC
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(2 * this.width * this.height + 2);
			for (int i = 0; i < this.height; i++)
			{
				for (int j = 0; j < this.width; j++)
				{
					byte b = this.bytes[i][j];
					if (b != 0)
					{
						if (b != 1)
						{
							stringBuilder.Append("  ");
						}
						else
						{
							stringBuilder.Append(" 1");
						}
					}
					else
					{
						stringBuilder.Append(" 0");
					}
				}
				stringBuilder.Append('\n');
			}
			return stringBuilder.ToString();
		}

		// Token: 0x17000062 RID: 98
		public byte[][] Array
		{
			// Token: 0x06000465 RID: 1125 RVA: 0x00025985 File Offset: 0x00023B85
			get
			{
				return this.bytes;
			}
		}

		// Token: 0x1700005F RID: 95
		public int Height
		{
			// Token: 0x06000461 RID: 1121 RVA: 0x0002595B File Offset: 0x00023B5B
			get
			{
				return this.height;
			}
		}

		// Token: 0x17000061 RID: 97
		public int this[int x, int y]
		{
			// Token: 0x06000463 RID: 1123 RVA: 0x0002596B File Offset: 0x00023B6B
			get
			{
				return (int)this.bytes[y][x];
			}
			// Token: 0x06000464 RID: 1124 RVA: 0x00025977 File Offset: 0x00023B77
			set
			{
				this.bytes[y][x] = (byte)value;
			}
		}

		// Token: 0x17000060 RID: 96
		public int Width
		{
			// Token: 0x06000462 RID: 1122 RVA: 0x00025963 File Offset: 0x00023B63
			get
			{
				return this.width;
			}
		}

		// Token: 0x0400031A RID: 794
		private readonly byte[][] bytes;

		// Token: 0x0400031C RID: 796
		private readonly int height;

		// Token: 0x0400031B RID: 795
		private readonly int width;
	}
}
