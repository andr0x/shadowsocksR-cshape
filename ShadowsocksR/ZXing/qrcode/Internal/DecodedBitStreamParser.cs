using System;
using System.Collections.Generic;
using System.Text;
using ZXing.Common;

namespace ZXing.QrCode.Internal
{
	// Token: 0x02000070 RID: 112
	internal static class DecodedBitStreamParser
	{
		// Token: 0x060003F8 RID: 1016 RVA: 0x00021034 File Offset: 0x0001F234
		internal static DecoderResult decode(byte[] bytes, Version version, ErrorCorrectionLevel ecLevel, IDictionary<DecodeHintType, object> hints)
		{
			BitSource bitSource = new BitSource(bytes);
			StringBuilder stringBuilder = new StringBuilder(50);
			List<byte[]> list = new List<byte[]>(1);
			int saSequence = -1;
			int saParity = -1;
			try
			{
				bool fc1InEffect = false;
				DecoderResult result;
				while (true)
				{
					Mode mode;
					if (bitSource.available() < 4)
					{
						mode = Mode.TERMINATOR;
					}
					else
					{
						try
						{
							mode = Mode.forBits(bitSource.readBits(4));
						}
						catch (ArgumentException)
						{
							result = null;
							return result;
						}
					}
					if (mode != Mode.TERMINATOR)
					{
						if (mode == Mode.FNC1_FIRST_POSITION || mode == Mode.FNC1_SECOND_POSITION)
						{
							fc1InEffect = true;
						}
						else if (mode == Mode.STRUCTURED_APPEND)
						{
							if (bitSource.available() < 16)
							{
								break;
							}
							saSequence = bitSource.readBits(8);
							saParity = bitSource.readBits(8);
						}
						else if (mode != Mode.ECI)
						{
							if (mode == Mode.HANZI)
							{
								int num = bitSource.readBits(4);
								int count = bitSource.readBits(mode.getCharacterCountBits(version));
								if (num == 1 && !DecodedBitStreamParser.decodeHanziSegment(bitSource, stringBuilder, count))
								{
									goto Block_14;
								}
							}
							else
							{
								int count2 = bitSource.readBits(mode.getCharacterCountBits(version));
								if (mode == Mode.NUMERIC)
								{
									if (!DecodedBitStreamParser.decodeNumericSegment(bitSource, stringBuilder, count2))
									{
										goto Block_16;
									}
								}
								else if (mode == Mode.ALPHANUMERIC)
								{
									if (!DecodedBitStreamParser.decodeAlphanumericSegment(bitSource, stringBuilder, count2, fc1InEffect))
									{
										goto Block_18;
									}
								}
								else if (mode == Mode.BYTE)
								{
									if (!DecodedBitStreamParser.decodeByteSegment(bitSource, stringBuilder, count2, list, hints))
									{
										goto Block_20;
									}
								}
								else
								{
									if (mode != Mode.KANJI)
									{
										goto IL_16B;
									}
									if (!DecodedBitStreamParser.decodeKanjiSegment(bitSource, stringBuilder, count2))
									{
										goto Block_22;
									}
								}
							}
						}
					}
					if (mode == Mode.TERMINATOR)
					{
						goto Block_23;
					}
				}
				result = null;
				return result;
				Block_14:
				result = null;
				return result;
				Block_16:
				result = null;
				return result;
				Block_18:
				result = null;
				return result;
				Block_20:
				result = null;
				return result;
				Block_22:
				result = null;
				return result;
				IL_16B:
				result = null;
				return result;
				Block_23:;
			}
			catch (ArgumentException)
			{
				DecoderResult result = null;
				return result;
			}
			string text = stringBuilder.ToString().Replace("\r\n", "\n").Replace("\n", Environment.NewLine);
			return new DecoderResult(bytes, text, (list.Count == 0) ? null : list, (ecLevel == null) ? null : ecLevel.ToString(), saSequence, saParity);
		}

		// Token: 0x060003FD RID: 1021 RVA: 0x00021440 File Offset: 0x0001F640
		private static bool decodeAlphanumericSegment(BitSource bits, StringBuilder result, int count, bool fc1InEffect)
		{
			int length = result.Length;
			while (count > 1)
			{
				if (bits.available() < 11)
				{
					return false;
				}
				int num = bits.readBits(11);
				result.Append(DecodedBitStreamParser.toAlphaNumericChar(num / 45));
				result.Append(DecodedBitStreamParser.toAlphaNumericChar(num % 45));
				count -= 2;
			}
			if (count == 1)
			{
				if (bits.available() < 6)
				{
					return false;
				}
				result.Append(DecodedBitStreamParser.toAlphaNumericChar(bits.readBits(6)));
			}
			if (fc1InEffect)
			{
				for (int i = length; i < result.Length; i++)
				{
					if (result[i] == '%')
					{
						if (i < result.Length - 1 && result[i + 1] == '%')
						{
							result.Remove(i + 1, 1);
						}
						else
						{
							result.Remove(i, 1);
							result.Insert(i, new char[]
							{
								'\u001d'
							});
						}
					}
				}
			}
			return true;
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x000213B0 File Offset: 0x0001F5B0
		private static bool decodeByteSegment(BitSource bits, StringBuilder result, int count, IList<byte[]> byteSegments, IDictionary<DecodeHintType, object> hints)
		{
			if (count << 3 > bits.available())
			{
				return false;
			}
			byte[] array = new byte[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = (byte)bits.readBits(8);
			}
			string name = StringUtils.guessEncoding(array, hints);
			try
			{
				result.Append(Encoding.GetEncoding(name).GetString(array, 0, array.Length));
			}
			catch (Exception)
			{
				return false;
			}
			byteSegments.Add(array);
			return true;
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x00021248 File Offset: 0x0001F448
		private static bool decodeHanziSegment(BitSource bits, StringBuilder result, int count)
		{
			if (count * 13 > bits.available())
			{
				return false;
			}
			byte[] array = new byte[2 * count];
			int num = 0;
			while (count > 0)
			{
				int num2 = bits.readBits(13);
				int num3 = num2 / 96 << 8 | num2 % 96;
				if (num3 < 959)
				{
					num3 += 41377;
				}
				else
				{
					num3 += 42657;
				}
				array[num] = (byte)(num3 >> 8 & 255);
				array[num + 1] = (byte)(num3 & 255);
				num += 2;
				count--;
			}
			try
			{
				result.Append(Encoding.GetEncoding(StringUtils.GB2312).GetString(array, 0, array.Length));
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x00021300 File Offset: 0x0001F500
		private static bool decodeKanjiSegment(BitSource bits, StringBuilder result, int count)
		{
			if (count * 13 > bits.available())
			{
				return false;
			}
			byte[] array = new byte[2 * count];
			int num = 0;
			while (count > 0)
			{
				int num2 = bits.readBits(13);
				int num3 = num2 / 192 << 8 | num2 % 192;
				if (num3 < 7936)
				{
					num3 += 33088;
				}
				else
				{
					num3 += 49472;
				}
				array[num] = (byte)(num3 >> 8);
				array[num + 1] = (byte)num3;
				num += 2;
				count--;
			}
			try
			{
				result.Append(Encoding.GetEncoding(StringUtils.SHIFT_JIS).GetString(array, 0, array.Length));
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x00021518 File Offset: 0x0001F718
		private static bool decodeNumericSegment(BitSource bits, StringBuilder result, int count)
		{
			while (count >= 3)
			{
				if (bits.available() < 10)
				{
					return false;
				}
				int num = bits.readBits(10);
				if (num >= 1000)
				{
					return false;
				}
				result.Append(DecodedBitStreamParser.toAlphaNumericChar(num / 100));
				result.Append(DecodedBitStreamParser.toAlphaNumericChar(num / 10 % 10));
				result.Append(DecodedBitStreamParser.toAlphaNumericChar(num % 10));
				count -= 3;
			}
			if (count == 2)
			{
				if (bits.available() < 7)
				{
					return false;
				}
				int num2 = bits.readBits(7);
				if (num2 >= 100)
				{
					return false;
				}
				result.Append(DecodedBitStreamParser.toAlphaNumericChar(num2 / 10));
				result.Append(DecodedBitStreamParser.toAlphaNumericChar(num2 % 10));
			}
			else if (count == 1)
			{
				if (bits.available() < 4)
				{
					return false;
				}
				int num3 = bits.readBits(4);
				if (num3 >= 10)
				{
					return false;
				}
				result.Append(DecodedBitStreamParser.toAlphaNumericChar(num3));
			}
			return true;
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x000215F0 File Offset: 0x0001F7F0
		private static int parseECIValue(BitSource bits)
		{
			int num = bits.readBits(8);
			if ((num & 128) == 0)
			{
				return num & 127;
			}
			if ((num & 192) == 128)
			{
				int num2 = bits.readBits(8);
				return (num & 63) << 8 | num2;
			}
			if ((num & 224) == 192)
			{
				int num3 = bits.readBits(16);
				return (num & 31) << 16 | num3;
			}
			throw new ArgumentException("Bad ECI bits starting with byte " + num);
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x0002142C File Offset: 0x0001F62C
		private static char toAlphaNumericChar(int value)
		{
			int arg_08_0 = DecodedBitStreamParser.ALPHANUMERIC_CHARS.Length;
			return DecodedBitStreamParser.ALPHANUMERIC_CHARS[value];
		}

		// Token: 0x040002DA RID: 730
		private static readonly char[] ALPHANUMERIC_CHARS = new char[]
		{
			'0',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F',
			'G',
			'H',
			'I',
			'J',
			'K',
			'L',
			'M',
			'N',
			'O',
			'P',
			'Q',
			'R',
			'S',
			'T',
			'U',
			'V',
			'W',
			'X',
			'Y',
			'Z',
			' ',
			'$',
			'%',
			'*',
			'+',
			'-',
			'.',
			'/',
			':'
		};

		// Token: 0x040002DB RID: 731
		private const int GB2312_SUBSET = 1;
	}
}
