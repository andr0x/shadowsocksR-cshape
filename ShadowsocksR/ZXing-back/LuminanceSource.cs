using System;
using System.Text;

namespace ZXing
{
	// Token: 0x02000064 RID: 100
	public abstract class LuminanceSource
	{
		// Token: 0x060003A4 RID: 932 RVA: 0x00020EF4 File Offset: 0x0001F0F4
		protected LuminanceSource(int width, int height)
		{
			this.width = width;
			this.height = height;
		}

		// Token: 0x060003AC RID: 940 RVA: 0x00020F2C File Offset: 0x0001F12C
		public virtual LuminanceSource crop(int left, int top, int width, int height)
		{
			throw new NotSupportedException("This luminance source does not support cropping.");
		}

		// Token: 0x060003A5 RID: 933
		public abstract byte[] getRow(int y, byte[] row);

		// Token: 0x060003AE RID: 942 RVA: 0x00020F38 File Offset: 0x0001F138
		public virtual LuminanceSource rotateCounterClockwise()
		{
			throw new NotSupportedException("This luminance source does not support rotation.");
		}

		// Token: 0x060003AF RID: 943 RVA: 0x00020F44 File Offset: 0x0001F144
		public virtual LuminanceSource rotateCounterClockwise45()
		{
			throw new NotSupportedException("This luminance source does not support rotation by 45 degrees.");
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x00020F50 File Offset: 0x0001F150
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

		// Token: 0x17000038 RID: 56
		public virtual bool CropSupported
		{
			// Token: 0x060003AB RID: 939 RVA: 0x0001F009 File Offset: 0x0001D209
			get
			{
				return false;
			}
		}

		// Token: 0x17000037 RID: 55
		public virtual int Height
		{
			// Token: 0x060003A9 RID: 937 RVA: 0x00020F1B File Offset: 0x0001F11B
			get
			{
				return this.height;
			}
			// Token: 0x060003AA RID: 938 RVA: 0x00020F23 File Offset: 0x0001F123
			protected set
			{
				this.height = value;
			}
		}

		// Token: 0x1700003A RID: 58
		public virtual bool InversionSupported
		{
			// Token: 0x060003B0 RID: 944 RVA: 0x0001F009 File Offset: 0x0001D209
			get
			{
				return false;
			}
		}

		// Token: 0x17000035 RID: 53
		public abstract byte[] Matrix
		{
			// Token: 0x060003A6 RID: 934
			get;
		}

		// Token: 0x17000039 RID: 57
		public virtual bool RotateSupported
		{
			// Token: 0x060003AD RID: 941 RVA: 0x0001F009 File Offset: 0x0001D209
			get
			{
				return false;
			}
		}

		// Token: 0x17000036 RID: 54
		public virtual int Width
		{
			// Token: 0x060003A7 RID: 935 RVA: 0x00020F0A File Offset: 0x0001F10A
			get
			{
				return this.width;
			}
			// Token: 0x060003A8 RID: 936 RVA: 0x00020F12 File Offset: 0x0001F112
			protected set
			{
				this.width = value;
			}
		}

		// Token: 0x040002BB RID: 699
		private int height;

		// Token: 0x040002BA RID: 698
		private int width;
	}
}
