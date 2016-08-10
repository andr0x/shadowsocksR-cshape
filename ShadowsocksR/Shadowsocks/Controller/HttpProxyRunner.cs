using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Shadowsocks.Model;
using Shadowsocks.Properties;

namespace Shadowsocks.Controller
{
	// Token: 0x02000041 RID: 65
	internal class HttpProxyRunner
	{
		// Token: 0x06000247 RID: 583 RVA: 0x00016724 File Offset: 0x00014924
		static HttpProxyRunner()
		{
			HttpProxyRunner._subPath = "temp";
			HttpProxyRunner._exeNameNoExt = "/ssr_privoxy";
			HttpProxyRunner._exeName = "/ssr_privoxy.exe";
			HttpProxyRunner.runningPath = Path.Combine(Application.StartupPath, HttpProxyRunner._subPath);
			HttpProxyRunner._exeNameNoExt = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
			HttpProxyRunner._exeName = "/" + HttpProxyRunner._exeNameNoExt + ".exe";
			if (!Directory.Exists(HttpProxyRunner.runningPath))
			{
				Directory.CreateDirectory(HttpProxyRunner.runningPath);
			}
			HttpProxyRunner.Kill();
			try
			{
				FileManager.UncompressFile(HttpProxyRunner.runningPath + HttpProxyRunner._exeName, Resources.privoxy_exe);
				FileManager.UncompressFile(HttpProxyRunner.runningPath + "/mgwz.dll", Resources.mgwz_dll);
			}
			catch (IOException arg_AA_0)
			{
				Logging.LogUsefulException(arg_AA_0);
			}
		}

		// Token: 0x0600024F RID: 591
		[DllImport("user32.dll")]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		// Token: 0x06000250 RID: 592
		[DllImport("user32.dll")]
		public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

		// Token: 0x06000251 RID: 593
		[DllImport("user32.dll")]
		public static extern bool GetClientRect(IntPtr hWnd, out HttpProxyRunner.RECT lpRect);

		// Token: 0x0600024E RID: 590 RVA: 0x00016B0C File Offset: 0x00014D0C
		private int GetFreePort()
		{
			int num = 60000;
			try
			{
				IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
				List<int> list = new List<int>();
				IPEndPoint[] activeTcpListeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
				for (int i = 0; i < activeTcpListeners.Length; i++)
				{
					IPEndPoint iPEndPoint = activeTcpListeners[i];
					list.Add(iPEndPoint.Port);
				}
				for (int j = 0; j < 1000; j++)
				{
					int num2 = new Random().Next(10000, 65536);
					if (!list.Contains(num2))
					{
						int i = num2;
						return i;
					}
				}
			}
			catch (Exception arg_7D_0)
			{
				Logging.LogUsefulException(arg_7D_0);
				int i = num;
				return i;
			}
			throw new Exception("No free port found.");
		}

		// Token: 0x06000249 RID: 585 RVA: 0x000167FC File Offset: 0x000149FC
		public bool HasExited()
		{
			return this._process == null || this._process.HasExited;
		}

		// Token: 0x0600024A RID: 586 RVA: 0x00016814 File Offset: 0x00014A14
		public static void Kill()
		{
			Process[] processesByName = Process.GetProcessesByName(HttpProxyRunner._exeNameNoExt);
			int i = 0;
			while (i < processesByName.Length)
			{
				Process process = processesByName[i];
				string fileName;
				try
				{
					fileName = process.MainModule.FileName;
				}
				catch (Exception)
				{
					goto IL_5A;
				}
				goto IL_24;
				IL_5A:
				i++;
				continue;
				IL_24:
				if (fileName == Path.GetFullPath(HttpProxyRunner.runningPath + HttpProxyRunner._exeName))
				{
					try
					{
						process.Kill();
						process.WaitForExit();
					}
					catch (Exception arg_4E_0)
					{
						Console.WriteLine(arg_4E_0.ToString());
					}
					goto IL_5A;
				}
				goto IL_5A;
			}
		}

		// Token: 0x06000253 RID: 595 RVA: 0x00016BBC File Offset: 0x00014DBC
		public void RefreshTrayArea()
		{
			IntPtr hwndParent = HttpProxyRunner.FindWindowEx(HttpProxyRunner.FindWindowEx(HttpProxyRunner.FindWindow("Shell_TrayWnd", null), IntPtr.Zero, "TrayNotifyWnd", null), IntPtr.Zero, "SysPager", null);
			IntPtr intPtr = HttpProxyRunner.FindWindowEx(hwndParent, IntPtr.Zero, "ToolbarWindow32", "Notification Area");
			if (intPtr == IntPtr.Zero)
			{
				intPtr = HttpProxyRunner.FindWindowEx(hwndParent, IntPtr.Zero, "ToolbarWindow32", "User Promoted Notification Area");
				HttpProxyRunner.RefreshTrayArea(HttpProxyRunner.FindWindowEx(HttpProxyRunner.FindWindow("NotifyIconOverflowWindow", null), IntPtr.Zero, "ToolbarWindow32", "Overflow Notification Area"));
			}
			HttpProxyRunner.RefreshTrayArea(intPtr);
		}

		// Token: 0x06000254 RID: 596 RVA: 0x00016C58 File Offset: 0x00014E58
		private static void RefreshTrayArea(IntPtr windowHandle)
		{
			HttpProxyRunner.RECT rECT;
			HttpProxyRunner.GetClientRect(windowHandle, out rECT);
			for (int i = 0; i < rECT.right; i += 5)
			{
				for (int j = 0; j < rECT.bottom; j += 5)
				{
					HttpProxyRunner.SendMessage(windowHandle, 512u, 0, (j << 16) + i);
				}
			}
		}

		// Token: 0x0600024C RID: 588 RVA: 0x000169D8 File Offset: 0x00014BD8
		public void Restart()
		{
			this._process = new Process();
			this._process.StartInfo.FileName = HttpProxyRunner.runningPath + HttpProxyRunner._exeName;
			this._process.StartInfo.Arguments = " \"" + HttpProxyRunner.runningPath + "/privoxy.conf\"";
			this._process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			this._process.StartInfo.UseShellExecute = true;
			this._process.StartInfo.CreateNoWindow = true;
			this._process.StartInfo.WorkingDirectory = Application.StartupPath;
			try
			{
				this._process.Start();
			}
			catch (Exception arg_A4_0)
			{
				Console.WriteLine(arg_A4_0.ToString());
			}
		}

		// Token: 0x06000252 RID: 594
		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

		// Token: 0x0600024B RID: 587 RVA: 0x000168A4 File Offset: 0x00014AA4
		public void Start(Configuration configuration)
		{
			configuration.GetCurrentServer(null, false, false);
			if (this._process == null)
			{
				HttpProxyRunner.Kill();
				string text = Resources.privoxy_conf;
				bool arg_D3_0 = configuration.bypassWhiteList;
				this._runningPort = this.GetFreePort();
				text = text.Replace("__SOCKS_PORT__", configuration.localPort.ToString());
				text = text.Replace("__POLIPO_BIND_PORT__", this._runningPort.ToString());
				text = text.Replace("__KEEP_ALIVE_TIMEOUT__", (configuration.TTL * 2).ToString());
				text = text.Replace("__POLIPO_BIND_IP__", "127.0.0.1");
				text = text.Replace("__BYPASS_ACTION__", "actionsfile " + HttpProxyRunner._subPath + "/bypass.action");
				FileManager.ByteArrayToFile(HttpProxyRunner.runningPath + "/privoxy.conf", Encoding.UTF8.GetBytes(text));
				string text2 = "{+forward-override{forward .}}\n0.*.*.*/\n10.*.*.*/\n127.*.*.*/\n192.168.*.*/\n172.1[6-9].*.*/\n172.2[0-9].*.*/\n172.3[0-1].*.*/\n169.254.*.*/\n::1/\nfc00::/\nfe80::/\nlocalhost/\n";
				if (arg_D3_0)
				{
					string path = Path.Combine(Application.StartupPath, PACServer.BYPASS_FILE);
					if (File.Exists(path))
					{
						text2 += File.ReadAllText(path, Encoding.UTF8);
					}
				}
				FileManager.ByteArrayToFile(HttpProxyRunner.runningPath + "/bypass.action", Encoding.UTF8.GetBytes(text2));
				this.Restart();
			}
		}

		// Token: 0x0600024D RID: 589 RVA: 0x00016AA8 File Offset: 0x00014CA8
		public void Stop()
		{
			if (this._process != null)
			{
				try
				{
					this._process.Kill();
					this._process.WaitForExit();
				}
				catch (Exception arg_20_0)
				{
					Console.WriteLine(arg_20_0.ToString());
				}
				finally
				{
					this._process = null;
				}
			}
			this.RefreshTrayArea();
		}

		// Token: 0x1700001B RID: 27
		public int RunningPort
		{
			// Token: 0x06000248 RID: 584 RVA: 0x000167F4 File Offset: 0x000149F4
			get
			{
				return this._runningPort;
			}
		}

		// Token: 0x040001C5 RID: 453
		private static string runningPath;

		// Token: 0x040001C9 RID: 457
		private static string _exeName;

		// Token: 0x040001C8 RID: 456
		private static string _exeNameNoExt;

		// Token: 0x040001C4 RID: 452
		private Process _process;

		// Token: 0x040001C6 RID: 454
		private int _runningPort;

		// Token: 0x040001C7 RID: 455
		private static string _subPath;

		// Token: 0x020000A8 RID: 168
		public struct RECT
		{
			// Token: 0x0400044C RID: 1100
			public int left;

			// Token: 0x0400044D RID: 1101
			public int top;

			// Token: 0x0400044E RID: 1102
			public int right;

			// Token: 0x0400044F RID: 1103
			public int bottom;
		}
	}
}
