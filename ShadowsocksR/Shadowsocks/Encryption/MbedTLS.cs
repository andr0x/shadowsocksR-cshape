using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Shadowsocks.Controller;
using Shadowsocks.Properties;

namespace Shadowsocks.Encryption
{
	// Token: 0x02000033 RID: 51
	public class MbedTLS
	{
		// Token: 0x060001D7 RID: 471 RVA: 0x00013D2C File Offset: 0x00011F2C
		static MbedTLS()
		{
			Path.GetTempPath();
			string text = Path.Combine(Application.StartupPath, "temp") + "/libsscrypto.dll";
			try
			{
				FileManager.UncompressFile(text, Resources.libsscrypto_dll);
			}
			catch (IOException)
			{
			}
			catch (Exception arg_30_0)
			{
				Console.WriteLine(arg_30_0.ToString());
			}
			MbedTLS.LoadLibrary(text);
		}

		// Token: 0x060001D8 RID: 472
		[DllImport("Kernel32.dll")]
		private static extern IntPtr LoadLibrary(string path);

		// Token: 0x060001D9 RID: 473 RVA: 0x00013D98 File Offset: 0x00011F98
		public static byte[] MbedTLSMD5(byte[] input)
		{
			IntPtr arg_0F_0 = Marshal.AllocHGlobal(88);
			byte[] array = new byte[16];
			MbedTLS.md5_init(arg_0F_0);
			MbedTLS.md5_starts(arg_0F_0);
			MbedTLS.md5_update(arg_0F_0, input, (uint)input.Length);
			MbedTLS.md5_finish(arg_0F_0, array);
			MbedTLS.md5_free(arg_0F_0);
			Marshal.FreeHGlobal(arg_0F_0);
			return array;
		}

		// Token: 0x060001DE RID: 478
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void md5_finish(IntPtr ctx, byte[] output);

		// Token: 0x060001DB RID: 475
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void md5_free(IntPtr ctx);

		// Token: 0x060001DA RID: 474
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void md5_init(IntPtr ctx);

		// Token: 0x060001DC RID: 476
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void md5_starts(IntPtr ctx);

		// Token: 0x060001DD RID: 477
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void md5_update(IntPtr ctx, byte[] input, uint ilen);

		// Token: 0x04000183 RID: 387
		private const string DLLNAME = "libsscrypto";

		// Token: 0x04000184 RID: 388
		public const int MD5_CTX_SIZE = 88;
	}
}
