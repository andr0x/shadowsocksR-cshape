using System;
using System.Collections.Generic;
using System.Text;
using ZXing.Common;
using ZXing.Common.ReedSolomon;

namespace ZXing.QrCode.Internal
{
	// Token: 0x0200007F RID: 127
	public static class Encoder
	{
		// Token: 0x0600047A RID: 1146 RVA: 0x00026198 File Offset: 0x00024398
		internal static void append8BitBytes(string content, BitArray bits, string encoding)
		{
			byte[] bytes;
			try
			{
				bytes = Encoding.GetEncoding(encoding).GetBytes(content);
			}
			catch (Exception ex)
			{
				throw new WriterException(ex.Message, ex);
			}
			byte[] array = bytes;
			for (int i = 0; i < array.Length; i++)
			{
				byte value = array[i];
				bits.appendBits((int)value, 8);
			}
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x000260D5 File Offset: 0x000242D5
		internal static void appendBytes(string content, Mode mode, BitArray bits, string encoding)
		{
			if (mode.Equals(Mode.BYTE))
			{
				Encoder.append8BitBytes(content, bits, encoding);
				return;
			}
			throw new WriterException("Invalid mode: " + mode);
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x0002608C File Offset: 0x0002428C
		internal static void appendLengthInfo(int numLetters, Version version, Mode mode, BitArray bits)
		{
			int characterCountBits = mode.getCharacterCountBits(version);
			if (numLetters >= 1 << characterCountBits)
			{
				throw new WriterException(numLetters + " is bigger than " + ((1 << characterCountBits) - 1));
			}
			bits.appendBits(numLetters, characterCountBits);
		}

		// Token: 0x06000476 RID: 1142 RVA: 0x0002607A File Offset: 0x0002427A
		internal static void appendModeInfo(Mode mode, BitArray bits)
		{
			bits.appendBits(mode.Bits, 4);
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x00026100 File Offset: 0x00024300
		internal static void appendNumericBytes(string content, BitArray bits)
		{
			int length = content.Length;
			int i = 0;
			while (i < length)
			{
				int num = (int)(content[i] - '0');
				if (i + 2 < length)
				{
					int num2 = (int)(content[i + 1] - '0');
					int num3 = (int)(content[i + 2] - '0');
					bits.appendBits(num * 100 + num2 * 10 + num3, 10);
					i += 3;
				}
				else if (i + 1 < length)
				{
					int num4 = (int)(content[i + 1] - '0');
					bits.appendBits(num * 10 + num4, 7);
					i += 2;
				}
				else
				{
					bits.appendBits(num, 4);
					i++;
				}
			}
		}

		// Token: 0x0600046A RID: 1130 RVA: 0x00025A7D File Offset: 0x00023C7D
		private static int calculateMaskPenalty(ByteMatrix matrix)
		{
			return MaskUtil.applyMaskPenaltyRule1(matrix) + MaskUtil.applyMaskPenaltyRule2(matrix) + MaskUtil.applyMaskPenaltyRule3(matrix) + MaskUtil.applyMaskPenaltyRule4(matrix);
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x00025C38 File Offset: 0x00023E38
		private static int chooseMaskPattern(BitArray bits, ErrorCorrectionLevel ecLevel, Version version, ByteMatrix matrix)
		{
			int num = 2147483647;
			int result = -1;
			for (int i = 0; i < QRCode.NUM_MASK_PATTERNS; i++)
			{
				MatrixUtil.buildMatrix(bits, ecLevel, version, i, matrix);
				int num2 = Encoder.calculateMaskPenalty(matrix);
				if (num2 < num)
				{
					num = num2;
					result = i;
				}
			}
			return result;
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x00025C26 File Offset: 0x00023E26
		public static Mode chooseMode(string content)
		{
			return Encoder.chooseMode(content, null);
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x00025C2F File Offset: 0x00023E2F
		private static Mode chooseMode(string content, string encoding)
		{
			return Mode.BYTE;
		}

		// Token: 0x06000471 RID: 1137 RVA: 0x00025C78 File Offset: 0x00023E78
		private static Version chooseVersion(int numInputBits, ErrorCorrectionLevel ecLevel)
		{
			for (int i = 1; i <= 40; i++)
			{
				Version versionForNumber = Version.getVersionForNumber(i);
				int totalCodewords = versionForNumber.TotalCodewords;
				int totalECCodewords = versionForNumber.getECBlocksForLevel(ecLevel).TotalECCodewords;
				int arg_2B_0 = totalCodewords - totalECCodewords;
				int num = (numInputBits + 7) / 8;
				if (arg_2B_0 >= num)
				{
					return versionForNumber;
				}
			}
			throw new WriterException("Data too big");
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x00025A9A File Offset: 0x00023C9A
		public static QRCode encode(string content, ErrorCorrectionLevel ecLevel)
		{
			return Encoder.encode(content, ecLevel, null);
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x00025AA4 File Offset: 0x00023CA4
		public static QRCode encode(string content, ErrorCorrectionLevel ecLevel, IDictionary<EncodeHintType, object> hints)
		{
			string text = (hints == null || !hints.ContainsKey(EncodeHintType.CHARACTER_SET)) ? null : ((string)hints[EncodeHintType.CHARACTER_SET]);
			if (text == null)
			{
				text = Encoder.DEFAULT_BYTE_MODE_ENCODING;
			}
			Encoder.DEFAULT_BYTE_MODE_ENCODING.Equals(text);
			Mode mode = Encoder.chooseMode(content, text);
			BitArray bitArray = new BitArray();
			Encoder.appendModeInfo(mode, bitArray);
			BitArray bitArray2 = new BitArray();
			Encoder.appendBytes(content, mode, bitArray2, text);
			Version version = Encoder.chooseVersion(bitArray.Size + mode.getCharacterCountBits(Version.getVersionForNumber(1)) + bitArray2.Size, ecLevel);
			Version version2 = Encoder.chooseVersion(bitArray.Size + mode.getCharacterCountBits(version) + bitArray2.Size, ecLevel);
			BitArray bitArray3 = new BitArray();
			bitArray3.appendBitArray(bitArray);
			Encoder.appendLengthInfo((mode == Mode.BYTE) ? bitArray2.SizeInBytes : content.Length, version2, mode, bitArray3);
			bitArray3.appendBitArray(bitArray2);
			Version.ECBlocks eCBlocksForLevel = version2.getECBlocksForLevel(ecLevel);
			int numDataBytes = version2.TotalCodewords - eCBlocksForLevel.TotalECCodewords;
			Encoder.terminateBits(numDataBytes, bitArray3);
			BitArray arg_133_0 = Encoder.interleaveWithECBytes(bitArray3, version2.TotalCodewords, numDataBytes, eCBlocksForLevel.NumBlocks);
			QRCode qRCode = new QRCode
			{
				ECLevel = ecLevel,
				Mode = mode,
				Version = version2
			};
			int expr_12B = version2.DimensionForVersion;
			ByteMatrix matrix = new ByteMatrix(expr_12B, expr_12B);
			int maskPattern = Encoder.chooseMaskPattern(arg_133_0, ecLevel, version2, matrix);
			qRCode.MaskPattern = maskPattern;
			MatrixUtil.buildMatrix(arg_133_0, ecLevel, version2, maskPattern, matrix);
			qRCode.Matrix = matrix;
			return qRCode;
		}

		// Token: 0x06000475 RID: 1141 RVA: 0x00026014 File Offset: 0x00024214
		internal static byte[] generateECBytes(byte[] dataBytes, int numEcBytesInBlock)
		{
			int num = dataBytes.Length;
			int[] array = new int[num + numEcBytesInBlock];
			for (int i = 0; i < num; i++)
			{
				array[i] = (int)(dataBytes[i] & 255);
			}
			new ReedSolomonEncoder(GenericGF.QR_CODE_FIELD_256).encode(array, numEcBytesInBlock);
			byte[] array2 = new byte[numEcBytesInBlock];
			for (int j = 0; j < numEcBytesInBlock; j++)
			{
				array2[j] = (byte)array[num + j];
			}
			return array2;
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x00025C11 File Offset: 0x00023E11
		internal static int getAlphanumericCode(int code)
		{
			if (code < Encoder.ALPHANUMERIC_TABLE.Length)
			{
				return Encoder.ALPHANUMERIC_TABLE[code];
			}
			return -1;
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x00025DA0 File Offset: 0x00023FA0
		internal static void getNumDataBytesAndNumECBytesForBlockID(int numTotalBytes, int numDataBytes, int numRSBlocks, int blockID, int[] numDataBytesInBlock, int[] numECBytesInBlock)
		{
			if (blockID >= numRSBlocks)
			{
				throw new WriterException("Block ID too large");
			}
			int num = numTotalBytes % numRSBlocks;
			int num2 = numRSBlocks - num;
			int expr_1A = numTotalBytes / numRSBlocks;
			int num3 = expr_1A + 1;
			int num4 = numDataBytes / numRSBlocks;
			int num5 = num4 + 1;
			int num6 = expr_1A - num4;
			int num7 = num3 - num5;
			if (num6 != num7)
			{
				throw new WriterException("EC bytes mismatch");
			}
			if (numRSBlocks != num2 + num)
			{
				throw new WriterException("RS blocks mismatch");
			}
			if (numTotalBytes != (num4 + num6) * num2 + (num5 + num7) * num)
			{
				throw new WriterException("Total bytes mismatch");
			}
			if (blockID < num2)
			{
				numDataBytesInBlock[0] = num4;
				numECBytesInBlock[0] = num6;
				return;
			}
			numDataBytesInBlock[0] = num5;
			numECBytesInBlock[0] = num7;
		}

		// Token: 0x06000474 RID: 1140 RVA: 0x00025E38 File Offset: 0x00024038
		internal static BitArray interleaveWithECBytes(BitArray bits, int numTotalBytes, int numDataBytes, int numRSBlocks)
		{
			if (bits.SizeInBytes != numDataBytes)
			{
				throw new WriterException("Number of bits and data bytes does not match");
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			List<BlockPair> list = new List<BlockPair>(numRSBlocks);
			for (int i = 0; i < numRSBlocks; i++)
			{
				int[] array = new int[1];
				int[] array2 = new int[1];
				Encoder.getNumDataBytesAndNumECBytesForBlockID(numTotalBytes, numDataBytes, numRSBlocks, i, array, array2);
				int num4 = array[0];
				byte[] array3 = new byte[num4];
				bits.toBytes(8 * num, array3, 0, num4);
				byte[] array4 = Encoder.generateECBytes(array3, array2[0]);
				list.Add(new BlockPair(array3, array4));
				num2 = Math.Max(num2, num4);
				num3 = Math.Max(num3, array4.Length);
				num += array[0];
			}
			if (numDataBytes != num)
			{
				throw new WriterException("Data bytes does not match offset");
			}
			BitArray bitArray = new BitArray();
			for (int j = 0; j < num2; j++)
			{
				using (List<BlockPair>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						byte[] dataBytes = enumerator.Current.DataBytes;
						if (j < dataBytes.Length)
						{
							bitArray.appendBits((int)dataBytes[j], 8);
						}
					}
				}
			}
			for (int k = 0; k < num3; k++)
			{
				using (List<BlockPair>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						byte[] errorCorrectionBytes = enumerator.Current.ErrorCorrectionBytes;
						if (k < errorCorrectionBytes.Length)
						{
							bitArray.appendBits((int)errorCorrectionBytes[k], 8);
						}
					}
				}
			}
			if (numTotalBytes != bitArray.SizeInBytes)
			{
				throw new WriterException(string.Concat(new object[]
				{
					"Interleaving error: ",
					numTotalBytes,
					" and ",
					bitArray.SizeInBytes,
					" differ."
				}));
			}
			return bitArray;
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x00025CC8 File Offset: 0x00023EC8
		internal static void terminateBits(int numDataBytes, BitArray bits)
		{
			int num = numDataBytes << 3;
			if (bits.Size > num)
			{
				throw new WriterException(string.Concat(new object[]
				{
					"data bits cannot fit in the QR Code",
					bits.Size,
					" > ",
					num
				}));
			}
			int num2 = 0;
			while (num2 < 4 && bits.Size < num)
			{
				bits.appendBit(false);
				num2++;
			}
			int num3 = bits.Size & 7;
			if (num3 > 0)
			{
				for (int i = num3; i < 8; i++)
				{
					bits.appendBit(false);
				}
			}
			int num4 = numDataBytes - bits.SizeInBytes;
			for (int j = 0; j < num4; j++)
			{
				bits.appendBits(((j & 1) == 0) ? 236 : 17, 8);
			}
			if (bits.Size != num)
			{
				throw new WriterException("Bits size does not equal capacity");
			}
		}

		// Token: 0x0400031D RID: 797
		private static readonly int[] ALPHANUMERIC_TABLE = new int[]
		{
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			36,
			-1,
			-1,
			-1,
			37,
			38,
			-1,
			-1,
			-1,
			-1,
			39,
			40,
			-1,
			41,
			42,
			43,
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			44,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			10,
			11,
			12,
			13,
			14,
			15,
			16,
			17,
			18,
			19,
			20,
			21,
			22,
			23,
			24,
			25,
			26,
			27,
			28,
			29,
			30,
			31,
			32,
			33,
			34,
			35,
			-1,
			-1,
			-1,
			-1,
			-1
		};

		// Token: 0x0400031E RID: 798
		internal static string DEFAULT_BYTE_MODE_ENCODING = "ISO-8859-1";
	}
}
