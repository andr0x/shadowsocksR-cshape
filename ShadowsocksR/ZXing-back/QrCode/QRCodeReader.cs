using System;
using System.Collections.Generic;
using ZXing.Common;
using ZXing.QrCode.Internal;

namespace ZXing.QrCode
{
	// Token: 0x0200006A RID: 106
	public class QRCodeReader
	{
		// Token: 0x060003D6 RID: 982 RVA: 0x0002143C File Offset: 0x0001F63C
		public Result decode(BinaryBitmap image)
		{
			return this.decode(image, null);
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x00021448 File Offset: 0x0001F648
		public Result decode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
		{
			if (image == null || image.BlackMatrix == null)
			{
				return null;
			}
			DecoderResult decoderResult;
			ResultPoint[] array;
			if (hints != null && hints.ContainsKey(DecodeHintType.PURE_BARCODE))
			{
				BitMatrix bitMatrix = QRCodeReader.extractPureBits(image.BlackMatrix);
				if (bitMatrix == null)
				{
					return null;
				}
				decoderResult = this.decoder.decode(bitMatrix, hints);
				array = QRCodeReader.NO_POINTS;
			}
			else
			{
				DetectorResult detectorResult = new Detector(image.BlackMatrix).detect(hints);
				if (detectorResult == null)
				{
					return null;
				}
				decoderResult = this.decoder.decode(detectorResult.Bits, hints);
				array = detectorResult.Points;
			}
			if (decoderResult == null)
			{
				return null;
			}
			QRCodeDecoderMetaData qRCodeDecoderMetaData = decoderResult.Other as QRCodeDecoderMetaData;
			if (qRCodeDecoderMetaData != null)
			{
				qRCodeDecoderMetaData.applyMirroredCorrection(array);
			}
			Result result = new Result(decoderResult.Text, decoderResult.RawBytes, array, BarcodeFormat.QR_CODE);
			IList<byte[]> byteSegments = decoderResult.ByteSegments;
			if (byteSegments != null)
			{
				result.putMetadata(ResultMetadataType.BYTE_SEGMENTS, byteSegments);
			}
			string eCLevel = decoderResult.ECLevel;
			if (eCLevel != null)
			{
				result.putMetadata(ResultMetadataType.ERROR_CORRECTION_LEVEL, eCLevel);
			}
			if (decoderResult.StructuredAppend)
			{
				result.putMetadata(ResultMetadataType.STRUCTURED_APPEND_SEQUENCE, decoderResult.StructuredAppendSequenceNumber);
				result.putMetadata(ResultMetadataType.STRUCTURED_APPEND_PARITY, decoderResult.StructuredAppendParity);
			}
			return result;
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x00021558 File Offset: 0x0001F758
		private static BitMatrix extractPureBits(BitMatrix image)
		{
			int[] topLeftOnBit = image.getTopLeftOnBit();
			int[] bottomRightOnBit = image.getBottomRightOnBit();
			if (topLeftOnBit == null || bottomRightOnBit == null)
			{
				return null;
			}
			float num;
			if (!QRCodeReader.moduleSize(topLeftOnBit, image, out num))
			{
				return null;
			}
			int num2 = topLeftOnBit[1];
			int num3 = bottomRightOnBit[1];
			int num4 = topLeftOnBit[0];
			int num5 = bottomRightOnBit[0];
			if (num4 >= num5 || num2 >= num3)
			{
				return null;
			}
			if (num3 - num2 != num5 - num4)
			{
				num5 = num4 + (num3 - num2);
			}
			int num6 = (int)Math.Round((double)((float)(num5 - num4 + 1) / num));
			int num7 = (int)Math.Round((double)((float)(num3 - num2 + 1) / num));
			if (num6 <= 0 || num7 <= 0)
			{
				return null;
			}
			if (num7 != num6)
			{
				return null;
			}
			int num8 = (int)(num / 2f);
			num2 += num8;
			num4 += num8;
			int num9 = num4 + (int)((float)(num6 - 1) * num) - (num5 - 1);
			if (num9 > 0)
			{
				if (num9 > num8)
				{
					return null;
				}
				num4 -= num9;
			}
			int num10 = num2 + (int)((float)(num7 - 1) * num) - (num3 - 1);
			if (num10 > 0)
			{
				if (num10 > num8)
				{
					return null;
				}
				num2 -= num10;
			}
			BitMatrix bitMatrix = new BitMatrix(num6, num7);
			for (int i = 0; i < num7; i++)
			{
				int y = num2 + (int)((float)i * num);
				for (int j = 0; j < num6; j++)
				{
					if (image[num4 + (int)((float)j * num), y])
					{
						bitMatrix[j, i] = true;
					}
				}
			}
			return bitMatrix;
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x00021434 File Offset: 0x0001F634
		protected Decoder getDecoder()
		{
			return this.decoder;
		}

		// Token: 0x060003DA RID: 986 RVA: 0x000216AC File Offset: 0x0001F8AC
		private static bool moduleSize(int[] leftTopBlack, BitMatrix image, out float msize)
		{
			int height = image.Height;
			int width = image.Width;
			int num = leftTopBlack[0];
			int num2 = leftTopBlack[1];
			bool flag = true;
			int num3 = 0;
			while (num < width && num2 < height)
			{
				if (flag != image[num, num2])
				{
					if (++num3 == 5)
					{
						break;
					}
					flag = !flag;
				}
				num++;
				num2++;
			}
			if (num == width || num2 == height)
			{
				msize = 0f;
				return false;
			}
			msize = (float)(num - leftTopBlack[0]) / 7f;
			return true;
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x00009AEF File Offset: 0x00007CEF
		public void reset()
		{
		}

		// Token: 0x040002D5 RID: 725
		private readonly Decoder decoder = new Decoder();

		// Token: 0x040002D4 RID: 724
		private static readonly ResultPoint[] NO_POINTS = new ResultPoint[0];
	}
}
