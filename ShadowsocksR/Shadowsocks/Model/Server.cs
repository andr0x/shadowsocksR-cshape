using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Shadowsocks.Controller;
using Shadowsocks.Encryption;
using Shadowsocks.Obfs;
using Shadowsocks.Util;

namespace Shadowsocks.Model
{
	// Token: 0x02000022 RID: 34
	[Serializable]
	public class Server
	{
		// Token: 0x06000148 RID: 328 RVA: 0x000101EC File Offset: 0x0000E3EC
		public Server()
		{
			this.server = "server ip or url";
			this.server_port = 8388;
			this.method = "aes-256-cfb";
			this.obfs = "plain";
			this.obfsparam = "";
			this.password = "0";
			this.remarks_base64 = "";
			this.udp_over_tcp = false;
			this.protocol = "origin";
			this.enable = true;
			byte[] array = new byte[16];
			byte[] expr_A1 = array;
			Utils.RandBytes(expr_A1, expr_A1.Length);
			this.id = BitConverter.ToString(array).Replace("-", "");
		}

		// Token: 0x06000149 RID: 329 RVA: 0x000102BD File Offset: 0x0000E4BD
		public Server(string ssURL) : this()
		{
			if (ssURL.StartsWith("ss://"))
			{
				this.ServerFromSS(ssURL);
				return;
			}
			if (ssURL.StartsWith("ssr://"))
			{
				this.ServerFromSSR(ssURL);
			}
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00010100 File Offset: 0x0000E300
		public Server Clone()
		{
			return new Server
			{
				server = (string)this.server.Clone(),
				server_port = this.server_port,
				password = (string)this.password.Clone(),
				method = (string)this.method.Clone(),
				protocol = this.protocol,
				obfs = (string)this.obfs.Clone(),
				obfsparam = (string)this.obfsparam.Clone(),
				remarks_base64 = (string)this.remarks_base64.Clone(),
				enable = this.enable,
				udp_over_tcp = this.udp_over_tcp,
				id = this.id,
				protocoldata = this.protocoldata,
				obfsdata = this.obfsdata
			};
		}

		// Token: 0x0600013D RID: 317 RVA: 0x0000FEDC File Offset: 0x0000E0DC
		public void CopyServer(Server Server)
		{
			this.serverSpeedLog = Server.serverSpeedLog;
			this.dnsBuffer = Server.dnsBuffer;
			this.dnsTargetBuffer = Server.dnsTargetBuffer;
			this.Connections = Server.Connections;
			this.enable = Server.enable;
		}

		// Token: 0x0600014A RID: 330 RVA: 0x000102EE File Offset: 0x0000E4EE
		public static string DecodeBase64(string val)
		{
			if (val.LastIndexOf(':') > 0)
			{
				return val;
			}
			return Utils.DecodeBase64(val);
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00010303 File Offset: 0x0000E503
		public static string DecodeUrlSafeBase64(string val)
		{
			if (val.LastIndexOf(':') > 0)
			{
				return val;
			}
			return Utils.DecodeUrlSafeBase64(val);
		}

		// Token: 0x06000140 RID: 320 RVA: 0x0000FF2B File Offset: 0x0000E12B
		public DnsBuffer DnsBuffer()
		{
			return this.dnsBuffer;
		}

		// Token: 0x06000141 RID: 321 RVA: 0x0000FF33 File Offset: 0x0000E133
		public DnsBuffer DnsTargetBuffer()
		{
			return this.dnsTargetBuffer;
		}

		// Token: 0x06000146 RID: 326 RVA: 0x0000FFD8 File Offset: 0x0000E1D8
		public string FriendlyName()
		{
			if (string.IsNullOrEmpty(this.server))
			{
				return I18N.GetString("New server");
			}
			if (string.IsNullOrEmpty(this.remarks_base64))
			{
				if (this.server.IndexOf(':') >= 0)
				{
					return string.Concat(new object[]
					{
						"[",
						this.server,
						"]:",
						this.server_port
					});
				}
				return this.server + ":" + this.server_port;
			}
			else
			{
				if (this.server.IndexOf(':') >= 0)
				{
					return string.Concat(new object[]
					{
						this.remarks,
						" ([",
						this.server,
						"]:",
						this.server_port,
						")"
					});
				}
				return string.Concat(new object[]
				{
					this.remarks,
					" (",
					this.server,
					":",
					this.server_port,
					")"
				});
			}
		}

		// Token: 0x0600013F RID: 319 RVA: 0x0000FF23 File Offset: 0x0000E123
		public Connections GetConnections()
		{
			return this.Connections;
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00010825 File Offset: 0x0000EA25
		public object getObfsData()
		{
			return this.obfsdata;
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00010836 File Offset: 0x0000EA36
		public object getProtocolData()
		{
			return this.protocoldata;
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00010814 File Offset: 0x0000EA14
		public bool isEnable()
		{
			return this.enable;
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00010604 File Offset: 0x0000E804
		public void ServerFromSS(string ssURL)
		{
			string text = Server.DecodeBase64(Regex.Split(ssURL, "ss://", RegexOptions.IgnoreCase)[1].ToString());
			if (text.Length == 0)
			{
				throw new FormatException();
			}
			try
			{
				int num = text.LastIndexOf('@');
				int num2 = text.IndexOf('#', num);
				if (num2 > 0)
				{
					if (num2 + 1 < text.Length)
					{
						this.remarks_base64 = text.Substring(num2 + 1);
					}
					text = text.Substring(0, num2);
				}
				num2 = text.IndexOf('/', num);
				if (num2 > 0)
				{
					if (num2 + 1 < text.Length)
					{
						text.Substring(num2 + 1);
					}
					text = text.Substring(0, num2);
				}
				string text2 = text.Substring(num + 1);
				int num3 = text2.LastIndexOf(':');
				this.server_port = int.Parse(text2.Substring(num3 + 1));
				this.server = text2.Substring(0, num3);
				string text3 = text.Substring(0, num);
				this.method = "";
				bool flag = true;
				while (flag)
				{
					string[] array = text3.Split(new char[]
					{
						':'
					}, 2);
					if (array.Length <= 1)
					{
						break;
					}
					try
					{
						ObfsBase obfsBase = (ObfsBase)ObfsFactory.GetObfs(array[0]);
						if (obfsBase.GetObfs().ContainsKey(array[0]))
						{
							int[] expr_134 = obfsBase.GetObfs()[array[0]];
							if (expr_134[0] == 1)
							{
								this.protocol = array[0];
							}
							if (expr_134[1] == 1)
							{
								this.obfs = array[0];
							}
						}
						else
						{
							flag = false;
						}
					}
					catch
					{
						try
						{
							EncryptorFactory.GetEncryptor(array[0], "m").Dispose();
							this.method = array[0];
							text3 = array[1];
						}
						catch
						{
						}
						break;
					}
					text3 = array[1];
				}
				if (this.method.Length == 0)
				{
					throw new FormatException();
				}
				this.password = text3;
			}
			catch (IndexOutOfRangeException)
			{
				throw new FormatException();
			}
		}

		// Token: 0x0600014C RID: 332 RVA: 0x00010318 File Offset: 0x0000E518
		public void ServerFromSSR(string ssrURL)
		{
			string text = Server.DecodeUrlSafeBase64(Regex.Split(ssrURL, "ssr://", RegexOptions.IgnoreCase)[1].ToString());
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (text.Length == 0)
			{
				throw new FormatException();
			}
			int num = text.IndexOf("?");
			if (num > 0)
			{
				string arg_5D_0 = text.Substring(num + 1);
				text = text.Substring(0, num);
				string[] array = arg_5D_0.Split(new char[]
				{
					'&'
				});
				for (int i = 0; i < array.Length; i++)
				{
					string text2 = array[i];
					if (text2.IndexOf('=') > 0)
					{
						int num2 = text2.IndexOf('=');
						string text3 = text2.Substring(0, num2);
						string text4 = text2.Substring(num2 + 1);
						if (text3 == "obfsparam" || text3 == "remarks" || text3 == "group")
						{
							text4 = Server.DecodeUrlSafeBase64(text4);
						}
						dictionary[text3] = text4;
					}
				}
			}
			if (text.IndexOf("/") >= 0)
			{
				text = text.Substring(0, text.LastIndexOf("/"));
			}
			string[] array2 = text.Split(new char[]
			{
				':'
			}, StringSplitOptions.None);
			if (array2.Length > 6)
			{
				string[] array3 = new string[6];
				string[] arg_13A_0 = array3;
				int arg_13A_1 = 5;
				string[] expr_134 = array2;
				arg_13A_0[arg_13A_1] = expr_134[expr_134.Length - 1];
				string[] arg_145_0 = array3;
				int arg_145_1 = 4;
				string[] expr_13F = array2;
				arg_145_0[arg_145_1] = expr_13F[expr_13F.Length - 2];
				string[] arg_150_0 = array3;
				int arg_150_1 = 3;
				string[] expr_14A = array2;
				arg_150_0[arg_150_1] = expr_14A[expr_14A.Length - 3];
				string[] arg_15B_0 = array3;
				int arg_15B_1 = 2;
				string[] expr_155 = array2;
				arg_15B_0[arg_15B_1] = expr_155[expr_155.Length - 4];
				string[] arg_166_0 = array3;
				int arg_166_1 = 1;
				string[] expr_160 = array2;
				arg_166_0[arg_166_1] = expr_160[expr_160.Length - 5];
				string[] arg_171_0 = array3;
				int arg_171_1 = 0;
				string[] expr_16B = array2;
				arg_171_0[arg_171_1] = expr_16B[expr_16B.Length - 6];
				for (int j = array2.Length - 7; j >= 0; j--)
				{
					array3[0] = array2[j] + ":" + array3[0];
				}
				array2 = array3;
			}
			if (array2.Length != 6)
			{
				throw new FormatException();
			}
			this.server = array2[0];
			this.server_port = int.Parse(array2[1]);
			this.protocol = ((array2[2].Length == 0) ? "origin" : array2[2]);
			this.protocol.Replace("_compatible", "");
			this.method = array2[3];
			this.obfs = ((array2[4].Length == 0) ? "plain" : array2[4]);
			this.obfs.Replace("_compatible", "");
			this.password = Server.DecodeUrlSafeBase64(array2[5]);
			if (dictionary.ContainsKey("obfsparam"))
			{
				this.obfsparam = dictionary["obfsparam"];
			}
			if (dictionary.ContainsKey("remarks"))
			{
				this.remarks = dictionary["remarks"];
			}
			if (dictionary.ContainsKey("group"))
			{
				this.group = dictionary["group"];
			}
			if (dictionary.ContainsKey("uot"))
			{
				this.udp_over_tcp = (int.Parse(dictionary["uot"]) != 0);
			}
			if (dictionary.ContainsKey("udpport"))
			{
				this.server_udp_port = int.Parse(dictionary["udpport"]);
			}
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000FF3B File Offset: 0x0000E13B
		public ServerSpeedLog ServerSpeedLog()
		{
			return this.serverSpeedLog;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x0000FF1A File Offset: 0x0000E11A
		public void SetConnections(Connections Connections)
		{
			this.Connections = Connections;
		}

		// Token: 0x0600014F RID: 335 RVA: 0x0001081C File Offset: 0x0000EA1C
		public void setEnable(bool enable)
		{
			this.enable = enable;
		}

		// Token: 0x06000151 RID: 337 RVA: 0x0001082D File Offset: 0x0000EA2D
		public void setObfsData(object data)
		{
			this.obfsdata = data;
		}

		// Token: 0x06000153 RID: 339 RVA: 0x0001083E File Offset: 0x0000EA3E
		public void setProtocolData(object data)
		{
			this.protocoldata = data;
		}

		// Token: 0x06000143 RID: 323 RVA: 0x0000FF43 File Offset: 0x0000E143
		public void SetServerSpeedLog(ServerSpeedLog log)
		{
			this.serverSpeedLog = log;
		}

		// Token: 0x17000011 RID: 17
		public string remarks
		{
			// Token: 0x06000144 RID: 324 RVA: 0x0000FF4C File Offset: 0x0000E14C
			get
			{
				if (this.remarks_base64.Length == 0)
				{
					return this.remarks_base64;
				}
				string text = this.remarks_base64.Replace('-', '+').Replace('_', '/');
				string result;
				try
				{
					result = Encoding.UTF8.GetString(Convert.FromBase64String(text));
				}
				catch (FormatException)
				{
					text = this.remarks_base64;
					this.remarks_base64 = Utils.EncodeUrlSafeBase64(this.remarks_base64);
					result = text;
				}
				return result;
			}
			// Token: 0x06000145 RID: 325 RVA: 0x0000FFC8 File Offset: 0x0000E1C8
			set
			{
				this.remarks_base64 = Utils.EncodeUrlSafeBase64(value);
			}
		}

		// Token: 0x04000102 RID: 258
		private Connections Connections = new Connections();

		// Token: 0x04000100 RID: 256
		private DnsBuffer dnsBuffer = new DnsBuffer();

		// Token: 0x04000101 RID: 257
		private DnsBuffer dnsTargetBuffer = new DnsBuffer();

		// Token: 0x040000FB RID: 251
		public bool enable;

		// Token: 0x040000F8 RID: 248
		public string group;

		// Token: 0x040000FC RID: 252
		public string id;

		// Token: 0x040000F4 RID: 244
		public string method;

		// Token: 0x040000F5 RID: 245
		public string obfs;

		// Token: 0x040000FE RID: 254
		private object obfsdata;

		// Token: 0x040000F6 RID: 246
		public string obfsparam;

		// Token: 0x040000F3 RID: 243
		public string password;

		// Token: 0x040000FA RID: 250
		public string protocol;

		// Token: 0x040000FD RID: 253
		private object protocoldata;

		// Token: 0x040000F7 RID: 247
		public string remarks_base64;

		// Token: 0x040000F0 RID: 240
		public string server;

		// Token: 0x040000FF RID: 255
		private ServerSpeedLog serverSpeedLog = new ServerSpeedLog();

		// Token: 0x040000F1 RID: 241
		public int server_port;

		// Token: 0x040000F2 RID: 242
		public int server_udp_port;

		// Token: 0x040000F9 RID: 249
		public bool udp_over_tcp;
	}
}
