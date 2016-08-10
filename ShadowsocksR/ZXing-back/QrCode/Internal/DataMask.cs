using System;
using ZXing.Common;

namespace ZXing.QrCode.Internal
{
	// Token: 0x0200006D RID: 109
	internal abstract class DataMask
	{
		// Token: 0x060003EA RID: 1002 RVA: 0x00007729 File Offset: 0x00005929
		private DataMask()
		{
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x00021D7B File Offset: 0x0001FF7B
		internal static DataMask forReference(int reference)
		{
			if (reference < 0 || reference > 7)
			{
				throw new ArgumentException();
			}
			return DataMask.DATA_MASKS[reference];
		}

		// Token: 0x060003EC RID: 1004
		internal abstract bool isMasked(int i, int j);

		// Token: 0x060003EB RID: 1003 RVA: 0x00021D44 File Offset: 0x0001FF44
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

		// Token: 0x040002DC RID: 732
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

		// Token: 0x020000BB RID: 187
		private sealed class DataMask000 : DataMask
		{
			// Token: 0x0600059A RID: 1434 RVA: 0x0002CA1C File Offset: 0x0002AC1C
			internal override bool isMasked(int i, int j)
			{
				return (i + j & 1) == 0;
			}
		}

		// Token: 0x020000BC RID: 188
		private sealed class DataMask001 : DataMask
		{
			// Token: 0x0600059C RID: 1436 RVA: 0x0002CA2E File Offset: 0x0002AC2E
			internal override bool isMasked(int i, int j)
			{
				return (i & 1) == 0;
			}
		}

		// Token: 0x020000BD RID: 189
		private sealed class DataMask010 : DataMask
		{
			// Token: 0x0600059E RID: 1438 RVA: 0x0002CA36 File Offset: 0x0002AC36
			internal override bool isMasked(int i, int j)
			{
				return j % 3 == 0;
			}
		}

		// Token: 0x020000BE RID: 190
		private sealed class DataMask011 : DataMask
		{
			// Token: 0x060005A0 RID: 1440 RVA: 0x0002CA3E File Offset: 0x0002AC3E
			internal override bool isMasked(int i, int j)
			{
				return (i + j) % 3 == 0;
			}
		}

		// Token: 0x020000BF RID: 191
		private sealed class DataMask100 : DataMask
		{
			// Token: 0x060005A2 RID: 1442 RVA: 0x0002CA48 File Offset: 0x0002AC48
			internal override bool isMasked(int i, int j)
			{
				return (((uint)i >> 1) + (uint)(j / 3) & 1u) == 0u;
			}
		}

		// Token: 0x020000C0 RID: 192
		private sealed class DataMask101 : DataMask
		{
			// Token: 0x060005A4 RID: 1444 RVA: 0x0002CA58 File Offset: 0x0002AC58
			internal override bool isMasked(int i, int j)
			{
				int num = i * j;
				return (num & 1) + num % 3 == 0;
			}
		}

		// Token: 0x020000C1 RID: 193
		private sealed class DataMask110 : DataMask
		{
			// Token: 0x060005A6 RID: 1446 RVA: 0x0002CA74 File Offset: 0x0002AC74
			internal override bool isMasked(int i, int j)
			{
				int num = i * j;
				return ((num & 1) + num % 3 & 1) == 0;
			}
		}

		// Token: 0x020000C2 RID: 194
		private sealed class DataMask111 : DataMask
		{
			// Token: 0x060005A8 RID: 1448 RVA: 0x0002CA91 File Offset: 0x0002AC91
			internal override bool isMasked(int i, int j)
			{
				return ((i + j & 1) + i * j % 3 & 1) == 0;
			}
		}
	}
}
