using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml;
using Shadowsocks.Model;

namespace Shadowsocks.Controller
{
	// Token: 0x02000049 RID: 73
	public class UpdateChecker
	{
		// Token: 0x0600027D RID: 637 RVA: 0x00017C84 File Offset: 0x00015E84
		public void CheckUpdate(Configuration config)
		{
			try
			{
				WebClient webClient = new WebClient();
				webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.3319.102 Safari/537.36");
				if (UpdateChecker.UseProxy)
				{
					webClient.Proxy = new WebProxy(IPAddress.Loopback.ToString(), config.localPort);
				}
				else
				{
					webClient.Proxy = null;
				}
				webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(this.http_DownloadStringCompleted);
				webClient.DownloadStringAsync(new Uri("https://raw.github.com/breakwa11/shadowsocks-rss/master/update/ssr-win-3.8.xml"));
			}
			catch (Exception arg_6A_0)
			{
				Logging.LogUsefulException(arg_6A_0);
			}
		}

		// Token: 0x0600027E RID: 638 RVA: 0x00017D14 File Offset: 0x00015F14
		public static int CompareVersion(string l, string r)
		{
			string[] array = l.Split(new char[]
			{
				'.'
			});
			string[] array2 = r.Split(new char[]
			{
				'.'
			});
			for (int i = 0; i < Math.Max(array.Length, array2.Length); i++)
			{
				int num = (i < array.Length) ? int.Parse(array[i]) : 0;
				int num2 = (i < array2.Length) ? int.Parse(array2[i]) : 0;
				if (num != num2)
				{
					return num - num2;
				}
			}
			return 0;
		}

		// Token: 0x06000282 RID: 642 RVA: 0x00017E84 File Offset: 0x00016084
		private void http_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			try
			{
				string result = e.Result;
				XmlDocument expr_0C = new XmlDocument();
				expr_0C.LoadXml(result);
				XmlNodeList arg_23_0 = expr_0C.GetElementsByTagName("media:content");
				List<string> list = new List<string>();
				//using 
				IEnumerator enumerator = arg_23_0.GetEnumerator();
				{
					while (enumerator.MoveNext())
					{
						foreach (XmlAttribute xmlAttribute in ((XmlNode)enumerator.Current).Attributes)
						{
							if (xmlAttribute.Name == "url" && this.IsNewVersion(xmlAttribute.Value))
							{
								list.Add(xmlAttribute.Value);
							}
						}
					}
				}
				if (list.Count != 0)
				{
					this.SortVersions(list);
					List<string> expr_CE = list;
					this.LatestVersionURL = expr_CE[expr_CE.Count - 1];
					this.LatestVersionNumber = UpdateChecker.ParseVersionFromURL(this.LatestVersionURL);
					if (this.NewVersionFound != null)
					{
						this.NewVersionFound(this, new EventArgs());
					}
				}
			}
			catch (Exception arg_10C_0)
			{
				Logging.Debug(arg_10C_0.ToString());
				if (this.NewVersionFound != null)
				{
					this.NewVersionFound(this, new EventArgs());
				}
			}
		}

		// Token: 0x06000281 RID: 641 RVA: 0x00017DDC File Offset: 0x00015FDC
		private bool IsNewVersion(string url)
		{
			if (url.IndexOf("prerelease") >= 0)
			{
				return false;
			}
			AssemblyName[] arg_20_0 = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
			Version version = Environment.Version;
			AssemblyName[] array = arg_20_0;
			for (int i = 0; i < array.Length; i++)
			{
				AssemblyName assemblyName = array[i];
				if (assemblyName.Name == "mscorlib")
				{
					version = assemblyName.Version;
				}
			}
			if (version.Major >= 4)
			{
				if (url.IndexOf("dotnet4.0") < 0)
				{
					return false;
				}
			}
			else if (url.IndexOf("dotnet4.0") >= 0)
			{
				return false;
			}
			string text = UpdateChecker.ParseVersionFromURL(url);
			if (text == null)
			{
				return false;
			}
			string r = "3.8.4.2";
			return UpdateChecker.CompareVersion(text, r) > 0;
		}

		// Token: 0x0600027F RID: 639 RVA: 0x00017D8C File Offset: 0x00015F8C
		private static string ParseVersionFromURL(string url)
		{
			Match match = Regex.Match(url, ".*ShadowsocksR-win.*?-([\\d\\.]+)\\.\\w+", RegexOptions.IgnoreCase);
			if (match.Success && match.Groups.Count == 2)
			{
				return match.Groups[1].Value;
			}
			return null;
		}

		// Token: 0x06000280 RID: 640 RVA: 0x00017DCF File Offset: 0x00015FCF
		private void SortVersions(List<string> versions)
		{
			versions.Sort(new UpdateChecker.VersionComparer());
		}

		// Token: 0x14000003 RID: 3
		// Token: 0x0600027B RID: 635 RVA: 0x00017C14 File Offset: 0x00015E14
		// Token: 0x0600027C RID: 636 RVA: 0x00017C4C File Offset: 0x00015E4C
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event EventHandler NewVersionFound;

		// Token: 0x040001F0 RID: 496
		public const string Copyright = "Copyright © BreakWall 2015";

		// Token: 0x040001F2 RID: 498
		public const string FullVersion = "3.8.4.2";

		// Token: 0x040001EC RID: 492
		public string LatestVersionNumber;

		// Token: 0x040001ED RID: 493
		public string LatestVersionURL;

		// Token: 0x040001EF RID: 495
		public const string Name = "ShadowsocksR";

		// Token: 0x040001EB RID: 491
		private const string UpdateURL = "https://raw.github.com/breakwa11/shadowsocks-rss/master/update/ssr-win-3.8.xml";

		// Token: 0x040001F3 RID: 499
		private static bool UseProxy = true;

		// Token: 0x040001F1 RID: 497
		public const string Version = "3.8.4.2";

		// Token: 0x020000AC RID: 172
		public class VersionComparer : IComparer<string>
		{
			// Token: 0x06000577 RID: 1399 RVA: 0x0002C944 File Offset: 0x0002AB44
			public int Compare(string x, string y)
			{
				return UpdateChecker.CompareVersion(UpdateChecker.ParseVersionFromURL(x), UpdateChecker.ParseVersionFromURL(y));
			}
		}
	}
}
