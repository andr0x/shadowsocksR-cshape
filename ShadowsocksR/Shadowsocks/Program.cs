using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Shadowsocks.Controller;
using Shadowsocks.Util;
using Shadowsocks.View;

namespace Shadowsocks
{
	// Token: 0x02000002 RID: 2
	internal static class Program
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		[STAThread]
		private static void Main(string[] args)
		{
			using (Mutex mutex = new Mutex(false, "Global\\ShadowsocksR_" + Application.StartupPath.GetHashCode()))
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				if (!mutex.WaitOne(0, false))
				{
					MessageBox.Show(I18N.GetString("Find Shadowsocks icon in your notify tray.") + "\n" + I18N.GetString("If you want to start multiple Shadowsocks, make a copy in another directory."), I18N.GetString("ShadowsocksR is already running."));
				}
				else
				{
					Directory.SetCurrentDirectory(Application.StartupPath);
					Logging.OpenLogFile();
					ShadowsocksController expr_7A = new ShadowsocksController();
					new MenuViewController(expr_7A);
					expr_7A.Start();
					Utils.ReleaseMemory();
					Application.Run();
				}
			}
		}
	}
}
