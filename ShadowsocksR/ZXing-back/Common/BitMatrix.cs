using System;

namespace ZXing.Common
{
	// Token: 0x02000082 RID: 130
	public sealed class BitMatrix
	{
		// Token: 0x060004B5 RID: 1205 RVA: 0x00028479 File Offset: 0x00026679
		public BitMatrix(int dimension) : this(dimension, dimension)
		{
		}

		// Token: 0x060004B6 RID: 1206 RVA: 0x00028484 File Offset: 0x00026684
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

		// Token: 0x060004B7 RID: 1207 RVA: 0x000284D7 File Offset: 0x000266D7
		private BitMatrix(int width, int height, int rowSize, int[] bits)
		{
			this.width = width;
			this.height = height;
			this.rowSize = rowSize;
			this.bits = bits;
		}

		// Token: 0x060004BA RID: 1210 RVA: 0x00028564 File Offset: 0x00026764
		public void flip(int x, int y)
		{
			int num = y * this.rowSize + (x >> 5);
			this.bits[num] ^= 1 << x;
		}

		// Token: 0x060004BF RID: 1215 RVA: 0x0002873C File Offset: 0x0002693C
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

		// Token: 0x060004BC RID: 1212 RVA: 0x00028638 File Offset: 0x00026838
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

		// Token: 0x060004BE RID: 1214 RVA: 0x000286BC File Offset: 0x000268BC
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

		// Token: 0x060004BB RID: 1211 RVA: 0x00028598 File Offset: 0x00026798
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

		// Token: 0x060004BD RID: 1213 RVA: 0x00028699 File Offset: 0x00026899
		public void setRow(int y, BitArray row)
		{
			Array.Copy(row.Array, 0, this.bits, y * this.rowSize, this.rowSize);
		}

		// Token: 0x1700006A RID: 106
		public int Height
		{
			// Token: 0x060004B4 RID: 1204 RVA: 0x00028471 File Offset: 0x00026671
			get
			{
				return this.height;
			}
		}

		// Token: 0x1700006B RID: 107
		public bool this[int x, int y]
		{
			// Token: 0x060004B8 RID: 1208 RVA: 0x000284FC File Offset: 0x000266FC
			get
			{
				int num = y * this.rowSize + (x >> 5);
				return ((uint)this.bits[num] >> x & 1u) > 0u;
			}
			// Token: 0x060004B9 RID: 1209 RVA: 0x0002852C File Offset: 0x0002672C
			set
			{
				if (value)
				{
					int num = y * this.rowSize + (x >> 5);
					this.bits[num] |= 1 << x;
				}
			}
		}

		// Token: 0x17000069 RID: 105
		public int Width
		{
			// Token: 0x060004B3 RID: 1203 RVA: 0x00028469 File Offset: 0x00026669
			get
			{
				return this.width;
			}
		}

		// Token: 0x04000339 RID: 825
		private readonly int[] bits;

		// Token: 0x04000337 RID: 823
		private readonly int height;

		// Token: 0x04000338 RID: 824
		private readonly int rowSize;

		// Token: 0x04000336 RID: 822
		private readonly int width;
	}
}
