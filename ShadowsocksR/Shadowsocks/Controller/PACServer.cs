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
	// Token: 0x0200004D RID: 77
	internal class PACServer : Listener.Service
	{
		// Token: 0x060002AE RID: 686 RVA: 0x000199A1 File Offset: 0x00017BA1
		public PACServer()
		{
			this.WatchPacFile();
			this.WatchUserRuleFile();
		}

		// Token: 0x060002BA RID: 698 RVA: 0x0001A068 File Offset: 0x00018268
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

		// Token: 0x060002B3 RID: 691 RVA: 0x00019C70 File Offset: 0x00017E70
		private string GetPACContent()
		{
			if (File.Exists(PACServer.PAC_FILE))
			{
				return File.ReadAllText(PACServer.PAC_FILE, Encoding.UTF8);
			}
			return Utils.UnGzip(Resources.proxy_pac_txt);
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x000199C0 File Offset: 0x00017BC0
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

		// Token: 0x060002B5 RID: 693 RVA: 0x00019E88 File Offset: 0x00018088
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

		// Token: 0x060002B4 RID: 692 RVA: 0x00019C98 File Offset: 0x00017E98
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

		// Token: 0x060002B1 RID: 689 RVA: 0x00019C20 File Offset: 0x00017E20
		public string TouchPACFile()
		{
			if (File.Exists(PACServer.PAC_FILE))
			{
				return PACServer.PAC_FILE;
			}
			FileManager.UncompressFile(PACServer.PAC_FILE, Resources.proxy_pac_txt);
			return PACServer.PAC_FILE;
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x00019C48 File Offset: 0x00017E48
		internal string TouchUserRuleFile()
		{
			if (File.Exists(PACServer.USER_RULE_FILE))
			{
				return PACServer.USER_RULE_FILE;
			}
			File.WriteAllText(PACServer.USER_RULE_FILE, Resources.user_rule);
			return PACServer.USER_RULE_FILE;
		}

		// Token: 0x060002AF RID: 687 RVA: 0x000199B5 File Offset: 0x00017BB5
		public void UpdateConfiguration(Configuration config)
		{
			this._config = config;
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x0001A04C File Offset: 0x0001824C
		private void UserRuleFileWatcher_Changed(object sender, FileSystemEventArgs e)
		{
			if (this.UserRuleFileChanged != null)
			{
				this.UserRuleFileChanged(this, new EventArgs());
			}
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x0001A031 File Offset: 0x00018231
		private void Watcher_Changed(object sender, FileSystemEventArgs e)
		{
			if (this.PACFileChanged != null)
			{
				this.PACFileChanged(this, new EventArgs());
			}
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x00019EC4 File Offset: 0x000180C4
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

		// Token: 0x060002B7 RID: 695 RVA: 0x00019F7C File Offset: 0x0001817C
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
		// Token: 0x060002AA RID: 682 RVA: 0x000198C4 File Offset: 0x00017AC4
		// Token: 0x060002AB RID: 683 RVA: 0x000198FC File Offset: 0x00017AFC
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event EventHandler PACFileChanged;

		// Token: 0x14000005 RID: 5
		// Token: 0x060002AC RID: 684 RVA: 0x00019934 File Offset: 0x00017B34
		// Token: 0x060002AD RID: 685 RVA: 0x0001996C File Offset: 0x00017B6C
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event EventHandler UserRuleFileChanged;

		// Token: 0x04000209 RID: 521
		public static string BYPASS_FILE = "bypass.action";

		// Token: 0x0400020C RID: 524
		private FileSystemWatcher PACFileWatcher;

		// Token: 0x04000208 RID: 520
		public static string PAC_FILE = "pac.txt";

		// Token: 0x0400020D RID: 525
		private FileSystemWatcher UserRuleFileWatcher;

		// Token: 0x0400020B RID: 523
		public static string USER_ABP_FILE = "abp.txt";

		// Token: 0x0400020A RID: 522
		public static string USER_RULE_FILE = "user-rule.txt";

		// Token: 0x0400020E RID: 526
		private Configuration _config;
	}
}
