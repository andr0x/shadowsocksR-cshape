using System;

namespace ZXing.QrCode.Internal
{
	// Token: 0x02000070 RID: 112
	public sealed class ErrorCorrectionLevel
	{
		// Token: 0x060003FD RID: 1021 RVA: 0x00022645 File Offset: 0x00020845
		private ErrorCorrectionLevel(int ordinal, int bits, string name)
		{
			this.ordinal_Renamed_Field = ordinal;
			this.bits = bits;
			this.name = name;
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x0002267A File Offset: 0x0002087A
		public static ErrorCorrectionLevel forBits(int bits)
		{
			if (bits < 0 || bits >= ErrorCorrectionLevel.FOR_BITS.Length)
			{
				throw new ArgumentException();
			}
            switch(bits)
            {
                case 0: return ErrorCorrectionLevel.M; break;
                case 1: return ErrorCorrectionLevel.L; break;
                case 2: return ErrorCorrectionLevel.H; break;
                case 3: return ErrorCorrectionLevel.Q; break;
                default: return ErrorCorrectionLevel.M; break;
            }
            return ErrorCorrectionLevel.FOR_BITS[bits];
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x00022672 File Offset: 0x00020872
		public int ordinal()
		{
			return this.ordinal_Renamed_Field;
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x0002266A File Offset: 0x0002086A
		public override string ToString()
		{
			return this.name;
		}

		// Token: 0x17000045 RID: 69
		public int Bits
		{
			// Token: 0x060003FE RID: 1022 RVA: 0x00022662 File Offset: 0x00020862
			get
			{
				return this.bits;
			}
		}

		// Token: 0x17000046 RID: 70
		public string Name
		{
			// Token: 0x060003FF RID: 1023 RVA: 0x0002266A File Offset: 0x0002086A
			get
			{
				return this.name;
			}
		}

		// Token: 0x040002E5 RID: 741
		private readonly int bits;

		// Token: 0x040002E4 RID: 740
		private static readonly ErrorCorrectionLevel[] FOR_BITS = new ErrorCorrectionLevel[]
		{
			ErrorCorrectionLevel.M,
			ErrorCorrectionLevel.L,
			ErrorCorrectionLevel.H,
			ErrorCorrectionLevel.Q
		};

		// Token: 0x040002E3 RID: 739
		public static readonly ErrorCorrectionLevel H = new ErrorCorrectionLevel(3, 2, "H");

		// Token: 0x040002E0 RID: 736
		public static readonly ErrorCorrectionLevel L = new ErrorCorrectionLevel(0, 1, "L");

		// Token: 0x040002E1 RID: 737
		public static readonly ErrorCorrectionLevel M = new ErrorCorrectionLevel(1, 0, "M");

		// Token: 0x040002E7 RID: 743
		private readonly string name;

		// Token: 0x040002E6 RID: 742
		private readonly int ordinal_Renamed_Field;

		// Token: 0x040002E2 RID: 738
		public static readonly ErrorCorrectionLevel Q = new ErrorCorrectionLevel(2, 3, "Q");
	}
}
