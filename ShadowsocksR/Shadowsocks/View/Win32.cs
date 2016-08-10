using System;
using System.Runtime.InteropServices;

namespace Shadowsocks.View
{
	// Token: 0x02000007 RID: 7
	internal class Win32
	{
		// Token: 0x06000063 RID: 99
		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

		// Token: 0x06000064 RID: 100
		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern int DeleteDC(IntPtr hdc);

		// Token: 0x06000066 RID: 102
		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern int DeleteObject(IntPtr hObject);

		// Token: 0x06000061 RID: 97
		[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr GetDC(IntPtr hWnd);

		// Token: 0x06000062 RID: 98
		[DllImport("user32.dll", ExactSpelling = true)]
		public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

		// Token: 0x06000065 RID: 101
		[DllImport("gdi32.dll", ExactSpelling = true)]
		public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

		// Token: 0x06000060 RID: 96
		[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern int UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Win32.Point pptDst, ref Win32.Size psize, IntPtr hdcSrc, ref Win32.Point pprSrc, int crKey, ref Win32.BLENDFUNCTION pblend, int dwFlags);

		// Token: 0x04000065 RID: 101
		public const byte AC_SRC_ALPHA = 1;

		// Token: 0x04000064 RID: 100
		public const byte AC_SRC_OVER = 0;

		// Token: 0x04000062 RID: 98
		public const int ULW_ALPHA = 2;

		// Token: 0x04000061 RID: 97
		public const int ULW_COLORKEY = 1;

		// Token: 0x04000063 RID: 99
		public const int ULW_OPAQUE = 4;

		// Token: 0x0200009E RID: 158
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct ARGB
		{
			// Token: 0x0400042A RID: 1066
			public byte Blue;

			// Token: 0x0400042B RID: 1067
			public byte Green;

			// Token: 0x0400042C RID: 1068
			public byte Red;

			// Token: 0x0400042D RID: 1069
			public byte Alpha;
		}

		// Token: 0x0200009F RID: 159
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct BLENDFUNCTION
		{
			// Token: 0x0400042E RID: 1070
			public byte BlendOp;

			// Token: 0x0400042F RID: 1071
			public byte BlendFlags;

			// Token: 0x04000430 RID: 1072
			public byte SourceConstantAlpha;

			// Token: 0x04000431 RID: 1073
			public byte AlphaFormat;
		}

		// Token: 0x0200009C RID: 156
		public struct Point
		{
			// Token: 0x0600054A RID: 1354 RVA: 0x0002BB3C File Offset: 0x00029D3C
			public Point(int x, int y)
			{
				this.x = x;
				this.y = y;
			}

			// Token: 0x04000426 RID: 1062
			public int x;

			// Token: 0x04000427 RID: 1063
			public int y;
		}

		// Token: 0x0200009D RID: 157
		public struct Size
		{
			// Token: 0x0600054B RID: 1355 RVA: 0x0002BB4C File Offset: 0x00029D4C
			public Size(int cx, int cy)
			{
				this.cx = cx;
				this.cy = cy;
			}

			// Token: 0x04000428 RID: 1064
			public int cx;

			// Token: 0x04000429 RID: 1065
			public int cy;
		}
	}
}
