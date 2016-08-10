using System;

namespace ZXing.Common.ReedSolomon
{
	// Token: 0x0200008C RID: 140
	public sealed class GenericGF
	{
		// Token: 0x06000502 RID: 1282 RVA: 0x00029B61 File Offset: 0x00027D61
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

		// Token: 0x06000508 RID: 1288 RVA: 0x00029CAF File Offset: 0x00027EAF
		internal static int addOrSubtract(int a, int b)
		{
			return a ^ b;
		}

		// Token: 0x06000507 RID: 1287 RVA: 0x00029C74 File Offset: 0x00027E74
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

		// Token: 0x06000504 RID: 1284 RVA: 0x00029C46 File Offset: 0x00027E46
		private void checkInit()
		{
			if (!this.initialized)
			{
				this.initialize();
			}
		}

		// Token: 0x06000509 RID: 1289 RVA: 0x00029CB4 File Offset: 0x00027EB4
		internal int exp(int a)
		{
			this.checkInit();
			return this.expTable[a];
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x00029B88 File Offset: 0x00027D88
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

		// Token: 0x0600050B RID: 1291 RVA: 0x00029CDD File Offset: 0x00027EDD
		internal int inverse(int a)
		{
			this.checkInit();
			if (a == 0)
			{
				throw new ArithmeticException();
			}
			return this.expTable[this.size - this.logTable[a] - 1];
		}

		// Token: 0x0600050A RID: 1290 RVA: 0x00029CC4 File Offset: 0x00027EC4
		internal int log(int a)
		{
			this.checkInit();
			if (a == 0)
			{
				throw new ArgumentException();
			}
			return this.logTable[a];
		}

		// Token: 0x0600050C RID: 1292 RVA: 0x00029D06 File Offset: 0x00027F06
		internal int multiply(int a, int b)
		{
			this.checkInit();
			if (a == 0 || b == 0)
			{
				return 0;
			}
			return this.expTable[(this.logTable[a] + this.logTable[b]) % (this.size - 1)];
		}

		// Token: 0x0600050F RID: 1295 RVA: 0x00029D48 File Offset: 0x00027F48
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

		// Token: 0x17000080 RID: 128
		public int GeneratorBase
		{
			// Token: 0x0600050E RID: 1294 RVA: 0x00029D3F File Offset: 0x00027F3F
			get
			{
				return this.generatorBase;
			}
		}

		// Token: 0x1700007E RID: 126
		internal GenericGFPoly One
		{
			// Token: 0x06000506 RID: 1286 RVA: 0x00029C64 File Offset: 0x00027E64
			get
			{
				this.checkInit();
				return this.one;
			}
		}

		// Token: 0x1700007F RID: 127
		public int Size
		{
			// Token: 0x0600050D RID: 1293 RVA: 0x00029D37 File Offset: 0x00027F37
			get
			{
				return this.size;
			}
		}

		// Token: 0x1700007D RID: 125
		internal GenericGFPoly Zero
		{
			// Token: 0x06000505 RID: 1285 RVA: 0x00029C56 File Offset: 0x00027E56
			get
			{
				this.checkInit();
				return this.zero;
			}
		}

		// Token: 0x04000366 RID: 870
		public static GenericGF AZTEC_DATA_10 = new GenericGF(1033, 1024, 1);

		// Token: 0x04000365 RID: 869
		public static GenericGF AZTEC_DATA_12 = new GenericGF(4201, 4096, 1);

		// Token: 0x04000367 RID: 871
		public static GenericGF AZTEC_DATA_6 = new GenericGF(67, 64, 1);

		// Token: 0x0400036B RID: 875
		public static GenericGF AZTEC_DATA_8 = GenericGF.DATA_MATRIX_FIELD_256;

		// Token: 0x04000368 RID: 872
		public static GenericGF AZTEC_PARAM = new GenericGF(19, 16, 1);

		// Token: 0x0400036A RID: 874
		public static GenericGF DATA_MATRIX_FIELD_256 = new GenericGF(301, 256, 1);

		// Token: 0x0400036E RID: 878
		private int[] expTable;

		// Token: 0x04000374 RID: 884
		private readonly int generatorBase;

		// Token: 0x0400036D RID: 877
		private const int INITIALIZATION_THRESHOLD = 0;

		// Token: 0x04000375 RID: 885
		private bool initialized;

		// Token: 0x0400036F RID: 879
		private int[] logTable;

		// Token: 0x0400036C RID: 876
		public static GenericGF MAXICODE_FIELD_64 = GenericGF.AZTEC_DATA_6;

		// Token: 0x04000371 RID: 881
		private GenericGFPoly one;

		// Token: 0x04000373 RID: 883
		private readonly int primitive;

		// Token: 0x04000369 RID: 873
		public static GenericGF QR_CODE_FIELD_256 = new GenericGF(285, 256, 0);

		// Token: 0x04000372 RID: 882
		private readonly int size;

		// Token: 0x04000370 RID: 880
		private GenericGFPoly zero;
	}
}
