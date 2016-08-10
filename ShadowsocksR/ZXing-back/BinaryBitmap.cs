using System;
using ZXing.Common;

namespace ZXing
{
	// Token: 0x02000060 RID: 96
	public sealed class BinaryBitmap
	{
		// Token: 0x06000396 RID: 918 RVA: 0x000208B7 File Offset: 0x0001EAB7
		public BinaryBitmap(Binarizer binarizer)
		{
			if (binarizer == null)
			{
				throw new ArgumentException("Binarizer must be non-null.");
			}
			this.binarizer = binarizer;
		}

		// Token: 0x0600039C RID: 924 RVA: 0x00020930 File Offset: 0x0001EB30
		public BinaryBitmap crop(int left, int top, int width, int height)
		{
			LuminanceSource source = this.binarizer.LuminanceSource.crop(left, top, width, height);
			return new BinaryBitmap(this.binarizer.createBinarizer(source));
		}

		// Token: 0x06000399 RID: 921 RVA: 0x000208EE File Offset: 0x0001EAEE
		public BitArray getBlackRow(int y, BitArray row)
		{
			return this.binarizer.getBlackRow(y, row);
		}

		// Token: 0x0600039E RID: 926 RVA: 0x00020978 File Offset: 0x0001EB78
		public BinaryBitmap rotateCounterClockwise()
		{
			LuminanceSource source = this.binarizer.LuminanceSource.rotateCounterClockwise();
			return new BinaryBitmap(this.binarizer.createBinarizer(source));
		}

		// Token: 0x0600039F RID: 927 RVA: 0x000209A8 File Offset: 0x0001EBA8
		public BinaryBitmap rotateCounterClockwise45()
		{
			LuminanceSource source = this.binarizer.LuminanceSource.rotateCounterClockwise45();
			return new BinaryBitmap(this.binarizer.createBinarizer(source));
		}

		// Token: 0x060003A0 RID: 928 RVA: 0x000209D8 File Offset: 0x0001EBD8
		public override string ToString()
		{
			BitMatrix blackMatrix = this.BlackMatrix;
			if (blackMatrix == null)
			{
				return string.Empty;
			}
			return blackMatrix.ToString();
		}

		// Token: 0x17000032 RID: 50
		public BitMatrix BlackMatrix
		{
			// Token: 0x0600039A RID: 922 RVA: 0x000208FD File Offset: 0x0001EAFD
			get
			{
				if (this.matrix == null)
				{
					this.matrix = this.binarizer.BlackMatrix;
				}
				return this.matrix;
			}
		}

		// Token: 0x17000033 RID: 51
		public bool CropSupported
		{
			// Token: 0x0600039B RID: 923 RVA: 0x0002091E File Offset: 0x0001EB1E
			get
			{
				return this.binarizer.LuminanceSource.CropSupported;
			}
		}

		// Token: 0x17000031 RID: 49
		public int Height
		{
			// Token: 0x06000398 RID: 920 RVA: 0x000208E1 File Offset: 0x0001EAE1
			get
			{
				return this.binarizer.Height;
			}
		}

		// Token: 0x17000034 RID: 52
		public bool RotateSupported
		{
			// Token: 0x0600039D RID: 925 RVA: 0x00020964 File Offset: 0x0001EB64
			get
			{
				return this.binarizer.LuminanceSource.RotateSupported;
			}
		}

		// Token: 0x17000030 RID: 48
		public int Width
		{
			// Token: 0x06000397 RID: 919 RVA: 0x000208D4 File Offset: 0x0001EAD4
			get
			{
				return this.binarizer.Width;
			}
		}

		// Token: 0x04000297 RID: 663
		private Binarizer binarizer;

		// Token: 0x04000298 RID: 664
		private BitMatrix matrix;
	}
}
