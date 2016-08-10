using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Shadowsocks.Controller;
using Shadowsocks.Properties;

namespace Shadowsocks.Encryption
{
	// Token: 0x02000035 RID: 53
	public class MbedTLS
	{
		// Token: 0x060001EC RID: 492 RVA: 0x000145BC File Offset: 0x000127BC
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

		// Token: 0x060001ED RID: 493
		[DllImport("Kernel32.dll")]
		private static extern IntPtr LoadLibrary(string path);

		// Token: 0x060001EE RID: 494 RVA: 0x00014628 File Offset: 0x00012828
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

		// Token: 0x060001F3 RID: 499
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void md5_finish(IntPtr ctx, byte[] output);

		// Token: 0x060001F0 RID: 496
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void md5_free(IntPtr ctx);

		// Token: 0x060001EF RID: 495
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void md5_init(IntPtr ctx);

		// Token: 0x060001F1 RID: 497
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void md5_starts(IntPtr ctx);

		// Token: 0x060001F2 RID: 498
		[DllImport("libsscrypto", CallingConvention = CallingConvention.Cdecl)]
		public static extern void md5_update(IntPtr ctx, byte[] input, uint ilen);

		// Token: 0x0400018D RID: 397
		private const string DLLNAME = "libsscrypto";

		// Token: 0x0400018E RID: 398
		public const int MD5_CTX_SIZE = 88;
	}
}
