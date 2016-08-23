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
	// Token: 0x0200003C RID: 60
	public class GFWListUpdater
	{
		// Token: 0x0600021E RID: 542 RVA: 0x000152E4 File Offset: 0x000134E4
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

		// Token: 0x0600021B RID: 539 RVA: 0x00015024 File Offset: 0x00013224
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

		// Token: 0x0600021D RID: 541 RVA: 0x00015230 File Offset: 0x00013430
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

		// Token: 0x0600021C RID: 540 RVA: 0x000150C4 File Offset: 0x000132C4
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

		// Token: 0x06000222 RID: 546 RVA: 0x00015538 File Offset: 0x00013738
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

		// Token: 0x06000221 RID: 545 RVA: 0x000154D0 File Offset: 0x000136D0
		public void UpdateBypassListFromDefault(Configuration config)
		{
			WebClient expr_05 = new WebClient();
			expr_05.Proxy = new WebProxy(IPAddress.Loopback.ToString(), config.localPort);
			expr_05.DownloadStringCompleted += new DownloadStringCompletedEventHandler(this.http_DownloadBypassListCompleted);
			expr_05.DownloadStringAsync(new Uri("https://raw.githubusercontent.com/breakwa11/gfw_whitelist/master/ssr/ss_bypass.action?rnd=" + this.random.Next().ToString()));
		}

		// Token: 0x0600021F RID: 543 RVA: 0x00015398 File Offset: 0x00013598
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

		// Token: 0x06000220 RID: 544 RVA: 0x00015468 File Offset: 0x00013668
		public void UpdatePACFromGFWList(Configuration config, string url)
		{
			WebClient expr_05 = new WebClient();
			expr_05.Proxy = new WebProxy(IPAddress.Loopback.ToString(), config.localPort);
			expr_05.DownloadStringCompleted += new DownloadStringCompletedEventHandler(this.http_DownloadPACCompleted);
			expr_05.DownloadStringAsync(new Uri(url + "?rnd=" + this.random.Next().ToString()));
		}

		// Token: 0x14000002 RID: 2
		// Token: 0x06000219 RID: 537 RVA: 0x00014FB4 File Offset: 0x000131B4
		// Token: 0x0600021A RID: 538 RVA: 0x00014FEC File Offset: 0x000131EC
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event ErrorEventHandler Error;

		// Token: 0x14000001 RID: 1
		// Token: 0x06000217 RID: 535 RVA: 0x00014F44 File Offset: 0x00013144
		// Token: 0x06000218 RID: 536 RVA: 0x00014F7C File Offset: 0x0001317C
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event EventHandler<GFWListUpdater.ResultEventArgs> UpdateCompleted;

		// Token: 0x040001A7 RID: 423
		private static string BYPASS_FILE = PACServer.BYPASS_FILE;

		// Token: 0x040001A5 RID: 421
		private const string BYPASS_LIST_URL = "https://raw.githubusercontent.com/breakwa11/gfw_whitelist/master/ssr/ss_bypass.action";

		// Token: 0x040001AA RID: 426
		private static string gfwlist_template = null;

		// Token: 0x040001A4 RID: 420
		private const string GFWLIST_TEMPLATE_URL = "https://raw.githubusercontent.com/breakwa11/gfw_whitelist/master/ss_gfw.pac";

		// Token: 0x040001A3 RID: 419
		private const string GFWLIST_URL = "https://raw.githubusercontent.com/gfwlist/gfwlist/master/gfwlist.txt";

		// Token: 0x040001AB RID: 427
		private Configuration lastConfig;

		// Token: 0x040001A6 RID: 422
		private static string PAC_FILE = PACServer.PAC_FILE;

		// Token: 0x040001AC RID: 428
		private Random random = new Random();

		// Token: 0x040001AD RID: 429
		public int update_type;

		// Token: 0x040001A9 RID: 425
		private static string USER_ABP_FILE = PACServer.USER_ABP_FILE;

		// Token: 0x040001A8 RID: 424
		private static string USER_RULE_FILE = PACServer.USER_RULE_FILE;

		// Token: 0x020000A8 RID: 168
		public class ResultEventArgs : EventArgs
		{
			// Token: 0x06000560 RID: 1376 RVA: 0x0002AF2B File Offset: 0x0002912B
			public ResultEventArgs(bool success)
			{
				this.Success = success;
			}

			// Token: 0x0400043B RID: 1083
			public bool Success;
		}
	}
}
