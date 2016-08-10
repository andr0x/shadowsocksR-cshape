using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Shadowsocks.Util;

namespace Shadowsocks.Controller
{
	// Token: 0x02000040 RID: 64
	internal class HttpPraser
	{
		// Token: 0x0600023C RID: 572 RVA: 0x00015EB4 File Offset: 0x000140B4
		public HttpPraser(bool redir = false)
		{
			this.redir = redir;
		}

		// Token: 0x06000240 RID: 576 RVA: 0x00016118 File Offset: 0x00014318
		protected int AppendRequest(ref byte[] Packet, ref int PacketLength)
		{
			if (this.httpContentLength > 0)
			{
				if (this.httpContentLength >= PacketLength)
				{
					this.httpContentLength -= PacketLength;
					PacketLength = 0;
					Packet = new byte[0];
					return -1;
				}
				int num = PacketLength - this.httpContentLength;
				byte[] array = new byte[num];
				Array.Copy(Packet, this.httpContentLength, array, 0, num);
				Packet = array;
				PacketLength -= this.httpContentLength;
				this.httpContentLength = 0;
			}
			byte[] m = new byte[]
			{
				13,
				10,
				13,
				10
			};
			if (this.httpRequestBuffer == null)
			{
				this.httpRequestBuffer = new byte[PacketLength];
			}
			else
			{
				Array.Resize<byte>(ref this.httpRequestBuffer, this.httpRequestBuffer.Length + PacketLength);
			}
			Array.Copy(Packet, 0, this.httpRequestBuffer, this.httpRequestBuffer.Length - PacketLength, PacketLength);
			return Utils.FindStr(this.httpRequestBuffer, this.httpRequestBuffer.Length, m);
		}

		// Token: 0x06000243 RID: 579 RVA: 0x000163D0 File Offset: 0x000145D0
		public int HandshakeReceive(byte[] _firstPacket, int _firstPacketLength, ref byte[] remoteHeaderSendBuffer)
		{
			remoteHeaderSendBuffer = null;
			int num = this.AppendRequest(ref _firstPacket, ref _firstPacketLength);
			if (num < 0)
			{
				return 1;
			}
			string @string = Encoding.UTF8.GetString(this.httpRequestBuffer, 0, num + 4);
			byte[] array = new byte[this.httpRequestBuffer.Length - (num + 4)];
			Array.Copy(this.httpRequestBuffer, num + 4, array, 0, array.Length);
			this.httpRequestBuffer = array;
			Dictionary<string, string> dictionary = this.ParseHttpHeader(@string);
			this.httpPort = 80;
			if (dictionary["@0"] == "CONNECT")
			{
				this.httpHost = HttpPraser.ParseHostAndPort(dictionary["@1"], ref this.httpPort);
			}
			else
			{
				if (!dictionary.ContainsKey("Host"))
				{
					return 500;
				}
				this.httpHost = HttpPraser.ParseHostAndPort(dictionary["Host"], ref this.httpPort);
			}
			if (dictionary.ContainsKey("Content-Length") && Convert.ToInt32(dictionary["Content-Length"]) > 0)
			{
				this.httpContentLength = Convert.ToInt32(dictionary["Content-Length"]) + 2;
			}
			this.HostToHandshakeBuffer(this.httpHost, this.httpPort, ref remoteHeaderSendBuffer);
			if (this.redir)
			{
				if (dictionary.ContainsKey("Proxy-Connection"))
				{
					dictionary["Connection"] = dictionary["Proxy-Connection"];
					dictionary.Remove("Proxy-Connection");
				}
				string s = this.HeaderDictToString(dictionary);
				int num2 = remoteHeaderSendBuffer.Length;
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				Array.Resize<byte>(ref remoteHeaderSendBuffer, num2 + bytes.Length);
				bytes.CopyTo(remoteHeaderSendBuffer, num2);
				this.httpProxy = true;
			}
			bool flag = false;
			if (this.httpAuthUser == null || this.httpAuthUser.Length == 0)
			{
				flag = true;
			}
			if (dictionary.ContainsKey("Proxy-Authorization"))
			{
				string b = dictionary["Proxy-Authorization"].Substring("Basic ".Length);
				string s2 = this.httpAuthUser + ":" + this.httpAuthPass;
				if (Convert.ToBase64String(Encoding.UTF8.GetBytes(s2)) == b)
				{
					flag = true;
				}
				dictionary.Remove("Proxy-Authorization");
			}
			if (flag && this.httpRequestBuffer.Length != 0)
			{
				int num3 = this.httpRequestBuffer.Length;
				Array arg_241_0 = this.httpRequestBuffer;
				Array.Resize<byte>(ref remoteHeaderSendBuffer, num3 + remoteHeaderSendBuffer.Length);
				arg_241_0.CopyTo(remoteHeaderSendBuffer, remoteHeaderSendBuffer.Length - num3);
				this.httpRequestBuffer = new byte[0];
			}
			if (flag && this.httpContentLength > 0)
			{
				int num4 = Math.Min(this.httpRequestBuffer.Length, this.httpContentLength);
				Array.Resize<byte>(ref remoteHeaderSendBuffer, num4 + remoteHeaderSendBuffer.Length);
				Array.Copy(this.httpRequestBuffer, 0, remoteHeaderSendBuffer, remoteHeaderSendBuffer.Length - num4, num4);
				byte[] array2 = new byte[this.httpRequestBuffer.Length - num4];
				Array.Copy(this.httpRequestBuffer, num4, array2, 0, array2.Length);
				this.httpRequestBuffer = array2;
			}
			else
			{
				this.httpContentLength = 0;
				this.httpRequestBuffer = new byte[0];
			}
			if (remoteHeaderSendBuffer == null || !flag)
			{
				return 2;
			}
			if (this.httpProxy)
			{
				return 3;
			}
			return 0;
		}

		// Token: 0x06000242 RID: 578 RVA: 0x000162A0 File Offset: 0x000144A0
		protected string HeaderDictToString(Dictionary<string, string> dict)
		{
			string str = "";
			string text = "";
			str = string.Concat(new string[]
			{
				dict["@0"],
				" ",
				dict["@1"],
				" ",
				dict["@2"],
				"\r\n"
			});
			dict.Remove("@0");
			dict.Remove("@1");
			dict.Remove("@2");
			text = text + "Host: " + dict["Host"] + "\r\n";
			dict.Remove("Host");
			foreach (KeyValuePair<string, string> current in dict)
			{
				text = string.Concat(new string[]
				{
					text,
					current.Key,
					": ",
					current.Value,
					"\r\n"
				});
			}
			return str + text + "\r\n";
		}

		// Token: 0x0600023F RID: 575 RVA: 0x00016050 File Offset: 0x00014250
		public void HostToHandshakeBuffer(string host, int port, ref byte[] remoteHeaderSendBuffer)
		{
			if (this.redir)
			{
				remoteHeaderSendBuffer = new byte[0];
				return;
			}
			if (host.Length > 0)
			{
				IPAddress iPAddress;
				if (!IPAddress.TryParse(host, out iPAddress))
				{
					remoteHeaderSendBuffer = new byte[2 + host.Length + 2];
					remoteHeaderSendBuffer[0] = 3;
					remoteHeaderSendBuffer[1] = (byte)host.Length;
					Encoding.UTF8.GetBytes(host).CopyTo(remoteHeaderSendBuffer, 2);
				}
				else if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
				{
					remoteHeaderSendBuffer = new byte[7];
					remoteHeaderSendBuffer[0] = 1;
					iPAddress.GetAddressBytes().CopyTo(remoteHeaderSendBuffer, 1);
				}
				else
				{
					remoteHeaderSendBuffer = new byte[19];
					remoteHeaderSendBuffer[0] = 4;
					iPAddress.GetAddressBytes().CopyTo(remoteHeaderSendBuffer, 1);
				}
				byte[] expr_A1 = remoteHeaderSendBuffer;
				expr_A1[expr_A1.Length - 2] = (byte)(port >> 8);
				byte[] expr_AD = remoteHeaderSendBuffer;
				expr_AD[expr_AD.Length - 1] = (byte)(port & 255);
			}
		}

		// Token: 0x06000244 RID: 580 RVA: 0x000166CB File Offset: 0x000148CB
		public string Http200()
		{
			return "HTTP/1.1 200 Connection Established\r\n\r\n";
		}

		// Token: 0x06000245 RID: 581 RVA: 0x000166D4 File Offset: 0x000148D4
		public string Http407()
		{
			string arg_16_0 = "HTTP/1.1 407 Proxy Authentication Required\r\nProxy-Authenticate: Basic realm=\"RRR\"\r\n";
			string str = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN \"http://www.w3.org/TR/1999/REC-html401-19991224/loose.dtd\"><HTML>  <HEAD>    <TITLE>Error</TITLE>    <META HTTP-EQUIV=\"Content-Type\" CONTENT=\"text/html; charset=ISO-8859-1\">  </HEAD>  <BODY><H1>407 Proxy Authentication Required.</H1></BODY></HTML>\r\n";
			return arg_16_0 + "\r\n" + str + "\r\n";
		}

		// Token: 0x06000246 RID: 582 RVA: 0x000166FC File Offset: 0x000148FC
		public string Http500()
		{
			string arg_16_0 = "HTTP/1.1 500 Internal Server Error\r\n";
			string str = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN \"http://www.w3.org/TR/1999/REC-html401-19991224/loose.dtd\"><HTML>  <HEAD>    <TITLE>Error</TITLE>    <META HTTP-EQUIV=\"Content-Type\" CONTENT=\"text/html; charset=ISO-8859-1\">  </HEAD>  <BODY><H1>500 Internal Server Error.</H1></BODY></HTML>";
			return arg_16_0 + "\r\n" + str + "\r\n";
		}

		// Token: 0x0600023D RID: 573 RVA: 0x00015EC4 File Offset: 0x000140C4
		private static string ParseHostAndPort(string str, ref int port)
		{
			string result;
			if (str.StartsWith("["))
			{
				int num = str.LastIndexOf(']');
				if (num > 0)
				{
					result = str.Substring(1, num - 1);
					if (str.Length > num + 1 && str[num + 2] == ':')
					{
						port = Convert.ToInt32(str.Substring(num + 2));
					}
				}
				else
				{
					result = str;
				}
			}
			else
			{
				int num2 = str.LastIndexOf(':');
				if (num2 > 0)
				{
					result = str.Substring(0, num2);
					port = Convert.ToInt32(str.Substring(num2 + 1));
				}
				else
				{
					result = str;
				}
			}
			return result;
		}

		// Token: 0x06000241 RID: 577 RVA: 0x000161F8 File Offset: 0x000143F8
		protected Dictionary<string, string> ParseHttpHeader(string header)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string[] array = header.Split(new string[]
			{
				"\r\n"
			}, StringSplitOptions.RemoveEmptyEntries);
			string[] array2 = array[0].Split(new char[]
			{
				' '
			}, 3);
			for (int i = 1; i < array.Length; i++)
			{
				string[] array3 = array[i].Split(new string[]
				{
					": "
				}, 2, StringSplitOptions.RemoveEmptyEntries);
				if (array3.Length > 1)
				{
					dictionary[array3[0]] = array3[1];
				}
			}
			dictionary["@0"] = array2[0];
			dictionary["@1"] = array2[1];
			dictionary["@2"] = array2[2];
			return dictionary;
		}

		// Token: 0x0600023E RID: 574 RVA: 0x00015F50 File Offset: 0x00014150
		protected string ParseURL(string url, string host, int port)
		{
			if (url.StartsWith("http://"))
			{
				url = url.Substring(7);
			}
			if (url.StartsWith("["))
			{
				if (url.StartsWith("[" + host + "]"))
				{
					url = url.Substring(host.Length + 2);
				}
			}
			else if (url.StartsWith(host))
			{
				url = url.Substring(host.Length);
			}
			if (url.StartsWith(":") && url.StartsWith(":" + port.ToString()))
			{
				url = url.Substring((":" + port.ToString()).Length);
			}
			if (!url.StartsWith("/"))
			{
				int num = url.IndexOf('/');
				int num2 = url.IndexOf(' ');
				if (num > 0 && num < num2)
				{
					url = url.Substring(num);
				}
			}
			if (url.StartsWith(" "))
			{
				url = "/" + url;
			}
			return url;
		}

		// Token: 0x040001C0 RID: 448
		public string httpAuthPass;

		// Token: 0x040001BF RID: 447
		public string httpAuthUser;

		// Token: 0x040001BE RID: 446
		public int httpContentLength;

		// Token: 0x040001C1 RID: 449
		protected string httpHost;

		// Token: 0x040001C2 RID: 450
		protected int httpPort;

		// Token: 0x040001BC RID: 444
		public bool httpProxy;

		// Token: 0x040001BD RID: 445
		public byte[] httpRequestBuffer;

		// Token: 0x040001C3 RID: 451
		private bool redir;
	}
}
