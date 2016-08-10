using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Shadowsocks.Controller;
using Shadowsocks.Properties;

namespace Shadowsocks.Encryption
{
	// Token: 0x02000038 RID: 56
	public class Sodium
	{
		// Token: 0x06000209 RID: 521 RVA: 0x00014A9C File Offset: 0x00012C9C
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

		// Token: 0x0600020D RID: 525
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern int crypto_stream_chacha20_ietf_xor_ic(byte[] c, byte[] m, ulong mlen, byte[] n, uint ic, byte[] k);

		// Token: 0x0600020C RID: 524
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void crypto_stream_chacha20_xor_ic(byte[] c, byte[] m, ulong mlen, byte[] n, ulong ic, byte[] k);

		// Token: 0x0600020B RID: 523
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void crypto_stream_salsa20_xor_ic(byte[] c, byte[] m, ulong mlen, byte[] n, ulong ic, byte[] k);

		// Token: 0x0600020A RID: 522
		[DllImport("Kernel32.dll")]
		private static extern IntPtr LoadLibrary(string path);

		// Token: 0x0400019A RID: 410
		private const string DLLNAME = "libsscrypto";
	}
}
