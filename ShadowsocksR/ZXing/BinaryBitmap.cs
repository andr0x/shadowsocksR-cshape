using System;
using ZXing.Common;

namespace ZXing
{
	// Token: 0x02000062 RID: 98
	public sealed class BinaryBitmap
	{
		// Token: 0x0600039F RID: 927 RVA: 0x0001FAFF File Offset: 0x0001DCFF
		public BinaryBitmap(Binarizer binarizer)
		{
			if (binarizer == null)
			{
				throw new ArgumentException("Binarizer must be non-null.");
			}
			this.binarizer = binarizer;
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x0001FB78 File Offset: 0x0001DD78
		public BinaryBitmap crop(int left, int top, int width, int height)
		{
			LuminanceSource source = this.binarizer.LuminanceSource.crop(left, top, width, height);
			return new BinaryBitmap(this.binarizer.createBinarizer(source));
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x0001FB36 File Offset: 0x0001DD36
		public BitArray getBlackRow(int y, BitArray row)
		{
			return this.binarizer.getBlackRow(y, row);
		}

		// Token: 0x060003A7 RID: 935 RVA: 0x0001FBC0 File Offset: 0x0001DDC0
		public BinaryBitmap rotateCounterClockwise()
		{
			LuminanceSource source = this.binarizer.LuminanceSource.rotateCounterClockwise();
			return new BinaryBitmap(this.binarizer.createBinarizer(source));
		}

		// Token: 0x060003A8 RID: 936 RVA: 0x0001FBF0 File Offset: 0x0001DDF0
		public BinaryBitmap rotateCounterClockwise45()
		{
			LuminanceSource source = this.binarizer.LuminanceSource.rotateCounterClockwise45();
			return new BinaryBitmap(this.binarizer.createBinarizer(source));
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x0001FC20 File Offset: 0x0001DE20
		public override string ToString()
		{
			BitMatrix blackMatrix = this.BlackMatrix;
			if (blackMatrix == null)
			{
				return string.Empty;
			}
			return blackMatrix.ToString();
		}

		// Token: 0x17000035 RID: 53
		public BitMatrix BlackMatrix
		{
			// Token: 0x060003A3 RID: 931 RVA: 0x0001FB45 File Offset: 0x0001DD45
			get
			{
				if (this.matrix == null)
				{
					this.matrix = this.binarizer.BlackMatrix;
				}
				return this.matrix;
			}
		}

		// Token: 0x17000036 RID: 54
		public bool CropSupported
		{
			// Token: 0x060003A4 RID: 932 RVA: 0x0001FB66 File Offset: 0x0001DD66
			get
			{
				return this.binarizer.LuminanceSource.CropSupported;
			}
		}

		// Token: 0x17000034 RID: 52
		public int Height
		{
			// Token: 0x060003A1 RID: 929 RVA: 0x0001FB29 File Offset: 0x0001DD29
			get
			{
				return this.binarizer.Height;
			}
		}

		// Token: 0x17000037 RID: 55
		public bool RotateSupported
		{
			// Token: 0x060003A6 RID: 934 RVA: 0x0001FBAC File Offset: 0x0001DDAC
			get
			{
				return this.binarizer.LuminanceSource.RotateSupported;
			}
		}

		// Token: 0x17000033 RID: 51
		public int Width
		{
			// Token: 0x060003A0 RID: 928 RVA: 0x0001FB1C File Offset: 0x0001DD1C
			get
			{
				return this.binarizer.Width;
			}
		}

		// Token: 0x04000294 RID: 660
		private Binarizer binarizer;

		// Token: 0x04000295 RID: 661
		private BitMatrix matrix;
	}
}
