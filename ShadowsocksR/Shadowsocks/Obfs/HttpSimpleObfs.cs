using System;
using System.Collections.Generic;
using Shadowsocks.Util;

namespace Shadowsocks.Obfs
{
	// Token: 0x02000013 RID: 19
	internal class HttpSimpleObfs : ObfsBase
	{
		// Token: 0x060000EA RID: 234 RVA: 0x0000E9B0 File Offset: 0x0000CBB0
		static HttpSimpleObfs()
		{
			// Note: this type is marked as 'beforefieldinit'.
			Dictionary<string, int[]> dictionary = new Dictionary<string, int[]>();
			Dictionary<string, int[]> arg_16_0 = dictionary;
			string arg_16_1 = "tls_simple";
			int[] expr_12 = new int[3];
			expr_12[1] = 1;
			arg_16_0.Add(arg_16_1, expr_12);
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
			Dictionary<string, int[]> arg_5D_0 = dictionary;
			string arg_5D_1 = "random_head";
			int[] expr_59 = new int[3];
			expr_59[1] = 1;
			arg_5D_0.Add(arg_5D_1, expr_59);
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

		// Token: 0x060000E1 RID: 225 RVA: 0x0000E244 File Offset: 0x0000C444
		public HttpSimpleObfs(string method) : base(method)
		{
			this.has_sent_header = false;
			this.raw_trans_sent = false;
			this.raw_trans_recv = false;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x0000E2D0 File Offset: 0x0000C4D0
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

		// Token: 0x060000E8 RID: 232 RVA: 0x0000E914 File Offset: 0x0000CB14
		public override byte[] ClientDecode(byte[] encryptdata, int datalength, out int outlength, out bool needsendback)
		{
			if (this.raw_trans_recv)
			{
				outlength = datalength;
				needsendback = false;
				return encryptdata;
			}
			byte[] array = new byte[datalength];
			if (this.Method == "tls_simple" || this.Method == "random_head")
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

		// Token: 0x060000E6 RID: 230 RVA: 0x0000E324 File Offset: 0x0000C524
		public override byte[] ClientEncode(byte[] encryptdata, int datalength, out int outlength)
		{
			if (this.raw_trans_sent)
			{
				this.SentLength += (long)datalength;
				outlength = datalength;
				return encryptdata;
			}
			byte[] array = new byte[datalength + 4096];
			if (this.Method == "tls_simple" || this.Method == "random_head")
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
				else if (this.Method == "random_head")
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
				else
				{
					byte[] array5 = new byte[32];
					this.random.NextBytes(array5);
					List<byte> list = new List<byte>();
					string text = "000016c02bc02fc00ac009c013c01400330039002f0035000a0100006fff01000100000a00080006001700180019000b0002010000230000337400000010002900270568322d31360568322d31350568322d313402683208737064792f332e3108687474702f312e31000500050100000000000d001600140401050106010201040305030603020304020202";
					byte[] array6 = array5;
					for (int i = 0; i < array6.Length; i++)
					{
						byte item = array6[i];
						list.Add(item);
					}
					for (int j = 0; j < text.Length; j += 2)
					{
						byte item2 = (byte)(text[j] | text[j + 1] * '\u0010');
						list.Add(item2);
					}
					list.Insert(0, 1);
					list.Insert(0, 3);
					list.Insert(0, (byte)(list.Count % 256));
					list.Insert(0, (byte)((list.Count - 1) / 256));
					list.Insert(0, 0);
					list.Insert(0, 1);
					list.Insert(0, (byte)(list.Count % 256));
					list.Insert(0, (byte)((list.Count - 1) / 256));
					list.Insert(0, 1);
					list.Insert(0, 3);
					list.Insert(0, 22);
					for (int k = 0; k < list.Count; k++)
					{
						array[k] = list[k];
					}
					outlength = list.Count;
					byte[] array7 = new byte[datalength];
					Array.Copy(encryptdata, 0, array7, 0, datalength);
					this.data_buffer.Add(array7);
				}
			}
			else if (this.Method == "http_simple" || this.Method == "http_post")
			{
				int num = this.Server.iv.Length + this.Server.head_len;
				byte[] array8;
				if (datalength - num > 64)
				{
					array8 = new byte[num + this.random.Next(0, 64)];
				}
				else
				{
					array8 = new byte[datalength];
				}
				Array.Copy(encryptdata, 0, array8, 0, array8.Length);
				int num2 = new Random().Next(HttpSimpleObfs._request_path.Length / 2) * 2;
				string text2 = this.Server.host;
				string text3 = "";
				if (this.Server.param.Length > 0)
				{
					string[] array9 = this.Server.param.Split(new char[]
					{
						'#'
					}, 2);
					string text4 = this.Server.param;
					if (array9.Length > 1)
					{
						text3 = array9[1];
						text4 = array9[0];
					}
					string[] array10 = text4.Split(new char[]
					{
						','
					});
					text2 = array10[this.random.Next(array10.Length)];
				}
				string[] expr_3F7 = new string[8];
				expr_3F7[0] = ((this.Method == "http_post") ? "POST /" : "GET /");
				expr_3F7[1] = HttpSimpleObfs._request_path[num2];
				int arg_42F_1 = 2;
				byte[] expr_427 = array8;
				expr_3F7[arg_42F_1] = this.data2urlencode(expr_427, expr_427.Length);
				expr_3F7[3] = HttpSimpleObfs._request_path[num2 + 1];
				expr_3F7[4] = " HTTP/1.1\r\nHost: ";
				expr_3F7[5] = text2;
				expr_3F7[6] = ((this.Server.port == 80) ? "" : (":" + this.Server.port.ToString()));
				expr_3F7[7] = "\r\n";
				string text5 = string.Concat(expr_3F7);
				if (text3.Length > 0)
				{
					text5 = text5 + text3.Replace("\\n", "\r\n") + "\r\n\r\n";
				}
				else
				{
					text5 = string.Concat(new string[]
					{
						text5,
						"User-Agent: ",
						HttpSimpleObfs._request_useragent[HttpSimpleObfs._useragent_index],
						"\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8\r\nAccept-Language: en-US,en;q=0.8\r\nAccept-Encoding: gzip, deflate\r\n",
						(this.Method == "http_post") ? ("Content-Type: multipart/form-data; boundary=" + this.boundary() + "\r\n") : "",
						"DNT: 1\r\nConnection: keep-alive\r\n\r\n"
					});
				}
				for (int l = 0; l < text5.Length; l++)
				{
					array[l] = (byte)text5[l];
				}
				if (array8.Length < datalength)
				{
					Array.Copy(encryptdata, array8.Length, array, text5.Length, datalength - array8.Length);
				}
				this.SentLength += (long)array8.Length;
				outlength = text5.Length + datalength - array8.Length;
				this.raw_trans_sent = true;
			}
			else
			{
				outlength = 0;
			}
			this.has_sent_header = true;
			return array;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x0000E290 File Offset: 0x0000C490
		private string data2urlencode(byte[] encryptdata, int datalength)
		{
			string text = "";
			for (int i = 0; i < datalength; i++)
			{
				text = text + "%" + encryptdata[i].ToString("x2");
			}
			return text;
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00009AEF File Offset: 0x00007CEF
		public override void Dispose()
		{
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x0000E8D8 File Offset: 0x0000CAD8
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

		// Token: 0x060000E3 RID: 227 RVA: 0x0000E289 File Offset: 0x0000C489
		public override Dictionary<string, int[]> GetObfs()
		{
			return HttpSimpleObfs._obfs;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x0000E278 File Offset: 0x0000C478
		public static List<string> SupportedObfs()
		{
			return new List<string>(HttpSimpleObfs._obfs.Keys);
		}

		// Token: 0x040000D0 RID: 208
		private List<byte[]> data_buffer = new List<byte[]>();

		// Token: 0x040000CD RID: 205
		private bool has_sent_header;

		// Token: 0x040000D1 RID: 209
		private Random random = new Random();

		// Token: 0x040000CF RID: 207
		private bool raw_trans_recv;

		// Token: 0x040000CE RID: 206
		private bool raw_trans_sent;

		// Token: 0x040000C9 RID: 201
		private static Dictionary<string, int[]> _obfs;

		// Token: 0x040000CA RID: 202
		private static string[] _request_path;

		// Token: 0x040000CB RID: 203
		private static string[] _request_useragent;

		// Token: 0x040000CC RID: 204
		private static int _useragent_index;
	}
}
