using System;

namespace Shadowsocks.Util
{
	// Token: 0x0200000B RID: 11
	internal class CRC32
	{
		// Token: 0x0600009B RID: 155 RVA: 0x0000CDE1 File Offset: 0x0000AFE1
		public static ulong CalcCRC32(byte[] input, int len, ulong value = 4294967295uL)
		{
			return CRC32.CalcCRC32(input, 0, len, value);
		}

		// Token: 0x0600009C RID: 156 RVA: 0x0000CDEC File Offset: 0x0000AFEC
		public static ulong CalcCRC32(byte[] input, int index, int len, ulong value = 4294967295uL)
		{
			for (int i = index; i < len; i++)
			{
				value = (value >> 8 ^ CRC32.Crc32Table[(int)(checked((IntPtr)((value & 255uL) ^ unchecked((ulong)input[i]))))]);
			}
			return unchecked (value ^ (ulong)-1);
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x0000CE7B File Offset: 0x0000B07B
		public static bool CheckCRC32(byte[] buffer, int length)
		{
			return CRC32.CalcCRC32(buffer, length, unchecked ((ulong)-1)) ==unchecked ( (ulong)-1);
		}

		// Token: 0x0600009A RID: 154 RVA: 0x0000CD80 File Offset: 0x0000AF80
		public static ulong[] CreateCRC32Table()
		{
			CRC32.Crc32Table = new ulong[256];
			for (int i = 0; i < 256; i++)
			{
				ulong num = (ulong)((long)i);
				for (int j = 8; j > 0; j--)
				{
					if ((num & 1uL) == 1uL)
					{
						num = (num >> 1 ^ unchecked ((ulong)-306674912));
					}
					else
					{
						num >>= 1;
					}
				}
				CRC32.Crc32Table[i] = num;
			}
			return CRC32.Crc32Table;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x0000CE25 File Offset: 0x0000B025
		public static void SetCRC32(byte[] buffer)
		{
			CRC32.SetCRC32(buffer, 0, buffer.Length);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x0000CE31 File Offset: 0x0000B031
		public static void SetCRC32(byte[] buffer, int length)
		{
			CRC32.SetCRC32(buffer, 0, length);
		}

		// Token: 0x0600009F RID: 159 RVA: 0x0000CE3C File Offset: 0x0000B03C
		public static void SetCRC32(byte[] buffer, int index, int length)
		{
			ulong num = ~CRC32.CalcCRC32(buffer, index, length - 4, unchecked ((ulong)-1));
			buffer[length - 1] = (byte)(num >> 24);
			buffer[length - 2] = (byte)(num >> 16);
			buffer[length - 3] = (byte)(num >> 8);
			buffer[length - 4] = (byte)num;
		}

		// Token: 0x040000B8 RID: 184
		protected static ulong[] Crc32Table = CRC32.CreateCRC32Table();
	}
}
