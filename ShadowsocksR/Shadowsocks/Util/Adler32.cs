using System;

namespace Shadowsocks.Util
{
	// Token: 0x0200000C RID: 12
	internal class Adler32
	{
		// Token: 0x060000A3 RID: 163 RVA: 0x0000CC68 File Offset: 0x0000AE68
		public static ulong CalcAdler32(byte[] input, int len)
		{
			ulong num = 1uL;
			ulong num2 = 0uL;
			for (int i = 0; i < len; i++)
			{
				num += (ulong)input[i];
				num2 += num;
			}
			num %= 65521uL;
			num2 %= 65521uL;
			return (num2 << 16) + num;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x0000CCAC File Offset: 0x0000AEAC
		public static bool CheckAdler32(byte[] input, int len)
		{
			int arg_29_0 = (int)Adler32.CalcAdler32(input, len - 4);
			int num = (int)input[len - 1] << 24 | (int)input[len - 2] << 16 | (int)input[len - 3] << 8 | (int)input[len - 4];
			return arg_29_0 == num;
		}
	}
}
