using System;

namespace ZXing
{
	// Token: 0x0200005E RID: 94
	public abstract class BaseLuminanceSource : LuminanceSource
	{
		// Token: 0x06000384 RID: 900 RVA: 0x000206B2 File Offset: 0x0001E8B2
		protected BaseLuminanceSource(int width, int height) : base(width, height)
		{
			this.luminances = new byte[width * height];
		}

		// Token: 0x06000385 RID: 901 RVA: 0x000206CA File Offset: 0x0001E8CA
		protected BaseLuminanceSource(byte[] luminanceArray, int width, int height) : base(width, height)
		{
			this.luminances = new byte[width * height];
			Buffer.BlockCopy(luminanceArray, 0, this.luminances, 0, width * height);
		}

		// Token: 0x0600038E RID: 910
		protected abstract LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height);

		// Token: 0x0600038B RID: 907 RVA: 0x000207D8 File Offset: 0x0001E9D8
		public override LuminanceSource crop(int left, int top, int width, int height)
		{
			if (left + width > this.Width || top + height > this.Height)
			{
				throw new ArgumentException("Crop rectangle does not fit within image data.");
			}
			byte[] array = new byte[width * height];
			byte[] matrix = this.Matrix;
			int width2 = this.Width;
			int num = left + width;
			int num2 = top + height;
			int i = top;
			int num3 = 0;
			while (i < num2)
			{
				int j = left;
				int num4 = 0;
				while (j < num)
				{
					array[num3 * width + num4] = matrix[i * width2 + j];
					j++;
					num4++;
				}
				i++;
				num3++;
			}
			return this.CreateLuminanceSource(array, width, height);
		}

		// Token: 0x06000386 RID: 902 RVA: 0x000206F4 File Offset: 0x0001E8F4
		public override byte[] getRow(int y, byte[] row)
		{
			int width = this.Width;
			if (row == null || row.Length < width)
			{
				row = new byte[width];
			}
			for (int i = 0; i < width; i++)
			{
				row[i] = this.luminances[y * width + i];
			}
			return row;
		}

		// Token: 0x06000388 RID: 904 RVA: 0x00020740 File Offset: 0x0001E940
		public override LuminanceSource rotateCounterClockwise()
		{
			byte[] array = new byte[this.Width * this.Height];
			int height = this.Height;
			int width = this.Width;
			byte[] matrix = this.Matrix;
			for (int i = 0; i < this.Height; i++)
			{
				for (int j = 0; j < this.Width; j++)
				{
					int num = width - j - 1;
					int num2 = i;
					array[num * height + num2] = matrix[i * this.Width + j];
				}
			}
			return this.CreateLuminanceSource(array, height, width);
		}

		// Token: 0x06000389 RID: 905 RVA: 0x000207CB File Offset: 0x0001E9CB
		public override LuminanceSource rotateCounterClockwise45()
		{
			return base.rotateCounterClockwise45();
		}

		// Token: 0x1700002A RID: 42
		public override bool CropSupported
		{
			// Token: 0x0600038C RID: 908 RVA: 0x000207D3 File Offset: 0x0001E9D3
			get
			{
				return true;
			}
		}

		// Token: 0x1700002B RID: 43
		public override bool InversionSupported
		{
			// Token: 0x0600038D RID: 909 RVA: 0x000207D3 File Offset: 0x0001E9D3
			get
			{
				return true;
			}
		}

		// Token: 0x17000028 RID: 40
		public override byte[] Matrix
		{
			// Token: 0x06000387 RID: 903 RVA: 0x00020735 File Offset: 0x0001E935
			get
			{
				return this.luminances;
			}
		}

		// Token: 0x17000029 RID: 41
		public override bool RotateSupported
		{
			// Token: 0x0600038A RID: 906 RVA: 0x000207D3 File Offset: 0x0001E9D3
			get
			{
				return true;
			}
		}

		// Token: 0x04000293 RID: 659
		protected const int BChannelWeight = 7424;

		// Token: 0x04000294 RID: 660
		protected const int ChannelWeight = 16;

		// Token: 0x04000292 RID: 658
		protected const int GChannelWeight = 38550;

		// Token: 0x04000295 RID: 661
		protected byte[] luminances;

		// Token: 0x04000291 RID: 657
		protected const int RChannelWeight = 19562;
	}
}
