using System;
using System.Text;

namespace ZXing.QrCode.Internal
{
	// Token: 0x0200007C RID: 124
	public sealed class ByteMatrix
	{
		// Token: 0x06000457 RID: 1111 RVA: 0x000266CC File Offset: 0x000248CC
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

		// Token: 0x0600045F RID: 1119 RVA: 0x00026768 File Offset: 0x00024968
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

		// Token: 0x0600045D RID: 1117 RVA: 0x00026745 File Offset: 0x00024945
		public void set(int x, int y, byte value)
		{
			this.bytes[y][x] = value;
		}

		// Token: 0x0600045E RID: 1118 RVA: 0x00026752 File Offset: 0x00024952
		public void set(int x, int y, bool value)
		{
            this.bytes[y][x] = (byte)(value ? (byte)1 : (byte)0);
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x000267A4 File Offset: 0x000249A4
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

		// Token: 0x1700005F RID: 95
		public byte[][] Array
		{
			// Token: 0x0600045C RID: 1116 RVA: 0x0002673D File Offset: 0x0002493D
			get
			{
				return this.bytes;
			}
		}

		// Token: 0x1700005C RID: 92
		public int Height
		{
			// Token: 0x06000458 RID: 1112 RVA: 0x00026713 File Offset: 0x00024913
			get
			{
				return this.height;
			}
		}

		// Token: 0x1700005E RID: 94
		public int this[int x, int y]
		{
			// Token: 0x0600045A RID: 1114 RVA: 0x00026723 File Offset: 0x00024923
			get
			{
				return (int)this.bytes[y][x];
			}
			// Token: 0x0600045B RID: 1115 RVA: 0x0002672F File Offset: 0x0002492F
			set
			{
				this.bytes[y][x] = (byte)value;
			}
		}

		// Token: 0x1700005D RID: 93
		public int Width
		{
			// Token: 0x06000459 RID: 1113 RVA: 0x0002671B File Offset: 0x0002491B
			get
			{
				return this.width;
			}
		}

		// Token: 0x0400031D RID: 797
		private readonly byte[][] bytes;

		// Token: 0x0400031F RID: 799
		private readonly int height;

		// Token: 0x0400031E RID: 798
		private readonly int width;
	}
}
