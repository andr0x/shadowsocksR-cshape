using System;

namespace ZXing.Common
{
	// Token: 0x02000081 RID: 129
	public sealed class BitArray
	{
		// Token: 0x060004A1 RID: 1185 RVA: 0x00028115 File Offset: 0x00026315
		public BitArray()
		{
			this.size = 0;
			this.bits = new int[1];
		}

		// Token: 0x060004A2 RID: 1186 RVA: 0x00028130 File Offset: 0x00026330
		public BitArray(int size)
		{
			if (size < 1)
			{
				throw new ArgumentException("size must be at least 1");
			}
			this.size = size;
			this.bits = BitArray.makeArray(size);
		}

		// Token: 0x060004A3 RID: 1187 RVA: 0x0002815A File Offset: 0x0002635A
		private BitArray(int[] bits, int size)
		{
			this.bits = bits;
			this.size = size;
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x00028210 File Offset: 0x00026410
		public void appendBit(bool bit)
		{
			this.ensureCapacity(this.size + 1);
			if (bit)
			{
				this.bits[this.size >> 5] |= 1 << this.size;
			}
			this.size++;
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x000282BC File Offset: 0x000264BC
		public void appendBitArray(BitArray other)
		{
			int num = other.size;
			this.ensureCapacity(this.size + num);
			for (int i = 0; i < num; i++)
			{
				this.appendBit(other[i]);
			}
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x0002826C File Offset: 0x0002646C
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

		// Token: 0x060004A7 RID: 1191 RVA: 0x000281E4 File Offset: 0x000263E4
		public void clear()
		{
			int num = this.bits.Length;
			for (int i = 0; i < num; i++)
			{
				this.bits[i] = 0;
			}
		}

		// Token: 0x060004B1 RID: 1201 RVA: 0x00028433 File Offset: 0x00026633
		public object Clone()
		{
			return new BitArray((int[])this.bits.Clone(), this.size);
		}

		// Token: 0x060004A4 RID: 1188 RVA: 0x00028170 File Offset: 0x00026370
		private void ensureCapacity(int size)
		{
			if (size > this.bits.Length << 5)
			{
				int[] destinationArray = BitArray.makeArray(size);
				System.Array.Copy(this.bits, 0, destinationArray, 0, this.bits.Length);
				this.bits = destinationArray;
			}
		}

		// Token: 0x060004AF RID: 1199 RVA: 0x000283A8 File Offset: 0x000265A8
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

		// Token: 0x060004B0 RID: 1200 RVA: 0x000283F8 File Offset: 0x000265F8
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

		// Token: 0x060004AE RID: 1198 RVA: 0x00028398 File Offset: 0x00026598
		private static int[] makeArray(int size)
		{
			return new int[size + 31 >> 5];
		}

		// Token: 0x060004A5 RID: 1189 RVA: 0x000281B0 File Offset: 0x000263B0
		private static int numberOfTrailingZeros(int num)
		{
			int num2 = (-num & num) % 37;
			if (num2 < 0)
			{
				num2 *= -1;
			}
			return BitArray._lookup[num2];
		}

		// Token: 0x060004A6 RID: 1190 RVA: 0x000281D4 File Offset: 0x000263D4
		public void setBulk(int i, int newBits)
		{
			this.bits[i >> 5] = newBits;
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x00028350 File Offset: 0x00026550
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

		// Token: 0x060004AC RID: 1196 RVA: 0x000282F8 File Offset: 0x000264F8
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

		// Token: 0x17000068 RID: 104
		public int[] Array
		{
			// Token: 0x060004A9 RID: 1193 RVA: 0x00028261 File Offset: 0x00026461
			get
			{
				return this.bits;
			}
		}

		// Token: 0x17000067 RID: 103
		public bool this[int i]
		{
			// Token: 0x0600049F RID: 1183 RVA: 0x000280DC File Offset: 0x000262DC
			get
			{
				return (this.bits[i >> 5] & 1 << i) != 0;
			}
			// Token: 0x060004A0 RID: 1184 RVA: 0x000280F5 File Offset: 0x000262F5
			set
			{
				if (value)
				{
					this.bits[i >> 5] |= 1 << i;
				}
			}
		}

		// Token: 0x17000065 RID: 101
		public int Size
		{
			// Token: 0x0600049D RID: 1181 RVA: 0x000280C8 File Offset: 0x000262C8
			get
			{
				return this.size;
			}
		}

		// Token: 0x17000066 RID: 102
		public int SizeInBytes
		{
			// Token: 0x0600049E RID: 1182 RVA: 0x000280D0 File Offset: 0x000262D0
			get
			{
				return this.size + 7 >> 3;
			}
		}

		// Token: 0x04000333 RID: 819
		private int[] bits;

		// Token: 0x04000334 RID: 820
		private int size;

		// Token: 0x04000335 RID: 821
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
