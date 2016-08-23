using System;
using System.Collections.Generic;
using ZXing.Common;
using ZXing.QrCode.Internal;

namespace ZXing.QrCode
{
	// Token: 0x0200006C RID: 108
	public class QRCodeReader
	{
		// Token: 0x060003DF RID: 991 RVA: 0x00020684 File Offset: 0x0001E884
		public Result decode(BinaryBitmap image)
		{
			return this.decode(image, null);
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x00020690 File Offset: 0x0001E890
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

		// Token: 0x060003E2 RID: 994 RVA: 0x000207A0 File Offset: 0x0001E9A0
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

		// Token: 0x060003DE RID: 990 RVA: 0x0002067C File Offset: 0x0001E87C
		protected Decoder getDecoder()
		{
			return this.decoder;
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x000208F4 File Offset: 0x0001EAF4
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

		// Token: 0x060003E1 RID: 993 RVA: 0x00009C9F File Offset: 0x00007E9F
		public void reset()
		{
		}

		// Token: 0x040002D2 RID: 722
		private readonly Decoder decoder = new Decoder();

		// Token: 0x040002D1 RID: 721
		private static readonly ResultPoint[] NO_POINTS = new ResultPoint[0];
	}
}
