using System;
using System.Collections.Generic;
using ZXing.Common;
using ZXing.Common.ReedSolomon;

namespace ZXing.QrCode.Internal
{
	// Token: 0x02000071 RID: 113
	public sealed class Decoder
	{
		// Token: 0x06000401 RID: 1025 RVA: 0x00021681 File Offset: 0x0001F881
		public Decoder()
		{
			this.rsDecoder = new ReedSolomonDecoder(GenericGF.QR_CODE_FIELD_256);
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x0002182C File Offset: 0x0001FA2C
		private bool correctErrors(byte[] codewordBytes, int numDataCodewords)
		{
			int num = codewordBytes.Length;
			int[] array = new int[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = (int)(codewordBytes[i] & 255);
			}
			int twoS = codewordBytes.Length - numDataCodewords;
			if (!this.rsDecoder.decode(array, twoS))
			{
				return false;
			}
			for (int j = 0; j < numDataCodewords; j++)
			{
				codewordBytes[j] = (byte)array[j];
			}
			return true;
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x0002169C File Offset: 0x0001F89C
		public DecoderResult decode(bool[][] image, IDictionary<DecodeHintType, object> hints)
		{
			int num = image.Length;
			BitMatrix bitMatrix = new BitMatrix(num);
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num; j++)
				{
					bitMatrix[j, i] = image[i][j];
				}
			}
			return this.decode(bitMatrix, hints);
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x000216E4 File Offset: 0x0001F8E4
		public DecoderResult decode(BitMatrix bits, IDictionary<DecodeHintType, object> hints)
		{
			BitMatrixParser bitMatrixParser = BitMatrixParser.createBitMatrixParser(bits);
			if (bitMatrixParser == null)
			{
				return null;
			}
			DecoderResult decoderResult = this.decode(bitMatrixParser, hints);
			if (decoderResult == null)
			{
				bitMatrixParser.remask();
				bitMatrixParser.setMirror(true);
				if (bitMatrixParser.readVersion() == null)
				{
					return null;
				}
				if (bitMatrixParser.readFormatInformation() == null)
				{
					return null;
				}
				bitMatrixParser.mirror();
				decoderResult = this.decode(bitMatrixParser, hints);
				if (decoderResult != null)
				{
					decoderResult.Other = new QRCodeDecoderMetaData(true);
				}
			}
			return decoderResult;
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x0002174C File Offset: 0x0001F94C
		private DecoderResult decode(BitMatrixParser parser, IDictionary<DecodeHintType, object> hints)
		{
			Version version = parser.readVersion();
			if (version == null)
			{
				return null;
			}
			FormatInformation formatInformation = parser.readFormatInformation();
			if (formatInformation == null)
			{
				return null;
			}
			ErrorCorrectionLevel errorCorrectionLevel = formatInformation.ErrorCorrectionLevel;
			byte[] array = parser.readCodewords();
			if (array == null)
			{
				return null;
			}
			DataBlock[] dataBlocks = DataBlock.getDataBlocks(array, version, errorCorrectionLevel);
			int num = 0;
			DataBlock[] array2 = dataBlocks;
			for (int i = 0; i < array2.Length; i++)
			{
				DataBlock dataBlock = array2[i];
				num += dataBlock.NumDataCodewords;
			}
			byte[] array3 = new byte[num];
			int num2 = 0;
			array2 = dataBlocks;
			for (int i = 0; i < array2.Length; i++)
			{
				DataBlock expr_7C = array2[i];
				byte[] codewords = expr_7C.Codewords;
				int numDataCodewords = expr_7C.NumDataCodewords;
				if (!this.correctErrors(codewords, numDataCodewords))
				{
					return null;
				}
				for (int j = 0; j < numDataCodewords; j++)
				{
					array3[num2++] = codewords[j];
				}
			}
			return DecodedBitStreamParser.decode(array3, version, errorCorrectionLevel, hints);
		}

		// Token: 0x040002DC RID: 732
		private readonly ReedSolomonDecoder rsDecoder;
	}
}
