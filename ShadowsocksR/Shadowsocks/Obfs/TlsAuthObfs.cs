using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Shadowsocks.Util;

namespace Shadowsocks.Obfs
{
	// Token: 0x02000015 RID: 21
	internal class TlsAuthObfs : ObfsBase
	{
		// Token: 0x060000F5 RID: 245 RVA: 0x0000F084 File Offset: 0x0000D284
		static TlsAuthObfs()
		{
			// Note: this type is marked as 'beforefieldinit'.
			Dictionary<string, int[]> dictionary = new Dictionary<string, int[]>();
			Dictionary<string, int[]> arg_16_0 = dictionary;
			string arg_16_1 = "tls1.0_session_auth";
			int[] expr_12 = new int[3];
			expr_12[1] = 1;
			arg_16_0.Add(arg_16_1, expr_12);
			TlsAuthObfs._obfs = dictionary;
			TlsAuthObfs.g_random = new RNGCryptoServiceProvider();
		}

		// Token: 0x060000EC RID: 236 RVA: 0x0000EB2B File Offset: 0x0000CD2B
		public TlsAuthObfs(string method) : base(method)
		{
			this.has_sent_header = false;
			this.raw_trans_sent = false;
			this.raw_trans_recv = false;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x0000EFF4 File Offset: 0x0000D1F4
		public override byte[] ClientDecode(byte[] encryptdata, int datalength, out int outlength, out bool needsendback)
		{
			if (this.raw_trans_recv)
			{
				outlength = datalength;
				needsendback = false;
				return encryptdata;
			}
			//new byte[datalength];
			if (datalength < 76)
			{
				throw new ObfsException("ClientDecode data error");
			}
			byte[] array = new byte[10];
			byte[] array2 = new byte[32];
			Array.Copy(encryptdata, 11, array2, 0, 22);
			byte[] expr_46 = array2;
			this.hmac_sha1(expr_46, expr_46.Length);
			Array.Copy(array2, 22, array, 0, 10);
			if (Utils.FindStr(encryptdata, 76, array) != 33)
			{
				throw new ObfsException("ClientDecode data error: wrong sha1");
			}
			outlength = 0;
			this.raw_trans_recv = true;
			needsendback = true;
			return encryptdata;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x0000ECD0 File Offset: 0x0000CED0
		public override byte[] ClientEncode(byte[] encryptdata, int datalength, out int outlength)
		{
			if (this.raw_trans_sent)
			{
				this.SentLength += (long)datalength;
				outlength = datalength;
				return encryptdata;
			}
			byte[] array = new byte[datalength + 4096];
			if (this.has_sent_header)
			{
				outlength = 0;
				if (datalength > 0)
				{
					byte[] array2 = new byte[datalength];
					Array.Copy(encryptdata, array2, datalength);
					this.data_buffer.Add(array2);
				}
				else
				{
					byte[] array3 = new byte[43];
					byte[] array4 = new byte[22];
					this.random.NextBytes(array4);
					byte[] bytes = Encoding.ASCII.GetBytes("\u0014\u0003\u0001\0\u0001\u0001\u0016\u0003\u0001\0 ");
					bytes.CopyTo(array3, 0);
					array4.CopyTo(array3, bytes.Length);
					byte[] expr_9E = array3;
					this.hmac_sha1(expr_9E, expr_9E.Length);
					this.data_buffer.Insert(0, array3);
					this.SentLength -= (long)array3.Length;
					foreach (byte[] current in this.data_buffer)
					{
						while (array.Length < outlength + current.Length)
						{
							Array.Resize<byte>(ref array, array.Length * 2);
						}
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
				byte[] array5 = new byte[32];
				this.PackAuthData(array5);
				List<byte> list = new List<byte>();
				string text = "0016c02bc02fc00ac009c013c01400330039002f0035000a0100006fff01000100000a00080006001700180019000b0002010000230000337400000010002900270568322d31360568322d31350568322d313402683208737064792f332e3108687474702f312e31000500050100000000000d001600140401050106010201040305030603020304020202";
				byte[] array6 = array5;
				for (int i = 0; i < array6.Length; i++)
				{
					byte item = array6[i];
					list.Add(item);
				}
				list.Add(32);
				array6 = ((TlsAuthData)this.Server.data).clientID;
				for (int i = 0; i < array6.Length; i++)
				{
					byte item2 = array6[i];
					list.Add(item2);
				}
				for (int j = 0; j < text.Length; j += 2)
				{
					byte item3 = (byte)(text[j] | text[j + 1] * '\u0010');
					list.Add(item3);
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
			this.has_sent_header = true;
			return array;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00009AEF File Offset: 0x00007CEF
		public override void Dispose()
		{
		}

		// Token: 0x060000EE RID: 238 RVA: 0x0000EB70 File Offset: 0x0000CD70
		public override Dictionary<string, int[]> GetObfs()
		{
			return TlsAuthObfs._obfs;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x0000EB80 File Offset: 0x0000CD80
		protected void hmac_sha1(byte[] data, int length)
		{
			byte[] array = new byte[this.Server.key.Length + 32];
			this.Server.key.CopyTo(array, 0);
			((TlsAuthData)this.Server.data).clientID.CopyTo(array, this.Server.key.Length);
			Array.Copy(new HMACSHA1(array).ComputeHash(data, 0, length - 10), 0, data, length - 10, 10);
		}

		// Token: 0x060000EF RID: 239 RVA: 0x0000EB77 File Offset: 0x0000CD77
		public override object InitData()
		{
			return new TlsAuthData();
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x0000EBFC File Offset: 0x0000CDFC
		public void PackAuthData(byte[] outdata)
		{
			int length = 32;
			byte[] array = new byte[18];
			TlsAuthObfs.g_random.GetBytes(array);
			array.CopyTo(outdata, 4);
			TlsAuthData tlsAuthData = (TlsAuthData)this.Server.data;
			TlsAuthData obj = tlsAuthData;
			lock (obj)
			{
				if (tlsAuthData.clientID == null)
				{
					tlsAuthData.clientID = new byte[32];
					TlsAuthObfs.g_random.GetBytes(tlsAuthData.clientID);
				}
			}
			byte[] expr_A0 = BitConverter.GetBytes((uint)((ulong)Math.Floor(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds)));
			Array.Reverse(expr_A0);
			Array.Copy(expr_A0, 0, outdata, 0, 4);
			this.hmac_sha1(outdata, length);
		}

		// Token: 0x060000ED RID: 237 RVA: 0x0000EB5F File Offset: 0x0000CD5F
		public static List<string> SupportedObfs()
		{
			return new List<string>(TlsAuthObfs._obfs.Keys);
		}

		// Token: 0x040000D7 RID: 215
		private List<byte[]> data_buffer = new List<byte[]>();

		// Token: 0x040000D8 RID: 216
		protected static RNGCryptoServiceProvider g_random;

		// Token: 0x040000D4 RID: 212
		private bool has_sent_header;

		// Token: 0x040000D9 RID: 217
		private Random random = new Random();

		// Token: 0x040000D6 RID: 214
		private bool raw_trans_recv;

		// Token: 0x040000D5 RID: 213
		private bool raw_trans_sent;

		// Token: 0x040000D3 RID: 211
		private static Dictionary<string, int[]> _obfs;
	}
}
