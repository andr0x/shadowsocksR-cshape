using System;

namespace ZXing.Common
{
	// Token: 0x02000084 RID: 132
	public sealed class BitMatrix
	{
		// Token: 0x060004BE RID: 1214 RVA: 0x000276C1 File Offset: 0x000258C1
		public BitMatrix(int dimension) : this(dimension, dimension)
		{
		}

		// Token: 0x060004BF RID: 1215 RVA: 0x000276CC File Offset: 0x000258CC
		public BitMatrix(int width, int height)
		{
			if (width < 1 || height < 1)
			{
				throw new ArgumentException("Both dimensions must be greater than 0");
			}
			this.width = width;
			this.height = height;
			this.rowSize = width + 31 >> 5;
			this.bits = new int[this.rowSize * height];
		}

		// Token: 0x060004C0 RID: 1216 RVA: 0x0002771F File Offset: 0x0002591F
		private BitMatrix(int width, int height, int rowSize, int[] bits)
		{
			this.width = width;
			this.height = height;
			this.rowSize = rowSize;
			this.bits = bits;
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x000277AC File Offset: 0x000259AC
		public void flip(int x, int y)
		{
			int num = y * this.rowSize + (x >> 5);
			this.bits[num] ^= 1 << x;
		}

		// Token: 0x060004C8 RID: 1224 RVA: 0x00027984 File Offset: 0x00025B84
		public int[] getBottomRightOnBit()
		{
			int num = this.bits.Length - 1;
			while (num >= 0 && this.bits[num] == 0)
			{
				num--;
			}
			if (num < 0)
			{
				return null;
			}
			int num2 = num / this.rowSize;
			int num3 = num % this.rowSize << 5;
			int num4 = this.bits[num];
			int num5 = 31;
			while ((uint)num4 >> num5 == 0u)
			{
				num5--;
			}
			num3 += num5;
			return new int[]
			{
				num3,
				num2
			};
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x00027880 File Offset: 0x00025A80
		public BitArray getRow(int y, BitArray row)
		{
			if (row == null || row.Size < this.width)
			{
				row = new BitArray(this.width);
			}
			else
			{
				row.clear();
			}
			int num = y * this.rowSize;
			for (int i = 0; i < this.rowSize; i++)
			{
				row.setBulk(i << 5, this.bits[num + i]);
			}
			return row;
		}

		// Token: 0x060004C7 RID: 1223 RVA: 0x00027904 File Offset: 0x00025B04
		public int[] getTopLeftOnBit()
		{
			int num = 0;
			while (num < this.bits.Length && this.bits[num] == 0)
			{
				num++;
			}
			if (num == this.bits.Length)
			{
				return null;
			}
			int num2 = num / this.rowSize;
			int num3 = num % this.rowSize << 5;
			int num4 = this.bits[num];
			int num5 = 0;
			while (num4 << 31 - num5 == 0)
			{
				num5++;
			}
			num3 += num5;
			return new int[]
			{
				num3,
				num2
			};
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x000277E0 File Offset: 0x000259E0
		public void setRegion(int left, int top, int width, int height)
		{
			if (top < 0 || left < 0)
			{
				throw new ArgumentException("Left and top must be nonnegative");
			}
			if (height < 1 || width < 1)
			{
				throw new ArgumentException("Height and width must be at least 1");
			}
			int num = left + width;
			int num2 = top + height;
			if (num2 > this.height || num > this.width)
			{
				throw new ArgumentException("The region must fit inside the matrix");
			}
			for (int i = top; i < num2; i++)
			{
				int num3 = i * this.rowSize;
				for (int j = left; j < num; j++)
				{
					this.bits[num3 + (j >> 5)] |= 1 << j;
				}
			}
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x000278E1 File Offset: 0x00025AE1
		public void setRow(int y, BitArray row)
		{
			Array.Copy(row.Array, 0, this.bits, y * this.rowSize, this.rowSize);
		}

		// Token: 0x1700006D RID: 109
		public int Height
		{
			// Token: 0x060004BD RID: 1213 RVA: 0x000276B9 File Offset: 0x000258B9
			get
			{
				return this.height;
			}
		}

		// Token: 0x1700006E RID: 110
		public bool this[int x, int y]
		{
			// Token: 0x060004C1 RID: 1217 RVA: 0x00027744 File Offset: 0x00025944
			get
			{
				int num = y * this.rowSize + (x >> 5);
				return ((uint)this.bits[num] >> x & 1u) > 0u;
			}
			// Token: 0x060004C2 RID: 1218 RVA: 0x00027774 File Offset: 0x00025974
			set
			{
				if (value)
				{
					int num = y * this.rowSize + (x >> 5);
					this.bits[num] |= 1 << x;
				}
			}
		}

		// Token: 0x1700006C RID: 108
		public int Width
		{
			// Token: 0x060004BC RID: 1212 RVA: 0x000276B1 File Offset: 0x000258B1
			get
			{
				return this.width;
			}
		}

		// Token: 0x04000336 RID: 822
		private readonly int[] bits;

		// Token: 0x04000334 RID: 820
		private readonly int height;

		// Token: 0x04000335 RID: 821
		private readonly int rowSize;

		// Token: 0x04000333 RID: 819
		private readonly int width;
	}
}
