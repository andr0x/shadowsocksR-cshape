using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Shadowsocks.Encryption
{
	// Token: 0x02000031 RID: 49
	internal class Libcrypto
	{
		// Token: 0x060001B3 RID: 435 RVA: 0x0001354C File Offset: 0x0001174C
		static Libcrypto()
		{
			try
			{
				try
				{
					Libcrypto.LoadLibrary("libcrypto.dll");
					return;
				}
				catch
				{
				}
				string text = Path.Combine(Application.StartupPath, "temp");
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				string path = text + "/libcrypto.dll";
				try
				{
					Libcrypto.LoadLibrary(path);
				}
				catch (IOException)
				{
				}
				catch
				{
				}
			}
			finally
			{
				if (Libcrypto.encrypt_func_map == null && Libcrypto.isSupport())
				{
					Dictionary<string, Libcrypto.EncryptFunc> expr_6F = new Dictionary<string, Libcrypto.EncryptFunc>();
					expr_6F["rc4"] = new Libcrypto.EncryptFunc(Libcrypto.EVP_rc4);
					expr_6F["aes-128-cfb"] = new Libcrypto.EncryptFunc(Libcrypto.EVP_aes_128_cfb);
					expr_6F["aes-192-cfb"] = new Libcrypto.EncryptFunc(Libcrypto.EVP_aes_192_cfb);
					expr_6F["aes-256-cfb"] = new Libcrypto.EncryptFunc(Libcrypto.EVP_aes_256_cfb);
					expr_6F["aes-128-ofb"] = new Libcrypto.EncryptFunc(Libcrypto.EVP_aes_128_ofb);
					expr_6F["aes-192-ofb"] = new Libcrypto.EncryptFunc(Libcrypto.EVP_aes_192_ofb);
					expr_6F["aes-256-ofb"] = new Libcrypto.EncryptFunc(Libcrypto.EVP_aes_256_ofb);
					expr_6F["bf-cfb"] = new Libcrypto.EncryptFunc(Libcrypto.EVP_bf_cfb);
					expr_6F["cast5-cfb"] = new Libcrypto.EncryptFunc(Libcrypto.EVP_cast5_cfb);
					Libcrypto.encrypt_func_map = expr_6F;
					Libcrypto.OpenSSL_add_all_ciphers();
				}
			}
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x00013823 File Offset: 0x00011A23
		public static void clean(IntPtr ctx)
		{
			Libcrypto.EVP_CIPHER_CTX_cleanup(ctx);
			Libcrypto.EVP_CIPHER_CTX_free(ctx);
		}

		// Token: 0x060001BB RID: 443
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_add_cipher(byte[] cipher_name);

		// Token: 0x060001BF RID: 447
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_aes_128_cfb();

		// Token: 0x060001C2 RID: 450
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_aes_128_ofb();

		// Token: 0x060001BE RID: 446
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_aes_192_cfb();

		// Token: 0x060001C1 RID: 449
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_aes_192_ofb();

		// Token: 0x060001BD RID: 445
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_aes_256_cfb();

		// Token: 0x060001C0 RID: 448
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_aes_256_ofb();

		// Token: 0x060001C4 RID: 452
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_bf_cfb();

		// Token: 0x060001C5 RID: 453
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_cast5_cfb();

		// Token: 0x060001C9 RID: 457
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern int EVP_CipherInit_ex(IntPtr ctx, IntPtr cipher, IntPtr _, byte[] key, byte[] iv, int op);

		// Token: 0x060001CA RID: 458
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern void EVP_CipherUpdate(IntPtr ctx, byte[] output, ref int output_size, byte[] data, int len);

		// Token: 0x060001C7 RID: 455
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern void EVP_CIPHER_CTX_cleanup(IntPtr ctx);

		// Token: 0x060001C8 RID: 456
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern void EVP_CIPHER_CTX_free(IntPtr ctx);

		// Token: 0x060001C6 RID: 454
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_CIPHER_CTX_new();

		// Token: 0x060001BC RID: 444
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_get_cipherbyname(byte[] cipher_name);

		// Token: 0x060001C3 RID: 451
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_rc4();

		// Token: 0x060001B6 RID: 438 RVA: 0x0001375C File Offset: 0x0001195C
		public static IntPtr init(string cipher_name, byte[] key, byte[] iv, int op)
		{
			IntPtr intPtr = IntPtr.Zero;
			string text = cipher_name;
			if (cipher_name.StartsWith("rc4-md5"))
			{
				text = "rc4";
			}
			byte[] bytes = Encoding.ASCII.GetBytes(text);
			Array.Resize<byte>(ref bytes, bytes.Length + 1);
			IntPtr intPtr2 = Libcrypto.EVP_get_cipherbyname(bytes);
			if (intPtr2 == IntPtr.Zero && Libcrypto.encrypt_func_map != null && Libcrypto.encrypt_func_map.ContainsKey(text))
			{
				intPtr2 = Libcrypto.encrypt_func_map[text]();
			}
			if (intPtr2 != IntPtr.Zero)
			{
				intPtr = Libcrypto.EVP_CIPHER_CTX_new();
				if (Libcrypto.EVP_CipherInit_ex(intPtr, intPtr2, IntPtr.Zero, key, iv, op) == 0)
				{
					Libcrypto.clean(intPtr);
					return IntPtr.Zero;
				}
			}
			return intPtr;
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x000136D8 File Offset: 0x000118D8
		public static bool isSupport()
		{
			bool result;
			try
			{
				Libcrypto.EVP_get_cipherbyname(null);
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x00013708 File Offset: 0x00011908
		public static bool is_cipher(string cipher_name)
		{
			string s = cipher_name;
			if (cipher_name.StartsWith("rc4-md5"))
			{
				s = "rc4";
			}
			IntPtr arg_1A_0 = IntPtr.Zero;
			byte[] bytes = Encoding.ASCII.GetBytes(s);
			Array.Resize<byte>(ref bytes, bytes.Length + 1);
			return Libcrypto.EVP_get_cipherbyname(bytes) != IntPtr.Zero;
		}

		// Token: 0x060001B9 RID: 441
		[DllImport("Kernel32.dll")]
		private static extern IntPtr LoadLibrary(string path);

		// Token: 0x060001BA RID: 442
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern void OpenSSL_add_all_ciphers();

		// Token: 0x060001B7 RID: 439 RVA: 0x00013808 File Offset: 0x00011A08
		public static int update(IntPtr ctx, byte[] data, int length, byte[] outbuf)
		{
			int result = 0;
			Libcrypto.EVP_CipherUpdate(ctx, outbuf, ref result, data, length);
			return result;
		}

		// Token: 0x04000179 RID: 377
		private const string DLLNAME = "libeay32";

		// Token: 0x0400017A RID: 378
		private static Dictionary<string, Libcrypto.EncryptFunc> encrypt_func_map;

		// Token: 0x020000A7 RID: 167
		// Token: 0x0600055D RID: 1373
		private delegate IntPtr EncryptFunc();
	}
}
