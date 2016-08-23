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
	// Token: 0x0200004C RID: 76
	public class UpdateChecker
	{
		// Token: 0x060002A2 RID: 674 RVA: 0x00019530 File Offset: 0x00017730
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

		// Token: 0x060002A3 RID: 675 RVA: 0x000195C0 File Offset: 0x000177C0
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

		// Token: 0x060002A7 RID: 679 RVA: 0x00019730 File Offset: 0x00017930
		private void http_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			try
			{
				string result = e.Result;
				XmlDocument expr_0C = new XmlDocument();
				expr_0C.LoadXml(result);
				XmlNodeList arg_23_0 = expr_0C.GetElementsByTagName("media:content");
				List<string> list = new List<string>();
				//using (
                IEnumerator enumerator = arg_23_0.GetEnumerator();//)
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

		// Token: 0x060002A6 RID: 678 RVA: 0x00019688 File Offset: 0x00017888
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
			string r = "3.8.5.0";
			return UpdateChecker.CompareVersion(text, r) > 0;
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x00019638 File Offset: 0x00017838
		private static string ParseVersionFromURL(string url)
		{
			Match match = Regex.Match(url, ".*ShadowsocksR-win.*?-([\\d\\.]+)\\.\\w+", RegexOptions.IgnoreCase);
			if (match.Success && match.Groups.Count == 2)
			{
				return match.Groups[1].Value;
			}
			return null;
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x0001967B File Offset: 0x0001787B
		private void SortVersions(List<string> versions)
		{
			versions.Sort(new UpdateChecker.VersionComparer());
		}

		// Token: 0x14000003 RID: 3
		// Token: 0x060002A0 RID: 672 RVA: 0x000194C0 File Offset: 0x000176C0
		// Token: 0x060002A1 RID: 673 RVA: 0x000194F8 File Offset: 0x000176F8
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event EventHandler NewVersionFound;

		// Token: 0x04000204 RID: 516
		public const string Copyright = "Copyright © BreakWall 2015";

		// Token: 0x04000206 RID: 518
		public const string FullVersion = "3.8.5.0 Beta";

		// Token: 0x04000200 RID: 512
		public string LatestVersionNumber;

		// Token: 0x04000201 RID: 513
		public string LatestVersionURL;

		// Token: 0x04000203 RID: 515
		public const string Name = "ShadowsocksR";

		// Token: 0x040001FF RID: 511
		private const string UpdateURL = "https://raw.github.com/breakwa11/shadowsocks-rss/master/update/ssr-win-3.8.xml";

		// Token: 0x04000207 RID: 519
		private static bool UseProxy = true;

		// Token: 0x04000205 RID: 517
		public const string Version = "3.8.5.0";

		// Token: 0x020000AF RID: 175
		public class VersionComparer : IComparer<string>
		{
			// Token: 0x06000580 RID: 1408 RVA: 0x0002BB8C File Offset: 0x00029D8C
			public int Compare(string x, string y)
			{
				return UpdateChecker.CompareVersion(UpdateChecker.ParseVersionFromURL(x), UpdateChecker.ParseVersionFromURL(y));
			}
		}
	}
}
