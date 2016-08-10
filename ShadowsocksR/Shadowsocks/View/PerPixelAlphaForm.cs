using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Shadowsocks.View
{
	// Token: 0x02000008 RID: 8
	public class PerPixelAlphaForm : Form
	{
		// Token: 0x06000068 RID: 104 RVA: 0x00007731 File Offset: 0x00005931
		public PerPixelAlphaForm()
		{
			base.FormBorderStyle = FormBorderStyle.None;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00007740 File Offset: 0x00005940
		public void SetBitmap(Bitmap bitmap)
		{
			this.SetBitmap(bitmap, 255);
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00007750 File Offset: 0x00005950
		public void SetBitmap(Bitmap bitmap, byte opacity)
		{
			if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
			{
				throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");
			}
			IntPtr dC = Win32.GetDC(IntPtr.Zero);
			IntPtr intPtr = Win32.CreateCompatibleDC(dC);
			IntPtr intPtr2 = IntPtr.Zero;
			IntPtr hObject = IntPtr.Zero;
			try
			{
				intPtr2 = bitmap.GetHbitmap(Color.FromArgb(0));
				hObject = Win32.SelectObject(intPtr, intPtr2);
				Win32.Size size = new Win32.Size(bitmap.Width, bitmap.Height);
				Win32.Point point = new Win32.Point(0, 0);
				Win32.Point point2 = new Win32.Point(base.Left, base.Top);
				Win32.BLENDFUNCTION bLENDFUNCTION = default(Win32.BLENDFUNCTION);
				bLENDFUNCTION.BlendOp = 0;
				bLENDFUNCTION.BlendFlags = 0;
				bLENDFUNCTION.SourceConstantAlpha = opacity;
				bLENDFUNCTION.AlphaFormat = 1;
				Win32.UpdateLayeredWindow(base.Handle, dC, ref point2, ref size, intPtr, ref point, 0, ref bLENDFUNCTION, 2);
			}
			finally
			{
				Win32.ReleaseDC(IntPtr.Zero, dC);
				if (intPtr2 != IntPtr.Zero)
				{
					Win32.SelectObject(intPtr, hObject);
					Win32.DeleteObject(intPtr2);
				}
				Win32.DeleteDC(intPtr);
			}
		}

		// Token: 0x17000002 RID: 2
		protected override CreateParams CreateParams
		{
			// Token: 0x0600006B RID: 107 RVA: 0x0000785C File Offset: 0x00005A5C
			get
			{
				CreateParams expr_06 = base.CreateParams;
				expr_06.ExStyle |= 524288;
				return expr_06;
			}
		}
	}
}
