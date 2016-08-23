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
	// Token: 0x0200003F RID: 63
	internal class HttpProxyRunner
	{
		// Token: 0x06000232 RID: 562 RVA: 0x00015E94 File Offset: 0x00014094
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

		// Token: 0x0600023A RID: 570
		[DllImport("user32.dll")]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		// Token: 0x0600023B RID: 571
		[DllImport("user32.dll")]
		public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

		// Token: 0x0600023C RID: 572
		[DllImport("user32.dll")]
		public static extern bool GetClientRect(IntPtr hWnd, out HttpProxyRunner.RECT lpRect);

		// Token: 0x06000239 RID: 569 RVA: 0x00016280 File Offset: 0x00014480
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

		// Token: 0x06000234 RID: 564 RVA: 0x00015F6C File Offset: 0x0001416C
		public bool HasExited()
		{
			return this._process == null || this._process.HasExited;
		}

		// Token: 0x06000235 RID: 565 RVA: 0x00015F84 File Offset: 0x00014184
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

		// Token: 0x0600023E RID: 574 RVA: 0x00016330 File Offset: 0x00014530
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

		// Token: 0x0600023F RID: 575 RVA: 0x000163CC File Offset: 0x000145CC
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

		// Token: 0x06000237 RID: 567 RVA: 0x0001614C File Offset: 0x0001434C
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

		// Token: 0x0600023D RID: 573
		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

		// Token: 0x06000236 RID: 566 RVA: 0x00016014 File Offset: 0x00014214
		public void Start(Configuration configuration)
		{
			configuration.GetCurrentServer(null, false, false);
			if (this._process == null)
			{
				HttpProxyRunner.Kill();
				string text = Resources.privoxy_conf;
				bool arg_D9_0 = configuration.bypassWhiteList;
				this._runningPort = this.GetFreePort();
				text = text.Replace("__SOCKS_PORT__", configuration.localPort.ToString());
				text = text.Replace("__PRIVOXY_BIND_PORT__", this._runningPort.ToString());
				text = text.Replace("__KEEP_ALIVE_TIMEOUT__", "3600");
				text = text.Replace("__CONNECTION_SHARING__", "1");
				text = text.Replace("__PRIVOXY_BIND_IP__", "127.0.0.1");
				text = text.Replace("__BYPASS_ACTION__", "actionsfile " + HttpProxyRunner._subPath + "/bypass.action");
				FileManager.ByteArrayToFile(HttpProxyRunner.runningPath + "/privoxy.conf", Encoding.UTF8.GetBytes(text));
				string text2 = "{+forward-override{forward .}}\n0.*.*.*/\n10.*.*.*/\n127.*.*.*/\n192.168.*.*/\n172.1[6-9].*.*/\n172.2[0-9].*.*/\n172.3[0-1].*.*/\n169.254.*.*/\n::1/\nfc00::/\nfe80::/\nlocalhost/\n";
				if (arg_D9_0)
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

		// Token: 0x06000238 RID: 568 RVA: 0x0001621C File Offset: 0x0001441C
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
			// Token: 0x06000233 RID: 563 RVA: 0x00015F64 File Offset: 0x00014164
			get
			{
				return this._runningPort;
			}
		}

		// Token: 0x040001BB RID: 443
		private static string runningPath;

		// Token: 0x040001BF RID: 447
		private static string _exeName;

		// Token: 0x040001BE RID: 446
		private static string _exeNameNoExt;

		// Token: 0x040001BA RID: 442
		private Process _process;

		// Token: 0x040001BC RID: 444
		private int _runningPort;

		// Token: 0x040001BD RID: 445
		private static string _subPath;

		// Token: 0x020000AA RID: 170
		public struct RECT
		{
			// Token: 0x04000449 RID: 1097
			public int left;

			// Token: 0x0400044A RID: 1098
			public int top;

			// Token: 0x0400044B RID: 1099
			public int right;

			// Token: 0x0400044C RID: 1100
			public int bottom;
		}
	}
}
