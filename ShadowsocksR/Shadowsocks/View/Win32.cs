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

		// Token: 0x020000A0 RID: 160
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct ARGB
		{
			// Token: 0x04000427 RID: 1063
			public byte Blue;

			// Token: 0x04000428 RID: 1064
			public byte Green;

			// Token: 0x04000429 RID: 1065
			public byte Red;

			// Token: 0x0400042A RID: 1066
			public byte Alpha;
		}

		// Token: 0x020000A1 RID: 161
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct BLENDFUNCTION
		{
			// Token: 0x0400042B RID: 1067
			public byte BlendOp;

			// Token: 0x0400042C RID: 1068
			public byte BlendFlags;

			// Token: 0x0400042D RID: 1069
			public byte SourceConstantAlpha;

			// Token: 0x0400042E RID: 1070
			public byte AlphaFormat;
		}

		// Token: 0x0200009E RID: 158
		public struct Point
		{
			// Token: 0x06000553 RID: 1363 RVA: 0x0002AD84 File Offset: 0x00028F84
			public Point(int x, int y)
			{
				this.x = x;
				this.y = y;
			}

			// Token: 0x04000423 RID: 1059
			public int x;

			// Token: 0x04000424 RID: 1060
			public int y;
		}

		// Token: 0x0200009F RID: 159
		public struct Size
		{
			// Token: 0x06000554 RID: 1364 RVA: 0x0002AD94 File Offset: 0x00028F94
			public Size(int cx, int cy)
			{
				this.cx = cx;
				this.cy = cy;
			}

			// Token: 0x04000425 RID: 1061
			public int cx;

			// Token: 0x04000426 RID: 1062
			public int cy;
		}
	}
}
