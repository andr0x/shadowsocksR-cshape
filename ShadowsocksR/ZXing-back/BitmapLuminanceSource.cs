using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ZXing
{
	// Token: 0x02000061 RID: 97
	public class BitmapLuminanceSource : BaseLuminanceSource
	{
		// Token: 0x060003A2 RID: 930 RVA: 0x00020A08 File Offset: 0x0001EC08
		public BitmapLuminanceSource(Bitmap bitmap) : base(bitmap.Width, bitmap.Height)
		{
			int height = bitmap.Height;
			int width = bitmap.Width;
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
			try
			{
				int num = Math.Abs(bitmapData.Stride);
				int num2 = num / width;
				if (num2 > 4)
				{
					for (int i = 0; i < height; i++)
					{
						int num3 = i * width;
						for (int j = 0; j < width; j++)
						{
							Color pixel = bitmap.GetPixel(j, i);
							this.luminances[num3 + j] = (byte)(19562 * (int)pixel.R + 38550 * (int)pixel.G + 7424 * (int)pixel.B >> 16);
						}
					}
				}
				else
				{
					int stride = bitmapData.Stride;
					byte[] array = new byte[num];
					IntPtr scan = bitmapData.Scan0;
					byte[] array2 = new byte[bitmap.Palette.Entries.Length];
					for (int k = 0; k < bitmap.Palette.Entries.Length; k++)
					{
						Color color = bitmap.Palette.Entries[k];
						array2[k] = (byte)(19562 * (int)color.R + 38550 * (int)color.G + 7424 * (int)color.B >> 16);
					}
					if (bitmap.PixelFormat == PixelFormat.Format32bppArgb || bitmap.PixelFormat == PixelFormat.Format32bppPArgb)
					{
						num2 = 40;
					}
					if (bitmap.PixelFormat == (PixelFormat)8207 || (bitmap.Flags & 32) == 32)
					{
						num2 = 41;
					}
					for (int l = 0; l < height; l++)
					{
						Marshal.Copy(scan, array, 0, num);
						scan = new IntPtr(scan.ToInt64() + (long)stride);
						int num4 = l * width;
						switch (num2)
						{
						case 0:
						{
							int num5 = 0;
							while (num5 * 8 < width)
							{
								int num6 = 0;
								while (num6 < 8 && 8 * num5 + num6 < width)
								{
									int num7 = array[num5] >> 7 - num6 & 1;
									this.luminances[num4 + 8 * num5 + num6] = array2[num7];
									num6++;
								}
								num5++;
							}
							break;
						}
						case 1:
							for (int m = 0; m < width; m++)
							{
								this.luminances[num4 + m] = array2[(int)array[m]];
							}
							break;
						case 2:
						{
							int num8 = 2 * width;
							for (int n = 0; n < num8; n += 2)
							{
								byte arg_280_0 = array[n];
								byte b = array[n + 1];
								int num9 = (int)(arg_280_0 & 31);
								int arg_2B1_0 = ((arg_280_0 & 224) >> 5 | (int)(b & 3) << 3) & 31;
								int num10 = (b >> 2 & 31) * 527 + 23 >> 6;
								int num11 = arg_2B1_0 * 527 + 23 >> 6;
								int num12 = num9 * 527 + 23 >> 6;
								this.luminances[num4] = (byte)(19562 * num10 + 38550 * num11 + 7424 * num12 >> 16);
								num4++;
							}
							break;
						}
						case 3:
						{
							int num13 = width * 3;
							for (int num14 = 0; num14 < num13; num14 += 3)
							{
								byte b2 = (byte)(7424 * (int)array[num14] + 38550 * (int)array[num14 + 1] + 19562 * (int)array[num14 + 2] >> 16);
								this.luminances[num4] = b2;
								num4++;
							}
							break;
						}
						case 4:
						{
							int num15 = 4 * width;
							for (int num16 = 0; num16 < num15; num16 += 4)
							{
								byte b3 = (byte)(7424 * (int)array[num16] + 38550 * (int)array[num16 + 1] + 19562 * (int)array[num16 + 2] >> 16);
								this.luminances[num4] = b3;
								num4++;
							}
							break;
						}
						default:
							if (num2 != 40)
							{
								if (num2 != 41)
								{
									throw new NotSupportedException();
								}
								int num17 = 4 * width;
								for (int num18 = 0; num18 < num17; num18 += 4)
								{
									byte b4 = (byte)(255 - (7424 * (int)array[num18] + 38550 * (int)array[num18 + 1] + 19562 * (int)array[num18 + 2] >> 16));
									this.luminances[num4] = b4;
									num4++;
								}
							}
							else
							{
								int num19 = 4 * width;
								for (int num20 = 0; num20 < num19; num20 += 4)
								{
									byte b5 = (byte)(7424 * (int)array[num20] + 38550 * (int)array[num20 + 1] + 19562 * (int)array[num20 + 2] >> 16);
									byte b6 = array[num20 + 3];
									b5 = (byte)((b5 * b6 >> 8) + (255 * (255 - b6) >> 8) + 1);
									this.luminances[num4] = b5;
									num4++;
								}
							}
							break;
						}
					}
				}
			}
			finally
			{
				bitmap.UnlockBits(bitmapData);
			}
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x000209FB File Offset: 0x0001EBFB
		protected BitmapLuminanceSource(int width, int height) : base(width, height)
		{
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x00020EE4 File Offset: 0x0001F0E4
		protected override LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height)
		{
			return new BitmapLuminanceSource(width, height)
			{
				luminances = newLuminances
			};
		}
	}
}
