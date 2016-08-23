using System;

namespace ZXing.Common.ReedSolomon
{
	// Token: 0x0200008E RID: 142
	public sealed class GenericGF
	{
		// Token: 0x0600050B RID: 1291 RVA: 0x00028DA9 File Offset: 0x00026FA9
		public GenericGF(int primitive, int size, int genBase)
		{
			this.primitive = primitive;
			this.size = size;
			this.generatorBase = genBase;
			if (size <= 0)
			{
				this.initialize();
			}
		}

		// Token: 0x06000511 RID: 1297 RVA: 0x00028EF7 File Offset: 0x000270F7
		internal static int addOrSubtract(int a, int b)
		{
			return a ^ b;
		}

		// Token: 0x06000510 RID: 1296 RVA: 0x00028EBC File Offset: 0x000270BC
		internal GenericGFPoly buildMonomial(int degree, int coefficient)
		{
			this.checkInit();
			if (degree < 0)
			{
				throw new ArgumentException();
			}
			if (coefficient == 0)
			{
				return this.zero;
			}
			int[] array = new int[degree + 1];
			array[0] = coefficient;
			return new GenericGFPoly(this, array);
		}

		// Token: 0x0600050D RID: 1293 RVA: 0x00028E8E File Offset: 0x0002708E
		private void checkInit()
		{
			if (!this.initialized)
			{
				this.initialize();
			}
		}

		// Token: 0x06000512 RID: 1298 RVA: 0x00028EFC File Offset: 0x000270FC
		internal int exp(int a)
		{
			this.checkInit();
			return this.expTable[a];
		}

		// Token: 0x0600050C RID: 1292 RVA: 0x00028DD0 File Offset: 0x00026FD0
		private void initialize()
		{
			this.expTable = new int[this.size];
			this.logTable = new int[this.size];
			int num = 1;
			for (int i = 0; i < this.size; i++)
			{
				this.expTable[i] = num;
				num <<= 1;
				if (num >= this.size)
				{
					num ^= this.primitive;
					num &= this.size - 1;
				}
			}
			for (int j = 0; j < this.size - 1; j++)
			{
				this.logTable[this.expTable[j]] = j;
			}
			this.zero = new GenericGFPoly(this, new int[1]);
			this.one = new GenericGFPoly(this, new int[]
			{
				1
			});
			this.initialized = true;
		}

		// Token: 0x06000514 RID: 1300 RVA: 0x00028F25 File Offset: 0x00027125
		internal int inverse(int a)
		{
			this.checkInit();
			if (a == 0)
			{
				throw new ArithmeticException();
			}
			return this.expTable[this.size - this.logTable[a] - 1];
		}

		// Token: 0x06000513 RID: 1299 RVA: 0x00028F0C File Offset: 0x0002710C
		internal int log(int a)
		{
			this.checkInit();
			if (a == 0)
			{
				throw new ArgumentException();
			}
			return this.logTable[a];
		}

		// Token: 0x06000515 RID: 1301 RVA: 0x00028F4E File Offset: 0x0002714E
		internal int multiply(int a, int b)
		{
			this.checkInit();
			if (a == 0 || b == 0)
			{
				return 0;
			}
			return this.expTable[(this.logTable[a] + this.logTable[b]) % (this.size - 1)];
		}

		// Token: 0x06000518 RID: 1304 RVA: 0x00028F90 File Offset: 0x00027190
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"GF(0x",
				this.primitive.ToString("X"),
				",",
				this.size,
				")"
			});
		}

		// Token: 0x17000083 RID: 131
		public int GeneratorBase
		{
			// Token: 0x06000517 RID: 1303 RVA: 0x00028F87 File Offset: 0x00027187
			get
			{
				return this.generatorBase;
			}
		}

		// Token: 0x17000081 RID: 129
		internal GenericGFPoly One
		{
			// Token: 0x0600050F RID: 1295 RVA: 0x00028EAC File Offset: 0x000270AC
			get
			{
				this.checkInit();
				return this.one;
			}
		}

		// Token: 0x17000082 RID: 130
		public int Size
		{
			// Token: 0x06000516 RID: 1302 RVA: 0x00028F7F File Offset: 0x0002717F
			get
			{
				return this.size;
			}
		}

		// Token: 0x17000080 RID: 128
		internal GenericGFPoly Zero
		{
			// Token: 0x0600050E RID: 1294 RVA: 0x00028E9E File Offset: 0x0002709E
			get
			{
				this.checkInit();
				return this.zero;
			}
		}

		// Token: 0x04000363 RID: 867
		public static GenericGF AZTEC_DATA_10 = new GenericGF(1033, 1024, 1);

		// Token: 0x04000362 RID: 866
		public static GenericGF AZTEC_DATA_12 = new GenericGF(4201, 4096, 1);

		// Token: 0x04000364 RID: 868
		public static GenericGF AZTEC_DATA_6 = new GenericGF(67, 64, 1);

		// Token: 0x04000368 RID: 872
		public static GenericGF AZTEC_DATA_8 = GenericGF.DATA_MATRIX_FIELD_256;

		// Token: 0x04000365 RID: 869
		public static GenericGF AZTEC_PARAM = new GenericGF(19, 16, 1);

		// Token: 0x04000367 RID: 871
		public static GenericGF DATA_MATRIX_FIELD_256 = new GenericGF(301, 256, 1);

		// Token: 0x0400036B RID: 875
		private int[] expTable;

		// Token: 0x04000371 RID: 881
		private readonly int generatorBase;

		// Token: 0x0400036A RID: 874
		private const int INITIALIZATION_THRESHOLD = 0;

		// Token: 0x04000372 RID: 882
		private bool initialized;

		// Token: 0x0400036C RID: 876
		private int[] logTable;

		// Token: 0x04000369 RID: 873
		public static GenericGF MAXICODE_FIELD_64 = GenericGF.AZTEC_DATA_6;

		// Token: 0x0400036E RID: 878
		private GenericGFPoly one;

		// Token: 0x04000370 RID: 880
		private readonly int primitive;

		// Token: 0x04000366 RID: 870
		public static GenericGF QR_CODE_FIELD_256 = new GenericGF(285, 256, 0);

		// Token: 0x0400036F RID: 879
		private readonly int size;

		// Token: 0x0400036D RID: 877
		private GenericGFPoly zero;
	}
}
