using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Shadowsocks.Util
{
	// Token: 0x0200000D RID: 13
	public class Utils
	{
		// Token: 0x060000AE RID: 174 RVA: 0x0000D05C File Offset: 0x0000B25C
		public static string DecodeBase64(string val)
		{
			byte[] array = null;
			string result = "";
			result = val;
			for (int i = 0; i < 3; i++)
			{
				try
				{
					array = Convert.FromBase64String(val);
				}
				catch (FormatException)
				{
					val += "=";
				}
			}
			if (array != null)
			{
				result = Encoding.UTF8.GetString(array);
			}
			return result;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x0000D0E0 File Offset: 0x0000B2E0
		public static string DecodeUrlSafeBase64(string val)
		{
			byte[] array = null;
			string result = "";
			result = val;
			for (int i = 0; i < 3; i++)
			{
				try
				{
					array = Convert.FromBase64String(val.Replace('-', '+').Replace('_', '/'));
				}
				catch (FormatException)
				{
					val += "=";
				}
			}
			if (array != null)
			{
				result = Encoding.UTF8.GetString(array);
			}
			return result;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x0000D0BC File Offset: 0x0000B2BC
		public static string EncodeUrlSafeBase64(string val)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(val)).Replace('+', '-').Replace('/', '_');
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x0000CDCC File Offset: 0x0000AFCC
		public static int FindStr(byte[] target, int targetLength, byte[] m)
		{
			if (m.Length != 0 && targetLength >= m.Length)
			{
				for (int i = 0; i <= targetLength - m.Length; i++)
				{
					if (target[i] == m[0])
					{
						int num = 1;
						while (num < m.Length && target[i + num] == m[num])
						{
							num++;
						}
						if (num >= m.Length)
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x0000CECC File Offset: 0x0000B0CC
		public static bool isLAN(Socket socket)
		{
			IPAddress address = ((IPEndPoint)socket.RemoteEndPoint).Address;
			if (((IPEndPoint)socket.RemoteEndPoint).Address.GetAddressBytes().Length == 4)
			{
				string[] array = new string[]
				{
					"0.0.0.0/8",
					"10.0.0.0/8",
					"127.0.0.0/8",
					"169.254.0.0/16",
					"172.16.0.0/12",
					"192.168.0.0/16",
					"::1/128",
					"fc00::/7",
					"fe80::/10"
				};
				for (int i = 0; i < array.Length; i++)
				{
					string netmask = array[i];
					if (Utils.isMatchSubNet(address, netmask))
					{
						return true;
					}
				}
				return false;
			}
			return true;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x0000CE6C File Offset: 0x0000B06C
		public static bool isMatchSubNet(IPAddress ip, string netmask)
		{
			string[] array = netmask.Split(new char[]
			{
				'/'
			});
			IPAddress iPAddress = IPAddress.Parse(array[0]);
			if (ip.AddressFamily == iPAddress.AddressFamily)
			{
				try
				{
					bool result = Utils.isMatchSubNet(ip, iPAddress, (int)Convert.ToInt16(array[1]));
					return result;
				}
				catch
				{
					bool result = false;
					return result;
				}
				//return false;
			}
			return false;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x0000CE1C File Offset: 0x0000B01C
		public static bool isMatchSubNet(IPAddress ip, IPAddress net, int netmask)
		{
			byte[] addressBytes = ip.GetAddressBytes();
			byte[] addressBytes2 = net.GetAddressBytes();
			int i = 8;
			int num = 0;
			while (i < netmask)
			{
				if (addressBytes[num] != addressBytes2[num])
				{
					return false;
				}
				i += 8;
				num++;
			}
			return addressBytes[num] >> i - netmask == addressBytes2[num] >> i - netmask;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x0000CDA4 File Offset: 0x0000AFA4
		public static void RandBytes(byte[] buf, int length)
		{
			byte[] array = new byte[length];
			new RNGCryptoServiceProvider().GetBytes(array);
			array.CopyTo(buf, 0);
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x0000CCE6 File Offset: 0x0000AEE6
		public static void ReleaseMemory()
		{
			GC.Collect(GC.MaxGeneration);
			GC.WaitForPendingFinalizers();
			Utils.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, (UIntPtr)4294967295u, (UIntPtr)4294967295u);
		}

		// Token: 0x060000B1 RID: 177
		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetProcessWorkingSetSize(IntPtr process, UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);

		// Token: 0x060000A7 RID: 167 RVA: 0x0000CD14 File Offset: 0x0000AF14
		public static string UnGzip(byte[] buf)
		{
			byte[] array = new byte[1024];
			string @string;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (GZipStream gZipStream = new GZipStream(new MemoryStream(buf), CompressionMode.Decompress, false))
				{
					int count;
					while ((count = gZipStream.Read(array, 0, array.Length)) > 0)
					{
						memoryStream.Write(array, 0, count);
					}
				}
				@string = Encoding.UTF8.GetString(memoryStream.ToArray());
			}
			return @string;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x0000CF74 File Offset: 0x0000B174
		public static string urlDecode(string str)
		{
			string text = "";
			for (int i = 0; i < str.Length; i++)
			{
				if (str[i] == '%' && i < str.Length - 2)
				{
					string arg_34_0 = str.Substring(i + 1, 2).ToLower();
					int num = 0;
					char c = arg_34_0[0];
					char c2 = arg_34_0[1];
					num += (int)((c < 'a') ? (c - '0') : ('\n' + (c - 'a')));
					num *= 16;
					num += (int)((c2 < 'a') ? (c2 - '0') : ('\n' + (c2 - 'a')));
					text += ((char)num).ToString();
					i += 2;
				}
				else if (str[i] == '+')
				{
					text += " ";
				}
				else
				{
					text += str[i].ToString();
				}
			}
			return text;
		}

		// Token: 0x020000A0 RID: 160
		public static class LoadResourceDll
		{
			// Token: 0x0600054C RID: 1356 RVA: 0x0002BB5C File Offset: 0x00029D5C
			private static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
			{
				string fullName = new AssemblyName(args.Name).FullName;
				Assembly assembly;
				if (Utils.LoadResourceDll.Dlls.TryGetValue(fullName, out assembly) && assembly != null)
				{
					Utils.LoadResourceDll.Dlls[fullName] = null;
					return assembly;
				}
				throw new DllNotFoundException(fullName);
			}

			// Token: 0x04000433 RID: 1075
			private static Dictionary<string, object> Assemblies = new Dictionary<string, object>();

			// Token: 0x04000432 RID: 1074
			private static Dictionary<string, Assembly> Dlls = new Dictionary<string, Assembly>();
		}
	}
}
