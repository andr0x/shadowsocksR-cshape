using System;

namespace ZXing.QrCode.Internal
{
	// Token: 0x02000072 RID: 114
	public sealed class ErrorCorrectionLevel
	{
		// Token: 0x06000406 RID: 1030 RVA: 0x0002188D File Offset: 0x0001FA8D
		private ErrorCorrectionLevel(int ordinal, int bits, string name)
		{
			this.ordinal_Renamed_Field = ordinal;
			this.bits = bits;
			this.name = name;
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x000218C2 File Offset: 0x0001FAC2
		public static ErrorCorrectionLevel forBits(int bits)
		{
			if (bits < 0 || bits >= ErrorCorrectionLevel.FOR_BITS.Length)
			{
				throw new ArgumentException();
			}
            switch (bits)
            {
                case 0: return ErrorCorrectionLevel.M; break;
                case 1: return ErrorCorrectionLevel.L; break;
                case 2: return ErrorCorrectionLevel.H; break;
                case 3: return ErrorCorrectionLevel.Q; break;
                default: return ErrorCorrectionLevel.M; break;
            }
			return ErrorCorrectionLevel.FOR_BITS[bits];
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x000218BA File Offset: 0x0001FABA
		public int ordinal()
		{
			return this.ordinal_Renamed_Field;
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x000218B2 File Offset: 0x0001FAB2
		public override string ToString()
		{
			return this.name;
		}

		// Token: 0x17000048 RID: 72
		public int Bits
		{
			// Token: 0x06000407 RID: 1031 RVA: 0x000218AA File Offset: 0x0001FAAA
			get
			{
				return this.bits;
			}
		}

		// Token: 0x17000049 RID: 73
		public string Name
		{
			// Token: 0x06000408 RID: 1032 RVA: 0x000218B2 File Offset: 0x0001FAB2
			get
			{
				return this.name;
			}
		}

		// Token: 0x040002E2 RID: 738
		private readonly int bits;

		// Token: 0x040002E1 RID: 737
		private static readonly ErrorCorrectionLevel[] FOR_BITS = new ErrorCorrectionLevel[]
		{
			ErrorCorrectionLevel.M,
			ErrorCorrectionLevel.L,
			ErrorCorrectionLevel.H,
			ErrorCorrectionLevel.Q
		};

		// Token: 0x040002E0 RID: 736
		public static readonly ErrorCorrectionLevel H = new ErrorCorrectionLevel(3, 2, "H");

		// Token: 0x040002DD RID: 733
		public static readonly ErrorCorrectionLevel L = new ErrorCorrectionLevel(0, 1, "L");

		// Token: 0x040002DE RID: 734
		public static readonly ErrorCorrectionLevel M = new ErrorCorrectionLevel(1, 0, "M");

		// Token: 0x040002E4 RID: 740
		private readonly string name;

		// Token: 0x040002E3 RID: 739
		private readonly int ordinal_Renamed_Field;

		// Token: 0x040002DF RID: 735
		public static readonly ErrorCorrectionLevel Q = new ErrorCorrectionLevel(2, 3, "Q");
	}
}
