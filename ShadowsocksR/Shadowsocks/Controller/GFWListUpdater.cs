using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using Shadowsocks.Model;
using SimpleJson;

namespace Shadowsocks.Controller
{
	// Token: 0x0200003E RID: 62
	public class GFWListUpdater
	{
		// Token: 0x06000233 RID: 563 RVA: 0x00015B74 File Offset: 0x00013D74
		private void http_DownloadBypassListCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			try
			{
				string result = e.Result;
				if (File.Exists(GFWListUpdater.BYPASS_FILE) && File.ReadAllText(GFWListUpdater.BYPASS_FILE, Encoding.UTF8) == result)
				{
					this.update_type = 8;
					this.UpdateCompleted(this, new GFWListUpdater.ResultEventArgs(false));
				}
				else
				{
					File.WriteAllText(GFWListUpdater.BYPASS_FILE, result, Encoding.UTF8);
					if (this.UpdateCompleted != null)
					{
						this.update_type = 8;
						this.UpdateCompleted(this, new GFWListUpdater.ResultEventArgs(true));
					}
				}
			}
			catch (Exception exception)
			{
				if (this.Error != null)
				{
					this.Error(this, new ErrorEventArgs(exception));
				}
			}
		}

		// Token: 0x06000230 RID: 560 RVA: 0x000158B4 File Offset: 0x00013AB4
		private void http_DownloadGFWTemplateCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			try
			{
				string result = e.Result;
				if (result.IndexOf("__RULES__") > 0 && result.IndexOf("FindProxyForURL") > 0)
				{
					GFWListUpdater.gfwlist_template = result;
					if (this.lastConfig != null)
					{
						this.UpdatePACFromGFWList(this.lastConfig);
					}
					this.lastConfig = null;
				}
				else
				{
					this.Error(this, new ErrorEventArgs(new Exception("Download ERROR")));
				}
			}
			catch (Exception exception)
			{
				if (this.Error != null)
				{
					this.Error(this, new ErrorEventArgs(exception));
				}
			}
		}

		// Token: 0x06000232 RID: 562 RVA: 0x00015AC0 File Offset: 0x00013CC0
		private void http_DownloadPACCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			try
			{
				string result = e.Result;
				if (File.Exists(GFWListUpdater.PAC_FILE) && File.ReadAllText(GFWListUpdater.PAC_FILE, Encoding.UTF8) == result)
				{
					this.update_type = 1;
					this.UpdateCompleted(this, new GFWListUpdater.ResultEventArgs(false));
				}
				else
				{
					File.WriteAllText(GFWListUpdater.PAC_FILE, result, Encoding.UTF8);
					if (this.UpdateCompleted != null)
					{
						this.update_type = 1;
						this.UpdateCompleted(this, new GFWListUpdater.ResultEventArgs(true));
					}
				}
			}
			catch (Exception exception)
			{
				if (this.Error != null)
				{
					this.Error(this, new ErrorEventArgs(exception));
				}
			}
		}

		// Token: 0x06000231 RID: 561 RVA: 0x00015954 File Offset: 0x00013B54
		private void http_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			try
			{
				List<string> list = this.ParseResult(e.Result);
				if (File.Exists(GFWListUpdater.USER_RULE_FILE))
				{
					string[] array = File.ReadAllText(GFWListUpdater.USER_RULE_FILE, Encoding.UTF8).Split(new char[]
					{
						'\r',
						'\n'
					}, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < array.Length; i++)
					{
						string text = array[i];
						if (!text.StartsWith("!") && !text.StartsWith("["))
						{
							list.Add(text);
						}
					}
				}
				string text2 = GFWListUpdater.gfwlist_template;
				if (File.Exists(GFWListUpdater.USER_ABP_FILE))
				{
					text2 = File.ReadAllText(GFWListUpdater.USER_ABP_FILE, Encoding.UTF8);
				}
				else
				{
					text2 = GFWListUpdater.gfwlist_template;
				}
				text2 = text2.Replace("__RULES__", SimpleJson.SimpleJson.SerializeObject(list));
				if (File.Exists(GFWListUpdater.PAC_FILE) && File.ReadAllText(GFWListUpdater.PAC_FILE, Encoding.UTF8) == text2)
				{
					this.update_type = 0;
					this.UpdateCompleted(this, new GFWListUpdater.ResultEventArgs(false));
				}
				else
				{
					File.WriteAllText(GFWListUpdater.PAC_FILE, text2, Encoding.UTF8);
					if (this.UpdateCompleted != null)
					{
						this.update_type = 0;
						this.UpdateCompleted(this, new GFWListUpdater.ResultEventArgs(true));
					}
				}
			}
			catch (Exception exception)
			{
				if (this.Error != null)
				{
					this.Error(this, new ErrorEventArgs(exception));
				}
			}
		}

		// Token: 0x06000237 RID: 567 RVA: 0x00015DC8 File Offset: 0x00013FC8
		public List<string> ParseResult(string response)
		{
			byte[] bytes = Convert.FromBase64String(response);
			string[] expr_28 = Encoding.ASCII.GetString(bytes).Split(new char[]
			{
				'\r',
				'\n'
			}, StringSplitOptions.RemoveEmptyEntries);
			List<string> list = new List<string>(expr_28.Length);
			string[] array = expr_28;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (!text.StartsWith("!") && !text.StartsWith("["))
				{
					list.Add(text);
				}
			}
			return list;
		}

		// Token: 0x06000236 RID: 566 RVA: 0x00015D60 File Offset: 0x00013F60
		public void UpdateBypassListFromDefault(Configuration config)
		{
			WebClient expr_05 = new WebClient();
			expr_05.Proxy = new WebProxy(IPAddress.Loopback.ToString(), config.localPort);
			expr_05.DownloadStringCompleted += new DownloadStringCompletedEventHandler(this.http_DownloadBypassListCompleted);
			expr_05.DownloadStringAsync(new Uri("https://raw.githubusercontent.com/breakwa11/gfw_whitelist/master/ssr/ss_bypass.action?rnd=" + this.random.Next().ToString()));
		}

		// Token: 0x06000234 RID: 564 RVA: 0x00015C28 File Offset: 0x00013E28
		public void UpdatePACFromGFWList(Configuration config)
		{
			if (GFWListUpdater.gfwlist_template == null)
			{
				this.lastConfig = config;
				WebClient expr_13 = new WebClient();
				expr_13.Proxy = new WebProxy(IPAddress.Loopback.ToString(), config.localPort);
				expr_13.DownloadStringCompleted += new DownloadStringCompletedEventHandler(this.http_DownloadGFWTemplateCompleted);
				expr_13.DownloadStringAsync(new Uri("https://raw.githubusercontent.com/breakwa11/gfw_whitelist/master/ss_gfw.pac?rnd=" + this.random.Next().ToString()));
				return;
			}
			WebClient expr_6D = new WebClient();
			expr_6D.Proxy = new WebProxy(IPAddress.Loopback.ToString(), config.localPort);
			expr_6D.DownloadStringCompleted += new DownloadStringCompletedEventHandler(this.http_DownloadStringCompleted);
			expr_6D.DownloadStringAsync(new Uri("https://raw.githubusercontent.com/gfwlist/gfwlist/master/gfwlist.txt?rnd=" + this.random.Next().ToString()));
		}

		// Token: 0x06000235 RID: 565 RVA: 0x00015CF8 File Offset: 0x00013EF8
		public void UpdatePACFromGFWList(Configuration config, string url)
		{
			WebClient expr_05 = new WebClient();
			expr_05.Proxy = new WebProxy(IPAddress.Loopback.ToString(), config.localPort);
			expr_05.DownloadStringCompleted += new DownloadStringCompletedEventHandler(this.http_DownloadPACCompleted);
			expr_05.DownloadStringAsync(new Uri(url + "?rnd=" + this.random.Next().ToString()));
		}

		// Token: 0x14000002 RID: 2
		// Token: 0x0600022E RID: 558 RVA: 0x00015844 File Offset: 0x00013A44
		// Token: 0x0600022F RID: 559 RVA: 0x0001587C File Offset: 0x00013A7C
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event ErrorEventHandler Error;

		// Token: 0x14000001 RID: 1
		// Token: 0x0600022C RID: 556 RVA: 0x000157D4 File Offset: 0x000139D4
		// Token: 0x0600022D RID: 557 RVA: 0x0001580C File Offset: 0x00013A0C
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event EventHandler<GFWListUpdater.ResultEventArgs> UpdateCompleted;

		// Token: 0x040001B1 RID: 433
		private static string BYPASS_FILE = PACServer.BYPASS_FILE;

		// Token: 0x040001AF RID: 431
		private const string BYPASS_LIST_URL = "https://raw.githubusercontent.com/breakwa11/gfw_whitelist/master/ssr/ss_bypass.action";

		// Token: 0x040001B4 RID: 436
		private static string gfwlist_template = null;

		// Token: 0x040001AE RID: 430
		private const string GFWLIST_TEMPLATE_URL = "https://raw.githubusercontent.com/breakwa11/gfw_whitelist/master/ss_gfw.pac";

		// Token: 0x040001AD RID: 429
		private const string GFWLIST_URL = "https://raw.githubusercontent.com/gfwlist/gfwlist/master/gfwlist.txt";

		// Token: 0x040001B5 RID: 437
		private Configuration lastConfig;

		// Token: 0x040001B0 RID: 432
		private static string PAC_FILE = PACServer.PAC_FILE;

		// Token: 0x040001B6 RID: 438
		private Random random = new Random();

		// Token: 0x040001B7 RID: 439
		public int update_type;

		// Token: 0x040001B3 RID: 435
		private static string USER_ABP_FILE = PACServer.USER_ABP_FILE;

		// Token: 0x040001B2 RID: 434
		private static string USER_RULE_FILE = PACServer.USER_RULE_FILE;

		// Token: 0x020000A6 RID: 166
		public class ResultEventArgs : EventArgs
		{
			// Token: 0x06000557 RID: 1367 RVA: 0x0002BCE3 File Offset: 0x00029EE3
			public ResultEventArgs(bool success)
			{
				this.Success = success;
			}

			// Token: 0x0400043E RID: 1086
			public bool Success;
		}
	}
}
