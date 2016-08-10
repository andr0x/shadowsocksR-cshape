using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Shadowsocks.Controller;
using Shadowsocks.Properties;

namespace Shadowsocks.Encryption
{
	// Token: 0x02000036 RID: 54
	public class PolarSSL
	{
		// Token: 0x060001F5 RID: 501 RVA: 0x00014670 File Offset: 0x00012870
		static PolarSSL()
		{
			string text = Path.Combine(Application.StartupPath, "temp");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			string text2 = text + "/libsscrypto.dll";
			try
			{
				FileManager.UncompressFile(text2, Resources.libsscrypto_dll);
				PolarSSL.LoadLibrary(text2);
			}
			catch (IOException)
			{
			}
			catch (Exception arg_42_0)
			{
				Console.WriteLine(arg_42_0.ToString());
			}
		}

		// Token: 0x060001FA RID: 506
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern int aes_crypt_cfb128(IntPtr ctx, int mode, int length, ref int iv_off, byte[] iv, byte[] input, byte[] output);

		// Token: 0x060001F8 RID: 504
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void aes_free(IntPtr ctx);

		// Token: 0x060001F7 RID: 503
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void aes_init(IntPtr ctx);

		// Token: 0x060001F9 RID: 505
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern int aes_setkey_enc(IntPtr ctx, byte[] key, int keysize);

		// Token: 0x060001FE RID: 510
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern int arc4_crypt(IntPtr ctx, int length, byte[] input, byte[] output);

		// Token: 0x060001FC RID: 508
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void arc4_free(IntPtr ctx);

		// Token: 0x060001FB RID: 507
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void arc4_init(IntPtr ctx);

		// Token: 0x060001FD RID: 509
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void arc4_setup(IntPtr ctx, byte[] key, int keysize);

		// Token: 0x060001F6 RID: 502
		[DllImport("Kernel32.dll")]
		private static extern IntPtr LoadLibrary(string path);

		// Token: 0x04000190 RID: 400
		public const int AES_CTX_SIZE = 280;

		// Token: 0x04000192 RID: 402
		public const int AES_DECRYPT = 0;

		// Token: 0x04000191 RID: 401
		public const int AES_ENCRYPT = 1;

		// Token: 0x04000193 RID: 403
		public const int ARC4_CTX_SIZE = 264;

		// Token: 0x0400018F RID: 399
		private const string DLLNAME = "libsscrypto";
	}
}
