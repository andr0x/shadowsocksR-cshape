using System;

namespace ZXing.QrCode.Internal
{
	// Token: 0x02000072 RID: 114
	public sealed class Mode
	{
		// Token: 0x0600040E RID: 1038 RVA: 0x00022BCE File Offset: 0x00020DCE
		private Mode(int[] characterCountBitsForVersions, int bits, string name)
		{
			this.characterCountBitsForVersions = characterCountBitsForVersions;
			this.bits = bits;
			this.name = name;
		}

		// Token: 0x0600040F RID: 1039 RVA: 0x00022BEC File Offset: 0x00020DEC
		public static Mode forBits(int bits)
		{
			switch (bits)
			{
			case 0:
				return Mode.TERMINATOR;
			case 1:
				return Mode.NUMERIC;
			case 2:
				return Mode.ALPHANUMERIC;
			case 3:
				return Mode.STRUCTURED_APPEND;
			case 4:
				return Mode.BYTE;
			case 5:
				return Mode.FNC1_FIRST_POSITION;
			case 7:
				return Mode.ECI;
			case 8:
				return Mode.KANJI;
			case 9:
				return Mode.FNC1_SECOND_POSITION;
			case 13:
				return Mode.HANZI;
			}
			throw new ArgumentException();
		}

		// Token: 0x06000410 RID: 1040 RVA: 0x00022C7C File Offset: 0x00020E7C
		public int getCharacterCountBits(Version version)
		{
			if (this.characterCountBitsForVersions == null)
			{
				throw new ArgumentException("Character count doesn't apply to this mode");
			}
			int versionNumber = version.VersionNumber;
			int num;
			if (versionNumber <= 9)
			{
				num = 0;
			}
			else if (versionNumber <= 26)
			{
				num = 1;
			}
			else
			{
				num = 2;
			}
			return this.characterCountBitsForVersions[num];
		}

		// Token: 0x06000412 RID: 1042 RVA: 0x00022BC6 File Offset: 0x00020DC6
		public override string ToString()
		{
			return this.name;
		}

		// Token: 0x1700004A RID: 74
		public int Bits
		{
			// Token: 0x06000411 RID: 1041 RVA: 0x00022CBF File Offset: 0x00020EBF
			get
			{
				return this.bits;
			}
		}

		// Token: 0x17000049 RID: 73
		public string Name
		{
			// Token: 0x0600040D RID: 1037 RVA: 0x00022BC6 File Offset: 0x00020DC6
			get
			{
				return this.name;
			}
		}

		// Token: 0x040002EF RID: 751
		public static readonly Mode ALPHANUMERIC = new Mode(new int[]
		{
			9,
			11,
			13
		}, 2, "ALPHANUMERIC");

		// Token: 0x040002F8 RID: 760
		private readonly int bits;

		// Token: 0x040002F1 RID: 753
		public static readonly Mode BYTE = new Mode(new int[]
		{
			8,
			16,
			16
		}, 4, "BYTE");

		// Token: 0x040002F7 RID: 759
		private readonly int[] characterCountBitsForVersions;

		// Token: 0x040002F2 RID: 754
		public static readonly Mode ECI = new Mode(null, 7, "ECI");

		// Token: 0x040002F4 RID: 756
		public static readonly Mode FNC1_FIRST_POSITION = new Mode(null, 5, "FNC1_FIRST_POSITION");

		// Token: 0x040002F5 RID: 757
		public static readonly Mode FNC1_SECOND_POSITION = new Mode(null, 9, "FNC1_SECOND_POSITION");

		// Token: 0x040002F6 RID: 758
		public static readonly Mode HANZI = new Mode(new int[]
		{
			8,
			10,
			12
		}, 13, "HANZI");

		// Token: 0x040002F3 RID: 755
		public static readonly Mode KANJI = new Mode(new int[]
		{
			8,
			10,
			12
		}, 8, "KANJI");

		// Token: 0x040002F9 RID: 761
		private readonly string name;

		// Token: 0x040002EE RID: 750
		public static readonly Mode NUMERIC = new Mode(new int[]
		{
			10,
			12,
			14
		}, 1, "NUMERIC");

		// Token: 0x040002F0 RID: 752
		public static readonly Mode STRUCTURED_APPEND = new Mode(new int[3], 3, "STRUCTURED_APPEND");

		// Token: 0x040002ED RID: 749
		public static readonly Mode TERMINATOR = new Mode(new int[3], 0, "TERMINATOR");
	}
}
