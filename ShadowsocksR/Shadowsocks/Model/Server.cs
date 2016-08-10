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
	// Token: 0x02000024 RID: 36
	[Serializable]
	public class Server
	{
		// Token: 0x0600015C RID: 348 RVA: 0x00010C30 File Offset: 0x0000EE30
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

		// Token: 0x0600015D RID: 349 RVA: 0x00010D01 File Offset: 0x0000EF01
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

		// Token: 0x0600015B RID: 347 RVA: 0x00010B44 File Offset: 0x0000ED44
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

		// Token: 0x06000151 RID: 337 RVA: 0x00010920 File Offset: 0x0000EB20
		public void CopyServer(Server Server)
		{
			this.serverSpeedLog = Server.serverSpeedLog;
			this.dnsBuffer = Server.dnsBuffer;
			this.dnsTargetBuffer = Server.dnsTargetBuffer;
			this.Connections = Server.Connections;
			this.enable = Server.enable;
		}

		// Token: 0x0600015E RID: 350 RVA: 0x00010D32 File Offset: 0x0000EF32
		public static string DecodeBase64(string val)
		{
			if (val.LastIndexOf(':') > 0)
			{
				return val;
			}
			return Utils.DecodeBase64(val);
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00010D47 File Offset: 0x0000EF47
		public static string DecodeUrlSafeBase64(string val)
		{
			if (val.LastIndexOf(':') > 0)
			{
				return val;
			}
			return Utils.DecodeUrlSafeBase64(val);
		}

		// Token: 0x06000154 RID: 340 RVA: 0x0001096F File Offset: 0x0000EB6F
		public DnsBuffer DnsBuffer()
		{
			return this.dnsBuffer;
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00010977 File Offset: 0x0000EB77
		public DnsBuffer DnsTargetBuffer()
		{
			return this.dnsTargetBuffer;
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00010A1C File Offset: 0x0000EC1C
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

		// Token: 0x06000153 RID: 339 RVA: 0x00010967 File Offset: 0x0000EB67
		public Connections GetConnections()
		{
			return this.Connections;
		}

		// Token: 0x06000164 RID: 356 RVA: 0x000111E9 File Offset: 0x0000F3E9
		public object getObfsData()
		{
			return this.obfsdata;
		}

		// Token: 0x06000166 RID: 358 RVA: 0x000111FA File Offset: 0x0000F3FA
		public object getProtocolData()
		{
			return this.protocoldata;
		}

		// Token: 0x06000162 RID: 354 RVA: 0x000111D8 File Offset: 0x0000F3D8
		public bool isEnable()
		{
			return this.enable;
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00010FC8 File Offset: 0x0000F1C8
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

		// Token: 0x06000160 RID: 352 RVA: 0x00010D5C File Offset: 0x0000EF5C
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

		// Token: 0x06000156 RID: 342 RVA: 0x0001097F File Offset: 0x0000EB7F
		public ServerSpeedLog ServerSpeedLog()
		{
			return this.serverSpeedLog;
		}

		// Token: 0x06000152 RID: 338 RVA: 0x0001095E File Offset: 0x0000EB5E
		public void SetConnections(Connections Connections)
		{
			this.Connections = Connections;
		}

		// Token: 0x06000163 RID: 355 RVA: 0x000111E0 File Offset: 0x0000F3E0
		public void setEnable(bool enable)
		{
			this.enable = enable;
		}

		// Token: 0x06000165 RID: 357 RVA: 0x000111F1 File Offset: 0x0000F3F1
		public void setObfsData(object data)
		{
			this.obfsdata = data;
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00011202 File Offset: 0x0000F402
		public void setProtocolData(object data)
		{
			this.protocoldata = data;
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00010987 File Offset: 0x0000EB87
		public void SetServerSpeedLog(ServerSpeedLog log)
		{
			this.serverSpeedLog = log;
		}

		// Token: 0x17000011 RID: 17
		public string remarks
		{
			// Token: 0x06000158 RID: 344 RVA: 0x00010990 File Offset: 0x0000EB90
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
			// Token: 0x06000159 RID: 345 RVA: 0x00010A0C File Offset: 0x0000EC0C
			set
			{
				this.remarks_base64 = Utils.EncodeUrlSafeBase64(value);
			}
		}

		// Token: 0x0400010D RID: 269
		private Connections Connections = new Connections();

		// Token: 0x0400010B RID: 267
		private DnsBuffer dnsBuffer = new DnsBuffer();

		// Token: 0x0400010C RID: 268
		private DnsBuffer dnsTargetBuffer = new DnsBuffer();

		// Token: 0x04000106 RID: 262
		public bool enable;

		// Token: 0x04000103 RID: 259
		public string group;

		// Token: 0x04000107 RID: 263
		public string id;

		// Token: 0x040000FF RID: 255
		public string method;

		// Token: 0x04000100 RID: 256
		public string obfs;

		// Token: 0x04000109 RID: 265
		private object obfsdata;

		// Token: 0x04000101 RID: 257
		public string obfsparam;

		// Token: 0x040000FE RID: 254
		public string password;

		// Token: 0x04000105 RID: 261
		public string protocol;

		// Token: 0x04000108 RID: 264
		private object protocoldata;

		// Token: 0x04000102 RID: 258
		public string remarks_base64;

		// Token: 0x040000FB RID: 251
		public string server;

		// Token: 0x0400010A RID: 266
		private ServerSpeedLog serverSpeedLog = new ServerSpeedLog();

		// Token: 0x040000FC RID: 252
		public int server_port;

		// Token: 0x040000FD RID: 253
		public int server_udp_port;

		// Token: 0x04000104 RID: 260
		public bool udp_over_tcp;
	}
}
