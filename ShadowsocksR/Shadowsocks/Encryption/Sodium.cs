using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Shadowsocks.Controller;
using Shadowsocks.Properties;

namespace Shadowsocks.Encryption
{
	// Token: 0x02000036 RID: 54
	public class Sodium
	{
		// Token: 0x060001F4 RID: 500 RVA: 0x0001420C File Offset: 0x0001240C
		static Sodium()
		{
			string text = Path.Combine(Application.StartupPath, "temp") + "/libsscrypto.dll";
			try
			{
				FileManager.UncompressFile(text, Resources.libsscrypto_dll);
				Sodium.LoadLibrary(text);
			}
			catch (IOException)
			{
			}
			catch (Exception arg_31_0)
			{
				Console.WriteLine(arg_31_0.ToString());
			}
		}

		// Token: 0x060001F8 RID: 504
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern int crypto_stream_chacha20_ietf_xor_ic(byte[] c, byte[] m, ulong mlen, byte[] n, uint ic, byte[] k);

		// Token: 0x060001F7 RID: 503
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void crypto_stream_chacha20_xor_ic(byte[] c, byte[] m, ulong mlen, byte[] n, ulong ic, byte[] k);

		// Token: 0x060001F6 RID: 502
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void crypto_stream_salsa20_xor_ic(byte[] c, byte[] m, ulong mlen, byte[] n, ulong ic, byte[] k);

		// Token: 0x060001F5 RID: 501
		[DllImport("Kernel32.dll")]
		private static extern IntPtr LoadLibrary(string path);

		// Token: 0x04000190 RID: 400
		private const string DLLNAME = "libsscrypto";
	}
}
