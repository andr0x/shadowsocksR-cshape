using System;

namespace ZXing.Common
{
	// Token: 0x02000083 RID: 131
	public sealed class BitArray
	{
		// Token: 0x060004AA RID: 1194 RVA: 0x0002735D File Offset: 0x0002555D
		public BitArray()
		{
			this.size = 0;
			this.bits = new int[1];
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x00027378 File Offset: 0x00025578
		public BitArray(int size)
		{
			if (size < 1)
			{
				throw new ArgumentException("size must be at least 1");
			}
			this.size = size;
			this.bits = BitArray.makeArray(size);
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x000273A2 File Offset: 0x000255A2
		private BitArray(int[] bits, int size)
		{
			this.bits = bits;
			this.size = size;
		}

		// Token: 0x060004B1 RID: 1201 RVA: 0x00027458 File Offset: 0x00025658
		public void appendBit(bool bit)
		{
			this.ensureCapacity(this.size + 1);
			if (bit)
			{
				this.bits[this.size >> 5] |= 1 << this.size;
			}
			this.size++;
		}

		// Token: 0x060004B4 RID: 1204 RVA: 0x00027504 File Offset: 0x00025704
		public void appendBitArray(BitArray other)
		{
			int num = other.size;
			this.ensureCapacity(this.size + num);
			for (int i = 0; i < num; i++)
			{
				this.appendBit(other[i]);
			}
		}

		// Token: 0x060004B3 RID: 1203 RVA: 0x000274B4 File Offset: 0x000256B4
		public void appendBits(int value, int numBits)
		{
			if (numBits < 0 || numBits > 32)
			{
				throw new ArgumentException("Num bits must be between 0 and 32");
			}
			this.ensureCapacity(this.size + numBits);
			for (int i = numBits; i > 0; i--)
			{
				this.appendBit((value >> i - 1 & 1) == 1);
			}
		}

		// Token: 0x060004B0 RID: 1200 RVA: 0x0002742C File Offset: 0x0002562C
		public void clear()
		{
			int num = this.bits.Length;
			for (int i = 0; i < num; i++)
			{
				this.bits[i] = 0;
			}
		}

		// Token: 0x060004BA RID: 1210 RVA: 0x0002767B File Offset: 0x0002587B
		public object Clone()
		{
			return new BitArray((int[])this.bits.Clone(), this.size);
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x000273B8 File Offset: 0x000255B8
		private void ensureCapacity(int size)
		{
			if (size > this.bits.Length << 5)
			{
				int[] destinationArray = BitArray.makeArray(size);
				System.Array.Copy(this.bits, 0, destinationArray, 0, this.bits.Length);
				this.bits = destinationArray;
			}
		}

		// Token: 0x060004B8 RID: 1208 RVA: 0x000275F0 File Offset: 0x000257F0
		public override bool Equals(object o)
		{
			BitArray bitArray = o as BitArray;
			if (bitArray == null)
			{
				return false;
			}
			if (this.size != bitArray.size)
			{
				return false;
			}
			for (int i = 0; i < this.size; i++)
			{
				if (this.bits[i] != bitArray.bits[i])
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060004B9 RID: 1209 RVA: 0x00027640 File Offset: 0x00025840
		public override int GetHashCode()
		{
			int num = this.size;
			int[] array = this.bits;
			for (int i = 0; i < array.Length; i++)
			{
				int num2 = array[i];
				num = 31 * num + num2.GetHashCode();
			}
			return num;
		}

		// Token: 0x060004B7 RID: 1207 RVA: 0x000275E0 File Offset: 0x000257E0
		private static int[] makeArray(int size)
		{
			return new int[size + 31 >> 5];
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x000273F8 File Offset: 0x000255F8
		private static int numberOfTrailingZeros(int num)
		{
			int num2 = (-num & num) % 37;
			if (num2 < 0)
			{
				num2 *= -1;
			}
			return BitArray._lookup[num2];
		}

		// Token: 0x060004AF RID: 1199 RVA: 0x0002741C File Offset: 0x0002561C
		public void setBulk(int i, int newBits)
		{
			this.bits[i >> 5] = newBits;
		}

		// Token: 0x060004B6 RID: 1206 RVA: 0x00027598 File Offset: 0x00025798
		public void toBytes(int bitOffset, byte[] array, int offset, int numBytes)
		{
			for (int i = 0; i < numBytes; i++)
			{
				int num = 0;
				for (int j = 0; j < 8; j++)
				{
					if (this[bitOffset])
					{
						num |= 1 << 7 - j;
					}
					bitOffset++;
				}
				array[offset + i] = (byte)num;
			}
		}

		// Token: 0x060004B5 RID: 1205 RVA: 0x00027540 File Offset: 0x00025740
		public void xor(BitArray other)
		{
			if (this.bits.Length != other.bits.Length)
			{
				throw new ArgumentException("Sizes don't match");
			}
			for (int i = 0; i < this.bits.Length; i++)
			{
				this.bits[i] ^= other.bits[i];
			}
		}

		// Token: 0x1700006B RID: 107
		public int[] Array
		{
			// Token: 0x060004B2 RID: 1202 RVA: 0x000274A9 File Offset: 0x000256A9
			get
			{
				return this.bits;
			}
		}

		// Token: 0x1700006A RID: 106
		public bool this[int i]
		{
			// Token: 0x060004A8 RID: 1192 RVA: 0x00027324 File Offset: 0x00025524
			get
			{
				return (this.bits[i >> 5] & 1 << i) != 0;
			}
			// Token: 0x060004A9 RID: 1193 RVA: 0x0002733D File Offset: 0x0002553D
			set
			{
				if (value)
				{
					this.bits[i >> 5] |= 1 << i;
				}
			}
		}

		// Token: 0x17000068 RID: 104
		public int Size
		{
			// Token: 0x060004A6 RID: 1190 RVA: 0x00027310 File Offset: 0x00025510
			get
			{
				return this.size;
			}
		}

		// Token: 0x17000069 RID: 105
		public int SizeInBytes
		{
			// Token: 0x060004A7 RID: 1191 RVA: 0x00027318 File Offset: 0x00025518
			get
			{
				return this.size + 7 >> 3;
			}
		}

		// Token: 0x04000330 RID: 816
		private int[] bits;

		// Token: 0x04000331 RID: 817
		private int size;

		// Token: 0x04000332 RID: 818
		private static readonly int[] _lookup = new int[]
		{
			32,
			0,
			1,
			26,
			2,
			23,
			27,
			0,
			3,
			16,
			24,
			30,
			28,
			11,
			0,
			13,
			4,
			7,
			17,
			0,
			25,
			22,
			31,
			15,
			29,
			10,
			12,
			6,
			0,
			21,
			14,
			9,
			5,
			20,
			8,
			19,
			18
		};
	}
}
