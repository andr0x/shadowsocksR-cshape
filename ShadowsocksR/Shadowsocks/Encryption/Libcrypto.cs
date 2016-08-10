using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Shadowsocks.Encryption
{
	// Token: 0x02000033 RID: 51
	internal class Libcrypto
	{
		// Token: 0x060001C7 RID: 455 RVA: 0x00013DBC File Offset: 0x00011FBC
		static Libcrypto()
		{
			try
			{
				try
				{
					Libcrypto.dlopen("libcrypto.so", 2);
					return;
				}
				catch (Exception)
				{
				}
				try
				{
					Libcrypto.LoadLibrary("libcrypto.dll");
					return;
				}
				catch (Exception)
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
				catch (Exception)
				{
				}
			}
			finally
			{
				if (Libcrypto.encrypt_func_map == null && Libcrypto.isSupport())
				{
					Dictionary<string, Libcrypto.EncryptFunc> expr_84 = new Dictionary<string, Libcrypto.EncryptFunc>();
					expr_84["rc4"] = new Libcrypto.EncryptFunc(Libcrypto.EVP_rc4);
					expr_84["aes-128-cfb"] = new Libcrypto.EncryptFunc(Libcrypto.EVP_aes_128_cfb);
					expr_84["aes-192-cfb"] = new Libcrypto.EncryptFunc(Libcrypto.EVP_aes_192_cfb);
					expr_84["aes-256-cfb"] = new Libcrypto.EncryptFunc(Libcrypto.EVP_aes_256_cfb);
					expr_84["aes-128-ofb"] = new Libcrypto.EncryptFunc(Libcrypto.EVP_aes_128_ofb);
					expr_84["aes-192-ofb"] = new Libcrypto.EncryptFunc(Libcrypto.EVP_aes_192_ofb);
					expr_84["aes-256-ofb"] = new Libcrypto.EncryptFunc(Libcrypto.EVP_aes_256_ofb);
					expr_84["bf-cfb"] = new Libcrypto.EncryptFunc(Libcrypto.EVP_bf_cfb);
					expr_84["cast5-cfb"] = new Libcrypto.EncryptFunc(Libcrypto.EVP_cast5_cfb);
					Libcrypto.encrypt_func_map = expr_84;
					Libcrypto.OpenSSL_add_all_ciphers();
				}
			}
		}

		// Token: 0x060001CC RID: 460 RVA: 0x000140B3 File Offset: 0x000122B3
		public static void clean(IntPtr ctx)
		{
			Libcrypto.EVP_CIPHER_CTX_cleanup(ctx);
			Libcrypto.EVP_CIPHER_CTX_free(ctx);
		}

		// Token: 0x060001CE RID: 462
		[DllImport("libdl.so")]
		private static extern IntPtr dlopen(string fileName, int flags);

		// Token: 0x060001D0 RID: 464
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_add_cipher(byte[] cipher_name);

		// Token: 0x060001D4 RID: 468
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_aes_128_cfb();

		// Token: 0x060001D7 RID: 471
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_aes_128_ofb();

		// Token: 0x060001D3 RID: 467
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_aes_192_cfb();

		// Token: 0x060001D6 RID: 470
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_aes_192_ofb();

		// Token: 0x060001D2 RID: 466
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_aes_256_cfb();

		// Token: 0x060001D5 RID: 469
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_aes_256_ofb();

		// Token: 0x060001D9 RID: 473
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_bf_cfb();

		// Token: 0x060001DA RID: 474
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_cast5_cfb();

		// Token: 0x060001DE RID: 478
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern int EVP_CipherInit_ex(IntPtr ctx, IntPtr cipher, IntPtr _, byte[] key, byte[] iv, int op);

		// Token: 0x060001DF RID: 479
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern void EVP_CipherUpdate(IntPtr ctx, byte[] output, ref int output_size, byte[] data, int len);

		// Token: 0x060001DC RID: 476
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern void EVP_CIPHER_CTX_cleanup(IntPtr ctx);

		// Token: 0x060001DD RID: 477
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern void EVP_CIPHER_CTX_free(IntPtr ctx);

		// Token: 0x060001DB RID: 475
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_CIPHER_CTX_new();

		// Token: 0x060001D1 RID: 465
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_get_cipherbyname(byte[] cipher_name);

		// Token: 0x060001D8 RID: 472
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr EVP_rc4();

		// Token: 0x060001CA RID: 458 RVA: 0x00013FEC File Offset: 0x000121EC
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

		// Token: 0x060001C8 RID: 456 RVA: 0x00013F68 File Offset: 0x00012168
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

		// Token: 0x060001C9 RID: 457 RVA: 0x00013F98 File Offset: 0x00012198
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

		// Token: 0x060001CD RID: 461
		[DllImport("Kernel32.dll")]
		private static extern IntPtr LoadLibrary(string path);

		// Token: 0x060001CF RID: 463
		[DllImport("libeay32", CallingConvention = CallingConvention.Cdecl)]
		public static extern void OpenSSL_add_all_ciphers();

		// Token: 0x060001CB RID: 459 RVA: 0x00014098 File Offset: 0x00012298
		public static int update(IntPtr ctx, byte[] data, int length, byte[] outbuf)
		{
			int result = 0;
			Libcrypto.EVP_CipherUpdate(ctx, outbuf, ref result, data, length);
			return result;
		}

		// Token: 0x04000183 RID: 387
		private const string DLLNAME = "libeay32";

		// Token: 0x04000184 RID: 388
		private static Dictionary<string, Libcrypto.EncryptFunc> encrypt_func_map;

		// Token: 0x020000A5 RID: 165
		// Token: 0x06000554 RID: 1364
		private delegate IntPtr EncryptFunc();
	}
}
