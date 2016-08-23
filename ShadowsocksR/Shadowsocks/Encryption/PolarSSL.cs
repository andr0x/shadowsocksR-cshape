using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Shadowsocks.Controller;
using Shadowsocks.Properties;

namespace Shadowsocks.Encryption
{
	// Token: 0x02000034 RID: 52
	public class PolarSSL
	{
		// Token: 0x060001E0 RID: 480 RVA: 0x00013DE0 File Offset: 0x00011FE0
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

		// Token: 0x060001E5 RID: 485
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern int aes_crypt_cfb128(IntPtr ctx, int mode, int length, ref int iv_off, byte[] iv, byte[] input, byte[] output);

		// Token: 0x060001E3 RID: 483
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void aes_free(IntPtr ctx);

		// Token: 0x060001E2 RID: 482
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void aes_init(IntPtr ctx);

		// Token: 0x060001E4 RID: 484
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern int aes_setkey_enc(IntPtr ctx, byte[] key, int keysize);

		// Token: 0x060001E9 RID: 489
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern int arc4_crypt(IntPtr ctx, int length, byte[] input, byte[] output);

		// Token: 0x060001E7 RID: 487
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void arc4_free(IntPtr ctx);

		// Token: 0x060001E6 RID: 486
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void arc4_init(IntPtr ctx);

		// Token: 0x060001E8 RID: 488
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void arc4_setup(IntPtr ctx, byte[] key, int keysize);

		// Token: 0x060001E1 RID: 481
		[DllImport("Kernel32.dll")]
		private static extern IntPtr LoadLibrary(string path);

		// Token: 0x04000186 RID: 390
		public const int AES_CTX_SIZE = 280;

		// Token: 0x04000188 RID: 392
		public const int AES_DECRYPT = 0;

		// Token: 0x04000187 RID: 391
		public const int AES_ENCRYPT = 1;

		// Token: 0x04000189 RID: 393
		public const int ARC4_CTX_SIZE = 264;

		// Token: 0x04000185 RID: 389
		private const string DLLNAME = "libsscrypto";
	}
}
