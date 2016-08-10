using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Shadowsocks.Util;

namespace Shadowsocks.Obfs
{
	// Token: 0x02000016 RID: 22
	internal class TlsTicketAuthObfs : ObfsBase
	{
		// Token: 0x060000F6 RID: 246 RVA: 0x0000F0BC File Offset: 0x0000D2BC
		public TlsTicketAuthObfs(string method) : base(method)
		{
			this.handshake_status = 0;
		}

		// Token: 0x06000100 RID: 256 RVA: 0x0000F7B4 File Offset: 0x0000D9B4
		public override byte[] ClientDecode(byte[] encryptdata, int datalength, out int outlength, out bool needsendback)
		{
			if (this.handshake_status == -1)
			{
				outlength = datalength;
				needsendback = false;
				return encryptdata;
			}
			if (this.handshake_status == 8)
			{
				Array.Resize<byte>(ref this.data_recv_buffer, this.data_recv_buffer.Length + datalength);
				Array.Copy(encryptdata, 0, this.data_recv_buffer, this.data_recv_buffer.Length - datalength, datalength);
				needsendback = false;
				byte[] array = new byte[65536];
				outlength = 0;
				while (this.data_recv_buffer.Length > 5)
				{
					if (this.data_recv_buffer[0] != 23)
					{
						throw new ObfsException("ClientDecode appdata error");
					}
					int num = ((int)this.data_recv_buffer[3] << 8) + (int)this.data_recv_buffer[4];
					int num2 = num + 5;
					if (num2 > this.data_recv_buffer.Length)
					{
						break;
					}
					Array.Copy(this.data_recv_buffer, 5, array, outlength, num);
					outlength += num;
					byte[] array2 = new byte[this.data_recv_buffer.Length - num2];
					Array.Copy(this.data_recv_buffer, num2, array2, 0, array2.Length);
					this.data_recv_buffer = array2;
				}
				return array;
			}
			//new byte[datalength];
			if (datalength < 76)
			{
				throw new ObfsException("ClientDecode data error");
			}
			byte[] array3 = new byte[10];
			byte[] array4 = new byte[32];
			Array.Copy(encryptdata, 11, array4, 0, 22);
			byte[] expr_120 = array4;
			this.hmac_sha1(expr_120, expr_120.Length);
			Array.Copy(array4, 22, array3, 0, 10);
			if (Utils.FindStr(encryptdata, 76, array3) != 33)
			{
				throw new ObfsException("ClientDecode data error: wrong sha1");
			}
			outlength = 0;
			needsendback = true;
			return encryptdata;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x0000F328 File Offset: 0x0000D528
		public override byte[] ClientEncode(byte[] encryptdata, int datalength, out int outlength)
		{
			if (this.handshake_status == -1)
			{
				this.SentLength += (long)datalength;
				outlength = datalength;
				return encryptdata;
			}
			byte[] array = new byte[datalength + 4096];
			if (this.handshake_status == 8)
			{
				array[0] = 23;
				array[1] = 3;
				array[2] = 3;
				array[3] = (byte)(datalength >> 8);
				array[4] = (byte)datalength;
				outlength = datalength + 5;
				Array.Copy(encryptdata, 0, array, 5, datalength);
			}
			else if (this.handshake_status == 1)
			{
				outlength = 0;
				if (datalength > 0)
				{
					byte[] array2 = new byte[datalength + 5];
					array2[0] = 23;
					array2[1] = 3;
					array2[2] = 3;
					array2[3] = (byte)(datalength >> 8);
					array2[4] = (byte)datalength;
					Array.Copy(encryptdata, 0, array2, 5, datalength);
					this.data_sent_buffer.Add(array2);
				}
				else
				{
					byte[] array3 = new byte[43];
					byte[] array4 = new byte[22];
					this.random.NextBytes(array4);
					byte[] bytes = Encoding.ASCII.GetBytes("\u0014\u0003\u0003\0\u0001\u0001\u0016\u0003\u0003\0 ");
					bytes.CopyTo(array3, 0);
					array4.CopyTo(array3, bytes.Length);
					byte[] expr_F3 = array3;
					this.hmac_sha1(expr_F3, expr_F3.Length);
					this.data_sent_buffer.Insert(0, array3);
					this.SentLength -= (long)array3.Length;
					foreach (byte[] current in this.data_sent_buffer)
					{
						while (array.Length < outlength + current.Length)
						{
							Array.Resize<byte>(ref array, array.Length * 2);
						}
						Array.Copy(current, 0, array, outlength, current.Length);
						this.SentLength += (long)current.Length;
						outlength += current.Length;
					}
					this.data_sent_buffer.Clear();
					this.handshake_status = 8;
				}
			}
			else
			{
				byte[] array5 = new byte[32];
				this.PackAuthData(array5);
				List<byte> list = new List<byte>();
				List<byte> list2 = new List<byte>();
				string str = "001cc02bc02fcca9cca8cc14cc13c00ac014c009c013009c0035002f000a0100";
				list.AddRange(array5);
				list.Add(32);
				list.AddRange(((TlsAuthData)this.Server.data).clientID);
				list.AddRange(this.to_bin(str));
				str = "ff01000100";
				list2.AddRange(this.to_bin(str));
				string text = this.Server.host;
				if (this.Server.param.Length > 0)
				{
					string[] array6 = this.Server.param.Split(new char[]
					{
						','
					});
					text = array6[this.random.Next(array6.Length)];
				}
				if (text != null && text.Length > 0)
				{
					string expr_280 = text;
					if (expr_280[expr_280.Length - 1] >= '0')
					{
						string expr_293 = text;
						if (expr_293[expr_293.Length - 1] <= '9' && this.Server.param.Length == 0)
						{
							text = "";
						}
					}
				}
				list2.AddRange(this.sni(text));
				string str2 = "00170000002300d0";
				list2.AddRange(this.to_bin(str2));
				byte[] array7 = new byte[208];
				TlsTicketAuthObfs.g_random.GetBytes(array7);
				list2.AddRange(array7);
				string text2 = "000d0016001406010603050105030401040303010303020102030005000501000000000012000075500000000b00020100000a0006000400170018";
				text2 += "00150066000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
				list2.AddRange(this.to_bin(text2));
				list2.Insert(0, (byte)(list2.Count % 256));
				list2.Insert(0, (byte)((list2.Count - 1) / 256));
				list.AddRange(list2);
				list.Insert(0, 3);
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
				for (int i = 0; i < list.Count; i++)
				{
					array[i] = list[i];
				}
				outlength = list.Count;
				byte[] array8 = new byte[datalength + 5];
				array8[0] = 23;
				array8[1] = 3;
				array8[2] = 3;
				array8[3] = (byte)(datalength >> 8);
				array8[4] = (byte)datalength;
				Array.Copy(encryptdata, 0, array8, 5, datalength);
				this.data_sent_buffer.Add(array8);
				this.handshake_status = 1;
			}
			return array;
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00009AEF File Offset: 0x00007CEF
		public override void Dispose()
		{
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x0000F0FF File Offset: 0x0000D2FF
		public override Dictionary<string, int[]> GetObfs()
		{
			return TlsTicketAuthObfs._obfs;
		}

		// Token: 0x060000FD RID: 253 RVA: 0x0000F1D8 File Offset: 0x0000D3D8
		protected void hmac_sha1(byte[] data, int length)
		{
			byte[] array = new byte[this.Server.key.Length + 32];
			this.Server.key.CopyTo(array, 0);
			((TlsAuthData)this.Server.data).clientID.CopyTo(array, this.Server.key.Length);
			Array.Copy(new HMACSHA1(array).ComputeHash(data, 0, length - 10), 0, data, length - 10, 10);
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x0000EB77 File Offset: 0x0000CD77
		public override object InitData()
		{
			return new TlsAuthData();
		}

		// Token: 0x060000FE RID: 254 RVA: 0x0000F254 File Offset: 0x0000D454
		public void PackAuthData(byte[] outdata)
		{
			int length = 32;
			byte[] array = new byte[18];
			TlsTicketAuthObfs.g_random.GetBytes(array);
			array.CopyTo(outdata, 4);
			TlsAuthData tlsAuthData = (TlsAuthData)this.Server.data;
			TlsAuthData obj = tlsAuthData;
			lock (obj)
			{
				if (tlsAuthData.clientID == null)
				{
					tlsAuthData.clientID = new byte[32];
					TlsTicketAuthObfs.g_random.GetBytes(tlsAuthData.clientID);
				}
			}
			byte[] expr_A0 = BitConverter.GetBytes((uint)((ulong)Math.Floor(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds)));
			Array.Reverse(expr_A0);
			Array.Copy(expr_A0, 0, outdata, 0, 4);
			this.hmac_sha1(outdata, length);
		}

		// Token: 0x060000FA RID: 250 RVA: 0x0000F108 File Offset: 0x0000D308
		protected byte[] sni(string url)
		{
			if (url == null)
			{
				url = "";
			}
			byte[] expr_15 = Encoding.UTF8.GetBytes(url);
			int num = expr_15.Length;
			byte[] array = new byte[num + 9];
			Array.Copy(expr_15, 0, array, 9, num);
			array[7] = (byte)(num >> 8);
			array[8] = (byte)num;
			num += 3;
			array[4] = (byte)(num >> 8);
			array[5] = (byte)num;
			num += 2;
			array[2] = (byte)(num >> 8);
			array[3] = (byte)num;
			return array;
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x0000F0EE File Offset: 0x0000D2EE
		public static List<string> SupportedObfs()
		{
			return new List<string>(TlsTicketAuthObfs._obfs.Keys);
		}

		// Token: 0x060000FC RID: 252 RVA: 0x0000F184 File Offset: 0x0000D384
		protected byte[] to_bin(string str)
		{
			byte[] array = new byte[str.Length / 2];
			for (int i = 0; i < str.Length; i += 2)
			{
				array[i / 2] = (byte)((int)this.to_val(str[i]) << 4 | (int)this.to_val(str[i + 1]));
			}
			return array;
		}

		// Token: 0x060000FB RID: 251 RVA: 0x0000F16F File Offset: 0x0000D36F
		protected byte to_val(char c)
		{
			if (c > '9')
			{
				return (byte)(c - 'a' + '\n');
			}
			return (byte)(c - '0');
		}

		// Token: 0x040000DD RID: 221
		private byte[] data_recv_buffer = new byte[0];

		// Token: 0x040000DC RID: 220
		private List<byte[]> data_sent_buffer = new List<byte[]>();

		// Token: 0x040000DE RID: 222
		protected static RNGCryptoServiceProvider g_random = new RNGCryptoServiceProvider();

		// Token: 0x040000DB RID: 219
		private int handshake_status;

		// Token: 0x040000DF RID: 223
		private Random random = new Random();

		// Token: 0x040000DA RID: 218
		private static Dictionary<string, int[]> _obfs = new Dictionary<string, int[]>
		{
			{
				"tls1.2_ticket_auth",
				new int[]
				{
					0,
					1,
					1
				}
			}
		};
	}
}
