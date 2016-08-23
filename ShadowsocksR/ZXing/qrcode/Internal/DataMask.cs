using System;
using ZXing.Common;

namespace ZXing.QrCode.Internal
{
	// Token: 0x0200006F RID: 111
	internal abstract class DataMask
	{
		// Token: 0x060003F3 RID: 1011 RVA: 0x00007881 File Offset: 0x00005A81
		private DataMask()
		{
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x00020FC3 File Offset: 0x0001F1C3
		internal static DataMask forReference(int reference)
		{
			if (reference < 0 || reference > 7)
			{
				throw new ArgumentException();
			}
			return DataMask.DATA_MASKS[reference];
		}

		// Token: 0x060003F5 RID: 1013
		internal abstract bool isMasked(int i, int j);

		// Token: 0x060003F4 RID: 1012 RVA: 0x00020F8C File Offset: 0x0001F18C
		internal void unmaskBitMatrix(BitMatrix bits, int dimension)
		{
			for (int i = 0; i < dimension; i++)
			{
				for (int j = 0; j < dimension; j++)
				{
					if (this.isMasked(i, j))
					{
						bits.flip(j, i);
					}
				}
			}
		}

		// Token: 0x040002D9 RID: 729
		private static readonly DataMask[] DATA_MASKS = new DataMask[]
		{
			new DataMask.DataMask000(),
			new DataMask.DataMask001(),
			new DataMask.DataMask010(),
			new DataMask.DataMask011(),
			new DataMask.DataMask100(),
			new DataMask.DataMask101(),
			new DataMask.DataMask110(),
			new DataMask.DataMask111()
		};

		// Token: 0x020000BC RID: 188
		private sealed class DataMask000 : DataMask
		{
			// Token: 0x0600059F RID: 1439 RVA: 0x0002BC64 File Offset: 0x00029E64
			internal override bool isMasked(int i, int j)
			{
				return (i + j & 1) == 0;
			}
		}

		// Token: 0x020000BD RID: 189
		private sealed class DataMask001 : DataMask
		{
			// Token: 0x060005A1 RID: 1441 RVA: 0x0002BC76 File Offset: 0x00029E76
			internal override bool isMasked(int i, int j)
			{
				return (i & 1) == 0;
			}
		}

		// Token: 0x020000BE RID: 190
		private sealed class DataMask010 : DataMask
		{
			// Token: 0x060005A3 RID: 1443 RVA: 0x0002BC7E File Offset: 0x00029E7E
			internal override bool isMasked(int i, int j)
			{
				return j % 3 == 0;
			}
		}

		// Token: 0x020000BF RID: 191
		private sealed class DataMask011 : DataMask
		{
			// Token: 0x060005A5 RID: 1445 RVA: 0x0002BC86 File Offset: 0x00029E86
			internal override bool isMasked(int i, int j)
			{
				return (i + j) % 3 == 0;
			}
		}

		// Token: 0x020000C0 RID: 192
		private sealed class DataMask100 : DataMask
		{
			// Token: 0x060005A7 RID: 1447 RVA: 0x0002BC90 File Offset: 0x00029E90
			internal override bool isMasked(int i, int j)
			{
				return (((uint)i >> 1) + (uint)(j / 3) & 1u) == 0u;
			}
		}

		// Token: 0x020000C1 RID: 193
		private sealed class DataMask101 : DataMask
		{
			// Token: 0x060005A9 RID: 1449 RVA: 0x0002BCA0 File Offset: 0x00029EA0
			internal override bool isMasked(int i, int j)
			{
				int num = i * j;
				return (num & 1) + num % 3 == 0;
			}
		}

		// Token: 0x020000C2 RID: 194
		private sealed class DataMask110 : DataMask
		{
			// Token: 0x060005AB RID: 1451 RVA: 0x0002BCBC File Offset: 0x00029EBC
			internal override bool isMasked(int i, int j)
			{
				int num = i * j;
				return ((num & 1) + num % 3 & 1) == 0;
			}
		}

		// Token: 0x020000C3 RID: 195
		private sealed class DataMask111 : DataMask
		{
			// Token: 0x060005AD RID: 1453 RVA: 0x0002BCD9 File Offset: 0x00029ED9
			internal override bool isMasked(int i, int j)
			{
				return ((i + j & 1) + i * j % 3 & 1) == 0;
			}
		}
	}
}
