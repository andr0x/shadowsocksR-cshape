using System;
using System.Text;

namespace ZXing
{
	// Token: 0x02000066 RID: 102
	public abstract class LuminanceSource
	{
		// Token: 0x060003AD RID: 941 RVA: 0x0002013C File Offset: 0x0001E33C
		protected LuminanceSource(int width, int height)
		{
			this.width = width;
			this.height = height;
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x00020174 File Offset: 0x0001E374
		public virtual LuminanceSource crop(int left, int top, int width, int height)
		{
			throw new NotSupportedException("This luminance source does not support cropping.");
		}

		// Token: 0x060003AE RID: 942
		public abstract byte[] getRow(int y, byte[] row);

		// Token: 0x060003B7 RID: 951 RVA: 0x00020180 File Offset: 0x0001E380
		public virtual LuminanceSource rotateCounterClockwise()
		{
			throw new NotSupportedException("This luminance source does not support rotation.");
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x0002018C File Offset: 0x0001E38C
		public virtual LuminanceSource rotateCounterClockwise45()
		{
			throw new NotSupportedException("This luminance source does not support rotation by 45 degrees.");
		}

		// Token: 0x060003BA RID: 954 RVA: 0x00020198 File Offset: 0x0001E398
		public override string ToString()
		{
			byte[] array = new byte[this.width];
			StringBuilder stringBuilder = new StringBuilder(this.height * (this.width + 1));
			for (int i = 0; i < this.height; i++)
			{
				array = this.getRow(i, array);
				for (int j = 0; j < this.width; j++)
				{
					int num = (int)(array[j] & 255);
					char value;
					if (num < 64)
					{
						value = '#';
					}
					else if (num < 128)
					{
						value = '+';
					}
					else if (num < 192)
					{
						value = '.';
					}
					else
					{
						value = ' ';
					}
					stringBuilder.Append(value);
				}
				stringBuilder.Append('\n');
			}
			return stringBuilder.ToString();
		}

		// Token: 0x1700003B RID: 59
		public virtual bool CropSupported
		{
			// Token: 0x060003B4 RID: 948 RVA: 0x0001E251 File Offset: 0x0001C451
			get
			{
				return false;
			}
		}

		// Token: 0x1700003A RID: 58
		public virtual int Height
		{
			// Token: 0x060003B2 RID: 946 RVA: 0x00020163 File Offset: 0x0001E363
			get
			{
				return this.height;
			}
			// Token: 0x060003B3 RID: 947 RVA: 0x0002016B File Offset: 0x0001E36B
			protected set
			{
				this.height = value;
			}
		}

		// Token: 0x1700003D RID: 61
		public virtual bool InversionSupported
		{
			// Token: 0x060003B9 RID: 953 RVA: 0x0001E251 File Offset: 0x0001C451
			get
			{
				return false;
			}
		}

		// Token: 0x17000038 RID: 56
		public abstract byte[] Matrix
		{
			// Token: 0x060003AF RID: 943
			get;
		}

		// Token: 0x1700003C RID: 60
		public virtual bool RotateSupported
		{
			// Token: 0x060003B6 RID: 950 RVA: 0x0001E251 File Offset: 0x0001C451
			get
			{
				return false;
			}
		}

		// Token: 0x17000039 RID: 57
		public virtual int Width
		{
			// Token: 0x060003B0 RID: 944 RVA: 0x00020152 File Offset: 0x0001E352
			get
			{
				return this.width;
			}
			// Token: 0x060003B1 RID: 945 RVA: 0x0002015A File Offset: 0x0001E35A
			protected set
			{
				this.width = value;
			}
		}

		// Token: 0x040002B8 RID: 696
		private int height;

		// Token: 0x040002B7 RID: 695
		private int width;
	}
}
