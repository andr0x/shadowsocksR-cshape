using System;
using System.Collections.Generic;
using Shadowsocks.Util;

namespace Shadowsocks.Obfs
{
	// Token: 0x02000012 RID: 18
	internal class HttpSimpleObfs : ObfsBase
	{
		// Token: 0x060000E0 RID: 224 RVA: 0x0000E5AC File Offset: 0x0000C7AC
		static HttpSimpleObfs()
		{
			// Note: this type is marked as 'beforefieldinit'.
			Dictionary<string, int[]> dictionary = new Dictionary<string, int[]>();
			dictionary.Add("http_simple", new int[]
			{
				0,
				1,
				1
			});
			dictionary.Add("http_post", new int[]
			{
				0,
				1,
				1
			});
			Dictionary<string, int[]> arg_48_0 = dictionary;
			string arg_48_1 = "random_head";
			int[] expr_44 = new int[3];
			expr_44[1] = 1;
			arg_48_0.Add(arg_48_1, expr_44);
			HttpSimpleObfs._obfs = dictionary;
			HttpSimpleObfs._request_path = new string[]
			{
				"",
				"",
				"login.php?redir=",
				"",
				"register.php?code=",
				"",
				"?keyword=",
				"",
				"search?src=typd&q=",
				"&lang=en",
				"s?ie=utf-8&f=8&rsv_bp=1&rsv_idx=1&ch=&bar=&wd=",
				"&rn=",
				"post.php?id=",
				"&goto=view.php"
			};
			HttpSimpleObfs._request_useragent = new string[]
			{
				"Mozilla/5.0 (Windows NT 6.3; WOW64; rv:40.0) Gecko/20100101 Firefox/40.0",
				"Mozilla/5.0 (Windows NT 6.3; WOW64; rv:40.0) Gecko/20100101 Firefox/44.0",
				"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36",
				"Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/535.11 (KHTML, like Gecko) Ubuntu/11.10 Chromium/27.0.1453.93 Chrome/27.0.1453.93 Safari/537.36",
				"Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:35.0) Gecko/20100101 Firefox/35.0",
				"Mozilla/5.0 (compatible; WOW64; MSIE 10.0; Windows NT 6.2)",
				"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/533.20.25 (KHTML, like Gecko) Version/5.0.4 Safari/533.20.27",
				"Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.3; Trident/7.0; .NET4.0E; .NET4.0C)",
				"Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko",
				"Mozilla/5.0 (Linux; Android 4.4; Nexus 5 Build/BuildID) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/30.0.0.0 Mobile Safari/537.36",
				"Mozilla/5.0 (iPad; CPU OS 5_0 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko) Version/5.1 Mobile/9A334 Safari/7534.48.3",
				"Mozilla/5.0 (iPhone; CPU iPhone OS 5_0 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko) Version/5.1 Mobile/9A334 Safari/7534.48.3"
			};
			HttpSimpleObfs._useragent_index = new Random().Next(HttpSimpleObfs._request_useragent.Length);
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x0000DFE8 File Offset: 0x0000C1E8
		public HttpSimpleObfs(string method) : base(method)
		{
			this.has_sent_header = false;
			this.raw_trans_sent = false;
			this.raw_trans_recv = false;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x0000E074 File Offset: 0x0000C274
		private string boundary()
		{
			string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			string text2 = "";
			for (int i = 0; i < 32; i++)
			{
				text2 += text[this.random.Next(text.Length)].ToString();
			}
			return text2;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x0000E520 File Offset: 0x0000C720
		public override byte[] ClientDecode(byte[] encryptdata, int datalength, out int outlength, out bool needsendback)
		{
			if (this.raw_trans_recv)
			{
				outlength = datalength;
				needsendback = false;
				return encryptdata;
			}
			byte[] array = new byte[datalength];
			if (this.Method == "random_head")
			{
				outlength = 0;
				this.raw_trans_recv = true;
				needsendback = true;
				return encryptdata;
			}
			int num = this.FindSubArray(encryptdata, datalength, new byte[]
			{
				13,
				10,
				13,
				10
			});
			if (num > 0)
			{
				outlength = datalength - (num + 4);
				Array.Copy(encryptdata, num + 4, array, 0, outlength);
				this.raw_trans_recv = true;
			}
			else
			{
				outlength = 0;
			}
			needsendback = false;
			return array;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x0000E0C8 File Offset: 0x0000C2C8
		public override byte[] ClientEncode(byte[] encryptdata, int datalength, out int outlength)
		{
			if (this.raw_trans_sent)
			{
				this.SentLength += (long)datalength;
				outlength = datalength;
				return encryptdata;
			}
			byte[] array = new byte[datalength + 4096];
			if (this.Method == "random_head")
			{
				if (this.has_sent_header)
				{
					outlength = 0;
					if (datalength > 0)
					{
						byte[] array2 = new byte[datalength];
						Array.Copy(encryptdata, 0, array2, 0, datalength);
						this.data_buffer.Add(array2);
					}
					else
					{
						foreach (byte[] current in this.data_buffer)
						{
							Array.Copy(current, 0, array, outlength, current.Length);
							this.SentLength += (long)current.Length;
							outlength += current.Length;
						}
						this.data_buffer.Clear();
						this.raw_trans_sent = true;
					}
				}
				else
				{
					byte[] array3 = new byte[this.random.Next(96) + 8];
					this.random.NextBytes(array3);
					CRC32.SetCRC32(array3);
					array3.CopyTo(array, 0);
					outlength = array3.Length;
					byte[] array4 = new byte[datalength];
					Array.Copy(encryptdata, 0, array4, 0, datalength);
					this.data_buffer.Add(array4);
				}
			}
			else if (this.Method == "http_simple" || this.Method == "http_post")
			{
				int num = this.Server.iv.Length + this.Server.head_len;
				byte[] array5;
				if (datalength - num > 64)
				{
					array5 = new byte[num + this.random.Next(0, 64)];
				}
				else
				{
					array5 = new byte[datalength];
				}
				Array.Copy(encryptdata, 0, array5, 0, array5.Length);
				int num2 = new Random().Next(HttpSimpleObfs._request_path.Length / 2) * 2;
				string text = this.Server.host;
				string text2 = "";
				if (this.Server.param.Length > 0)
				{
					string[] array6 = this.Server.param.Split(new char[]
					{
						'#'
					}, 2);
					string text3 = this.Server.param;
					if (array6.Length > 1)
					{
						text2 = array6[1];
						text3 = array6[0];
					}
					string[] array7 = text3.Split(new char[]
					{
						','
					});
					text = array7[this.random.Next(array7.Length)];
				}
				string[] expr_261 = new string[8];
				expr_261[0] = ((this.Method == "http_post") ? "POST /" : "GET /");
				expr_261[1] = HttpSimpleObfs._request_path[num2];
				int arg_299_1 = 2;
				byte[] expr_291 = array5;
				expr_261[arg_299_1] = this.data2urlencode(expr_291, expr_291.Length);
				expr_261[3] = HttpSimpleObfs._request_path[num2 + 1];
				expr_261[4] = " HTTP/1.1\r\nHost: ";
				expr_261[5] = text;
				expr_261[6] = ((this.Server.port == 80) ? "" : (":" + this.Server.port.ToString()));
				expr_261[7] = "\r\n";
				string text4 = string.Concat(expr_261);
				if (text2.Length > 0)
				{
					text4 = text4 + text2.Replace("\\n", "\r\n") + "\r\n\r\n";
				}
				else
				{
					text4 = string.Concat(new string[]
					{
						text4,
						"User-Agent: ",
						HttpSimpleObfs._request_useragent[HttpSimpleObfs._useragent_index],
						"\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8\r\nAccept-Language: en-US,en;q=0.8\r\nAccept-Encoding: gzip, deflate\r\n",
						(this.Method == "http_post") ? ("Content-Type: multipart/form-data; boundary=" + this.boundary() + "\r\n") : "",
						"DNT: 1\r\nConnection: keep-alive\r\n\r\n"
					});
				}
				for (int i = 0; i < text4.Length; i++)
				{
					array[i] = (byte)text4[i];
				}
				if (array5.Length < datalength)
				{
					Array.Copy(encryptdata, array5.Length, array, text4.Length, datalength - array5.Length);
				}
				this.SentLength += (long)array5.Length;
				outlength = text4.Length + datalength - array5.Length;
				this.raw_trans_sent = true;
			}
			else
			{
				outlength = 0;
			}
			this.has_sent_header = true;
			return array;
		}

		// Token: 0x060000DA RID: 218 RVA: 0x0000E034 File Offset: 0x0000C234
		private string data2urlencode(byte[] encryptdata, int datalength)
		{
			string text = "";
			for (int i = 0; i < datalength; i++)
			{
				text = text + "%" + encryptdata[i].ToString("x2");
			}
			return text;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00009C9F File Offset: 0x00007E9F
		public override void Dispose()
		{
		}

		// Token: 0x060000DD RID: 221 RVA: 0x0000E4E4 File Offset: 0x0000C6E4
		private int FindSubArray(byte[] array, int length, byte[] subArray)
		{
			for (int i = 0; i < length; i++)
			{
				int num = 0;
				while (num < subArray.Length && array[i + num] == subArray[num])
				{
					num++;
				}
				if (num == subArray.Length)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x0000E02D File Offset: 0x0000C22D
		public override Dictionary<string, int[]> GetObfs()
		{
			return HttpSimpleObfs._obfs;
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x0000E01C File Offset: 0x0000C21C
		public static List<string> SupportedObfs()
		{
			return new List<string>(HttpSimpleObfs._obfs.Keys);
		}

		// Token: 0x040000CC RID: 204
		private List<byte[]> data_buffer = new List<byte[]>();

		// Token: 0x040000C9 RID: 201
		private bool has_sent_header;

		// Token: 0x040000CD RID: 205
		private Random random = new Random();

		// Token: 0x040000CB RID: 203
		private bool raw_trans_recv;

		// Token: 0x040000CA RID: 202
		private bool raw_trans_sent;

		// Token: 0x040000C5 RID: 197
		private static Dictionary<string, int[]> _obfs;

		// Token: 0x040000C6 RID: 198
		private static string[] _request_path;

		// Token: 0x040000C7 RID: 199
		private static string[] _request_useragent;

		// Token: 0x040000C8 RID: 200
		private static int _useragent_index;
	}
}
