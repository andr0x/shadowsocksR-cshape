using System;
using System.Collections.Generic;
using ZXing.Common;
using ZXing.Common.ReedSolomon;

namespace ZXing.QrCode.Internal
{
	// Token: 0x0200006F RID: 111
	public sealed class Decoder
	{
		// Token: 0x060003F8 RID: 1016 RVA: 0x00022439 File Offset: 0x00020639
		public Decoder()
		{
			this.rsDecoder = new ReedSolomonDecoder(GenericGF.QR_CODE_FIELD_256);
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x000225E4 File Offset: 0x000207E4
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

		// Token: 0x060003F9 RID: 1017 RVA: 0x00022454 File Offset: 0x00020654
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

		// Token: 0x060003FA RID: 1018 RVA: 0x0002249C File Offset: 0x0002069C
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

		// Token: 0x060003FB RID: 1019 RVA: 0x00022504 File Offset: 0x00020704
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
			if(formatInformation.ErrorCorrectionLevel==null)
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

		// Token: 0x040002DF RID: 735
		private readonly ReedSolomonDecoder rsDecoder;
	}
}
