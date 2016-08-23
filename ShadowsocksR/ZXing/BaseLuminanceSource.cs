using System;

namespace ZXing
{
	// Token: 0x02000060 RID: 96
	public abstract class BaseLuminanceSource : LuminanceSource
	{
		// Token: 0x0600038D RID: 909 RVA: 0x0001F8FA File Offset: 0x0001DAFA
		protected BaseLuminanceSource(int width, int height) : base(width, height)
		{
			this.luminances = new byte[width * height];
		}

		// Token: 0x0600038E RID: 910 RVA: 0x0001F912 File Offset: 0x0001DB12
		protected BaseLuminanceSource(byte[] luminanceArray, int width, int height) : base(width, height)
		{
			this.luminances = new byte[width * height];
			Buffer.BlockCopy(luminanceArray, 0, this.luminances, 0, width * height);
		}

		// Token: 0x06000397 RID: 919
		protected abstract LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height);

		// Token: 0x06000394 RID: 916 RVA: 0x0001FA20 File Offset: 0x0001DC20
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

		// Token: 0x0600038F RID: 911 RVA: 0x0001F93C File Offset: 0x0001DB3C
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

		// Token: 0x06000391 RID: 913 RVA: 0x0001F988 File Offset: 0x0001DB88
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

		// Token: 0x06000392 RID: 914 RVA: 0x0001FA13 File Offset: 0x0001DC13
		public override LuminanceSource rotateCounterClockwise45()
		{
			return base.rotateCounterClockwise45();
		}

		// Token: 0x1700002D RID: 45
		public override bool CropSupported
		{
			// Token: 0x06000395 RID: 917 RVA: 0x0001FA1B File Offset: 0x0001DC1B
			get
			{
				return true;
			}
		}

		// Token: 0x1700002E RID: 46
		public override bool InversionSupported
		{
			// Token: 0x06000396 RID: 918 RVA: 0x0001FA1B File Offset: 0x0001DC1B
			get
			{
				return true;
			}
		}

		// Token: 0x1700002B RID: 43
		public override byte[] Matrix
		{
			// Token: 0x06000390 RID: 912 RVA: 0x0001F97D File Offset: 0x0001DB7D
			get
			{
				return this.luminances;
			}
		}

		// Token: 0x1700002C RID: 44
		public override bool RotateSupported
		{
			// Token: 0x06000393 RID: 915 RVA: 0x0001FA1B File Offset: 0x0001DC1B
			get
			{
				return true;
			}
		}

		// Token: 0x04000290 RID: 656
		protected const int BChannelWeight = 7424;

		// Token: 0x04000291 RID: 657
		protected const int ChannelWeight = 16;

		// Token: 0x0400028F RID: 655
		protected const int GChannelWeight = 38550;

		// Token: 0x04000292 RID: 658
		protected byte[] luminances;

		// Token: 0x0400028E RID: 654
		protected const int RChannelWeight = 19562;
	}
}
