using System;
using System.Collections.Generic;
using System.Text;

namespace ZXing.Common
{
	// Token: 0x0200008D RID: 141
	public static class StringUtils
	{
		// Token: 0x06000509 RID: 1289 RVA: 0x00028AC4 File Offset: 0x00026CC4
		public static string guessEncoding(byte[] bytes, IDictionary<DecodeHintType, object> hints)
		{
			if (hints != null && hints.ContainsKey(DecodeHintType.CHARACTER_SET))
			{
				string text = (string)hints[DecodeHintType.CHARACTER_SET];
				if (text != null)
				{
					return text;
				}
			}
			int num = bytes.Length;
			bool flag = true;
			bool flag2 = true;
			bool flag3 = true;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			int num10 = 0;
			int num11 = 0;
			int num12 = 0;
			bool flag4 = bytes.Length > 3 && bytes[0] == 239 && bytes[1] == 187 && bytes[2] == 191;
			int num13 = 0;
			while (num13 < num && (flag | flag2 | flag3))
			{
				int num14 = (int)(bytes[num13] & 255);
				if (flag3)
				{
					if (num2 > 0)
					{
						if ((num14 & 128) == 0)
						{
							flag3 = false;
						}
						else
						{
							num2--;
						}
					}
					else if ((num14 & 128) != 0)
					{
						if ((num14 & 64) == 0)
						{
							flag3 = false;
						}
						else
						{
							num2++;
							if ((num14 & 32) == 0)
							{
								num3++;
							}
							else
							{
								num2++;
								if ((num14 & 16) == 0)
								{
									num4++;
								}
								else
								{
									num2++;
									if ((num14 & 8) == 0)
									{
										num5++;
									}
									else
									{
										flag3 = false;
									}
								}
							}
						}
					}
				}
				if (flag)
				{
					if (num14 > 127 && num14 < 160)
					{
						flag = false;
					}
					else if (num14 > 159 && (num14 < 192 || num14 == 215 || num14 == 247))
					{
						num12++;
					}
				}
				if (flag2)
				{
					if (num6 > 0)
					{
						if (num14 < 64 || num14 == 127 || num14 > 252)
						{
							flag2 = false;
						}
						else
						{
							num6--;
						}
					}
					else if (num14 == 128 || num14 == 160 || num14 > 239)
					{
						flag2 = false;
					}
					else if (num14 > 160 && num14 < 224)
					{
						num7++;
						num9 = 0;
						num8++;
						if (num8 > num10)
						{
							num10 = num8;
						}
					}
					else if (num14 > 127)
					{
						num6++;
						num8 = 0;
						num9++;
						if (num9 > num11)
						{
							num11 = num9;
						}
					}
					else
					{
						num8 = 0;
						num9 = 0;
					}
				}
				num13++;
			}
			if (flag3 && num2 > 0)
			{
				flag3 = false;
			}
			if (flag2 && num6 > 0)
			{
				flag2 = false;
			}
			if (flag3 && (flag4 || num3 + num4 + num5 > 0))
			{
				return "UTF-8";
			}
			if (flag2 && (StringUtils.ASSUME_SHIFT_JIS || num10 >= 3 || num11 >= 3))
			{
				return StringUtils.SHIFT_JIS;
			}
			if (flag & flag2)
			{
				if ((num10 != 2 || num7 != 2) && num12 * 10 < num)
				{
					return "ISO-8859-1";
				}
				return StringUtils.SHIFT_JIS;
			}
			else
			{
				if (flag)
				{
					return "ISO-8859-1";
				}
				if (flag2)
				{
					return StringUtils.SHIFT_JIS;
				}
				if (flag3)
				{
					return "UTF-8";
				}
				return StringUtils.PLATFORM_DEFAULT_ENCODING;
			}
		}

		// Token: 0x04000361 RID: 865
		private static readonly bool ASSUME_SHIFT_JIS = string.Compare(StringUtils.SHIFT_JIS, StringUtils.PLATFORM_DEFAULT_ENCODING, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare("EUC-JP", StringUtils.PLATFORM_DEFAULT_ENCODING, StringComparison.OrdinalIgnoreCase) == 0;

		// Token: 0x0400035E RID: 862
		private const string EUC_JP = "EUC-JP";

		// Token: 0x0400035D RID: 861
		public static string GB2312 = "GB2312";

		// Token: 0x04000360 RID: 864
		private const string ISO88591 = "ISO-8859-1";

		// Token: 0x0400035B RID: 859
		private static string PLATFORM_DEFAULT_ENCODING = Encoding.Default.WebName;

		// Token: 0x0400035C RID: 860
		public static string SHIFT_JIS = "SJIS";

		// Token: 0x0400035F RID: 863
		private const string UTF8 = "UTF-8";
	}
}
