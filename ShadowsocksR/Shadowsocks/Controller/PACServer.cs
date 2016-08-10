using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using Shadowsocks.Model;
using Shadowsocks.Properties;
using Shadowsocks.Util;

namespace Shadowsocks.Controller
{
	// Token: 0x0200004A RID: 74
	internal class PACServer : Listener.Service
	{
		// Token: 0x06000289 RID: 649 RVA: 0x000180F5 File Offset: 0x000162F5
		public PACServer()
		{
			this.WatchPacFile();
			this.WatchUserRuleFile();
		}

		// Token: 0x06000295 RID: 661 RVA: 0x000187BC File Offset: 0x000169BC
		private string GetPACAddress(byte[] requestBuf, int length, IPEndPoint localEndPoint, int socksType)
		{
			if (socksType == 5)
			{
				return string.Concat(new object[]
				{
					"SOCKS5 ",
					localEndPoint.Address,
					":",
					this._config.localPort,
					";"
				});
			}
			if (socksType == 4)
			{
				return string.Concat(new object[]
				{
					"SOCKS ",
					localEndPoint.Address,
					":",
					this._config.localPort,
					";"
				});
			}
			return string.Concat(new object[]
			{
				"PROXY ",
				localEndPoint.Address,
				":",
				this._config.localPort,
				";"
			});
		}

		// Token: 0x0600028E RID: 654 RVA: 0x000183C4 File Offset: 0x000165C4
		private string GetPACContent()
		{
			if (File.Exists(PACServer.PAC_FILE))
			{
				return File.ReadAllText(PACServer.PAC_FILE, Encoding.UTF8);
			}
			return Utils.UnGzip(Resources.proxy_pac_txt);
		}

		// Token: 0x0600028B RID: 651 RVA: 0x00018114 File Offset: 0x00016314
		public bool Handle(byte[] firstPacket, int length, Socket socket)
		{
			bool result;
			try
			{
				string[] arg_39_0 = Encoding.UTF8.GetString(firstPacket, 0, length).Split(new string[]
				{
					"\r\n",
					"\r",
					"\n"
				}, StringSplitOptions.RemoveEmptyEntries);
				bool flag = false;
				bool flag2 = false;
				int socksType = 0;
				string text = null;
				string[] array = arg_39_0;
				for (int i = 0; i < array.Length; i++)
				{
					string text2 = array[i];
					string[] array2 = text2.Split(new char[]
					{
						':'
					}, 2);
					if (array2.Length == 2)
					{
						if (array2[0] == "Host")
						{
							if (array2[1].Trim() == ((IPEndPoint)socket.LocalEndPoint).ToString())
							{
								flag = true;
							}
						}
						else if (array2[0] == "User-Agent")
						{
						}
					}
					else if (array2.Length == 1 && text2.IndexOf("pac") > 0 && text2.IndexOf("GET") == 0)
					{
						string expr_E7 = text2;
						string text3 = expr_E7.Substring(expr_E7.IndexOf(" ") + 1);
						text3 = text3.Substring(0, text3.IndexOf(" "));
						flag2 = true;
						int num = text3.IndexOf("port=");
						if (num > 0)
						{
							string text4 = text3.Substring(num + 5);
							if (text4.IndexOf("&") >= 0)
							{
								text4 = text4.Substring(0, text4.IndexOf("&"));
							}
							int num2 = text3.IndexOf("ip=");
							if (num2 > 0)
							{
								text = text3.Substring(num2 + 3);
								if (text.IndexOf("&") >= 0)
								{
									text = text.Substring(0, text.IndexOf("&"));
								}
								text = text + ":" + text4 + ";";
							}
							else
							{
								text = "127.0.0.1:" + text4 + ";";
							}
						}
						if (text3.IndexOf("type=socks4") > 0 || text3.IndexOf("type=s4") > 0)
						{
							socksType = 4;
						}
						if (text3.IndexOf("type=socks5") > 0 || text3.IndexOf("type=s5") > 0)
						{
							socksType = 5;
						}
					}
				}
				if (flag & flag2)
				{
					this.SendResponse(firstPacket, length, socket, socksType, text);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			catch (ArgumentException)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000290 RID: 656 RVA: 0x000185DC File Offset: 0x000167DC
		private void SendCallback(IAsyncResult ar)
		{
			Socket socket = (Socket)ar.AsyncState;
			try
			{
				socket.Shutdown(SocketShutdown.Both);
				socket.Close();
			}
			catch
			{
			}
		}

		// Token: 0x0600028F RID: 655 RVA: 0x000183EC File Offset: 0x000165EC
		public void SendResponse(byte[] firstPacket, int length, Socket socket, int socksType, string setProxy)
		{
			try
			{
				string text = this.GetPACContent();
				IPEndPoint localEndPoint = (IPEndPoint)socket.LocalEndPoint;
				string str = (setProxy == null) ? this.GetPACAddress(firstPacket, length, localEndPoint, socksType) : ((socksType == 5) ? ("SOCKS5 " + setProxy) : ((socksType == 4) ? ("SOCKS " + setProxy) : ("PROXY " + setProxy)));
				if (this._config.pacDirectGoProxy && this._config.proxyEnable)
				{
					if (this._config.proxyType == 0)
					{
						text = text.Replace("__DIRECT__", string.Concat(new string[]
						{
							"SOCKS5 ",
							this._config.proxyHost,
							":",
							this._config.proxyPort.ToString(),
							";DIRECT;"
						}));
					}
					else if (this._config.proxyType == 1)
					{
						text = text.Replace("__DIRECT__", string.Concat(new string[]
						{
							"PROXY ",
							this._config.proxyHost,
							":",
							this._config.proxyPort.ToString(),
							";DIRECT;"
						}));
					}
				}
				else
				{
					text = text.Replace("__DIRECT__", "DIRECT;");
				}
				text = text.Replace("__PROXY__", str + "DIRECT;");
				string s = string.Format("HTTP/1.1 200 OK\r\nServer: ShadowsocksR\r\nContent-Type: application/x-ns-proxy-autoconfig\r\nContent-Length: {0}\r\nConnection: Close\r\n\r\n", Encoding.UTF8.GetBytes(text).Length) + text;
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback(this.SendCallback), socket);
				Utils.ReleaseMemory();
			}
			catch (Exception arg_1B1_0)
			{
				Console.WriteLine(arg_1B1_0);
				socket.Shutdown(SocketShutdown.Both);
				socket.Close();
			}
		}

		// Token: 0x0600028C RID: 652 RVA: 0x00018374 File Offset: 0x00016574
		public string TouchPACFile()
		{
			if (File.Exists(PACServer.PAC_FILE))
			{
				return PACServer.PAC_FILE;
			}
			FileManager.UncompressFile(PACServer.PAC_FILE, Resources.proxy_pac_txt);
			return PACServer.PAC_FILE;
		}

		// Token: 0x0600028D RID: 653 RVA: 0x0001839C File Offset: 0x0001659C
		internal string TouchUserRuleFile()
		{
			if (File.Exists(PACServer.USER_RULE_FILE))
			{
				return PACServer.USER_RULE_FILE;
			}
			File.WriteAllText(PACServer.USER_RULE_FILE, Resources.user_rule);
			return PACServer.USER_RULE_FILE;
		}

		// Token: 0x0600028A RID: 650 RVA: 0x00018109 File Offset: 0x00016309
		public void UpdateConfiguration(Configuration config)
		{
			this._config = config;
		}

		// Token: 0x06000294 RID: 660 RVA: 0x000187A0 File Offset: 0x000169A0
		private void UserRuleFileWatcher_Changed(object sender, FileSystemEventArgs e)
		{
			if (this.UserRuleFileChanged != null)
			{
				this.UserRuleFileChanged(this, new EventArgs());
			}
		}

		// Token: 0x06000293 RID: 659 RVA: 0x00018785 File Offset: 0x00016985
		private void Watcher_Changed(object sender, FileSystemEventArgs e)
		{
			if (this.PACFileChanged != null)
			{
				this.PACFileChanged(this, new EventArgs());
			}
		}

		// Token: 0x06000291 RID: 657 RVA: 0x00018618 File Offset: 0x00016818
		private void WatchPacFile()
		{
			if (this.PACFileWatcher != null)
			{
				this.PACFileWatcher.Dispose();
			}
			this.PACFileWatcher = new FileSystemWatcher(Directory.GetCurrentDirectory());
			this.PACFileWatcher.NotifyFilter = (NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite);
			this.PACFileWatcher.Filter = PACServer.PAC_FILE;
			this.PACFileWatcher.Changed += new FileSystemEventHandler(this.Watcher_Changed);
			this.PACFileWatcher.Created += new FileSystemEventHandler(this.Watcher_Changed);
			this.PACFileWatcher.Deleted += new FileSystemEventHandler(this.Watcher_Changed);
			this.PACFileWatcher.Renamed += new RenamedEventHandler(this.Watcher_Changed);
			this.PACFileWatcher.EnableRaisingEvents = true;
		}

		// Token: 0x06000292 RID: 658 RVA: 0x000186D0 File Offset: 0x000168D0
		private void WatchUserRuleFile()
		{
			if (this.UserRuleFileWatcher != null)
			{
				this.UserRuleFileWatcher.Dispose();
			}
			this.UserRuleFileWatcher = new FileSystemWatcher(Directory.GetCurrentDirectory());
			this.UserRuleFileWatcher.NotifyFilter = (NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite);
			this.UserRuleFileWatcher.Filter = PACServer.USER_RULE_FILE;
			this.UserRuleFileWatcher.Changed += new FileSystemEventHandler(this.UserRuleFileWatcher_Changed);
			this.UserRuleFileWatcher.Created += new FileSystemEventHandler(this.UserRuleFileWatcher_Changed);
			this.UserRuleFileWatcher.Deleted += new FileSystemEventHandler(this.UserRuleFileWatcher_Changed);
			this.UserRuleFileWatcher.Renamed += new RenamedEventHandler(this.UserRuleFileWatcher_Changed);
			this.UserRuleFileWatcher.EnableRaisingEvents = true;
		}

		// Token: 0x14000004 RID: 4
		// Token: 0x06000285 RID: 645 RVA: 0x00018018 File Offset: 0x00016218
		// Token: 0x06000286 RID: 646 RVA: 0x00018050 File Offset: 0x00016250
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event EventHandler PACFileChanged;

		// Token: 0x14000005 RID: 5
		// Token: 0x06000287 RID: 647 RVA: 0x00018088 File Offset: 0x00016288
		// Token: 0x06000288 RID: 648 RVA: 0x000180C0 File Offset: 0x000162C0
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event EventHandler UserRuleFileChanged;

		// Token: 0x040001F5 RID: 501
		public static string BYPASS_FILE = "bypass.action";

		// Token: 0x040001F8 RID: 504
		private FileSystemWatcher PACFileWatcher;

		// Token: 0x040001F4 RID: 500
		public static string PAC_FILE = "pac.txt";

		// Token: 0x040001F9 RID: 505
		private FileSystemWatcher UserRuleFileWatcher;

		// Token: 0x040001F7 RID: 503
		public static string USER_ABP_FILE = "abp.txt";

		// Token: 0x040001F6 RID: 502
		public static string USER_RULE_FILE = "user-rule.txt";

		// Token: 0x040001FA RID: 506
		private Configuration _config;
	}
}
