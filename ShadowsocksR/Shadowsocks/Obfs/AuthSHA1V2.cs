using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Shadowsocks.Util;

namespace Shadowsocks.Obfs
{
	// Token: 0x02000012 RID: 18
	public class AuthSHA1V2 : VerifySimpleBase
	{
		// Token: 0x060000D7 RID: 215 RVA: 0x0000DC58 File Offset: 0x0000BE58
		public AuthSHA1V2(string method) : base(method)
		{
			this.has_sent_header = false;
			this.has_recv_header = false;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x0000E0D8 File Offset: 0x0000C2D8
		public override byte[] ClientPostDecrypt(byte[] plaindata, int datalength, out int outlength)
		{
			byte[] array = new byte[this.recv_buf_len + datalength];
			Array.Copy(plaindata, 0, this.recv_buf, this.recv_buf_len, datalength);
			this.recv_buf_len += datalength;
			outlength = 0;
			while (this.recv_buf_len > 2)
			{
				int num = ((int)this.recv_buf[0] << 8) + (int)this.recv_buf[1];
				if (num >= 8192 || num < 7)
				{
					throw new ObfsException("ClientPostDecrypt data error");
				}
				if (num > this.recv_buf_len)
				{
					break;
				}
				if (!Adler32.CheckAdler32(this.recv_buf, num))
				{
					throw new ObfsException("ClientPostDecrypt data uncorrect checksum");
				}
				int num2 = (int)this.recv_buf[2];
				if (num2 < 255)
				{
					num2 += 2;
				}
				else
				{
					num2 = ((int)this.recv_buf[3] << 8 | (int)this.recv_buf[4]) + 2;
				}
				int num3 = num - num2 - 4;
				if (outlength + num3 > array.Length)
				{
					Array.Resize<byte>(ref array, (outlength + num3) * 2);
				}
				Array.Copy(this.recv_buf, num2, array, outlength, num3);
				outlength += num3;
				this.recv_buf_len -= num;
				Array.Copy(this.recv_buf, num, this.recv_buf, 0, this.recv_buf_len);
			}
			return array;
		}

		// Token: 0x060000DD RID: 221 RVA: 0x0000DF6C File Offset: 0x0000C16C
		public override byte[] ClientPreEncrypt(byte[] plaindata, int datalength, out int outlength)
		{
			int expr_01 = datalength;
			byte[] array = new byte[expr_01 + expr_01 / 10 + 32];
			byte[] array2 = new byte[9000];
			byte[] array3 = plaindata;
			outlength = 0;
			int num = datalength;
			if (!this.has_sent_header)
			{
				int headSize = ObfsBase.GetHeadSize(plaindata, 30);
				int num2 = Math.Min(this.random.Next(32) + headSize, datalength);
				int num3;
				this.PackAuthData(array3, num2, array2, out num3);
				this.has_sent_header = true;
				if (array.Length < outlength + num3)
				{
					Array.Resize<byte>(ref array, (outlength + num3) * 2);
				}
				Array.Copy(array2, 0, array, outlength, num3);
				outlength += num3;
				datalength -= num2;
				byte[] array4 = new byte[datalength];
				Array.Copy(array3, num2, array4, 0, array4.Length);
				array3 = array4;
			}
			while (datalength > 8100)
			{
				int num4;
				this.PackData(array3, 8100, array2, out num4);
				if (array.Length < outlength + num4)
				{
					Array.Resize<byte>(ref array, (outlength + num4) * 2);
				}
				Array.Copy(array2, 0, array, outlength, num4);
				outlength += num4;
				datalength -= 8100;
				byte[] array5 = new byte[datalength];
				Array.Copy(array3, 8100, array5, 0, array5.Length);
				array3 = array5;
			}
			if (datalength > 0 || num == -1)
			{
				if (num == -1)
				{
					datalength = 0;
				}
				int num5;
				this.PackData(array3, datalength, array2, out num5);
				if (array.Length < outlength + num5)
				{
					Array.Resize<byte>(ref array, (outlength + num5) * 2);
				}
				Array.Copy(array2, 0, array, outlength, num5);
				outlength += num5;
			}
			return array;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00009AEF File Offset: 0x00007CEF
		public override void Dispose()
		{
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x0000DC80 File Offset: 0x0000BE80
		public override Dictionary<string, int[]> GetObfs()
		{
			return AuthSHA1V2._obfs;
		}

		// Token: 0x060000DA RID: 218 RVA: 0x0000D2DC File Offset: 0x0000B4DC
		public override object InitData()
		{
			return new AuthData();
		}

		// Token: 0x060000DC RID: 220 RVA: 0x0000DD38 File Offset: 0x0000BF38
		public void PackAuthData(byte[] data, int datalength, byte[] outdata, out int outlength)
		{
			int num = (datalength > 1300) ? 1 : ((datalength > 400) ? (this.random.Next(128) + 1) : (this.random.Next(1024) + 3));
			int num2 = num + 4 + 2;
			outlength = num2 + datalength + 12 + 10;
			AuthData authData = (AuthData)this.Server.data;
			AuthData obj = authData;
			lock (obj)
			{
				if (authData.connectionID > 4278190080u)
				{
					authData.clientID = null;
				}
				if (authData.clientID == null)
				{
					authData.clientID = new byte[8];
					AuthSHA1V2.g_random.GetBytes(authData.clientID);
					authData.connectionID = (uint)BitConverter.ToInt64(authData.clientID, 0) % 16777213u;
				}
				authData.connectionID += 1u;
				Array.Copy(authData.clientID, 0, outdata, num2, 8);
				Array.Copy(BitConverter.GetBytes(authData.connectionID), 0, outdata, num2 + 8, 4);
			}
			Array.Copy(data, 0, outdata, num2 + 12, datalength);
			outdata[4] = (byte)(outlength >> 8);
			outdata[5] = (byte)outlength;
			if (num < 128)
			{
				outdata[6] = (byte)num;
			}
			else
			{
				outdata[6] = 255;
				outdata[7] = (byte)(num >> 8);
				outdata[8] = (byte)num;
			}
			byte[] bytes = Encoding.UTF8.GetBytes("auth_sha1_v2");
			byte[] array = new byte[bytes.Length + this.Server.key.Length];
			bytes.CopyTo(array, 0);
			this.Server.key.CopyTo(array, bytes.Length);
			byte[] expr_186 = array;
			BitConverter.GetBytes((uint)CRC32.CalcCRC32(expr_186, expr_186.Length,  unchecked ((ulong)-1))).CopyTo(outdata, 0);
			byte[] array2 = new byte[this.Server.iv.Length + this.Server.key.Length];
			this.Server.iv.CopyTo(array2, 0);
			this.Server.key.CopyTo(array2, this.Server.iv.Length);
			Array.Copy(new HMACSHA1(array2).ComputeHash(outdata, 0, outlength - 10), 0, outdata, outlength - 10, 10);
		}

		// Token: 0x060000DB RID: 219 RVA: 0x0000DC88 File Offset: 0x0000BE88
		public void PackData(byte[] data, int datalength, byte[] outdata, out int outlength)
		{
			int num = (datalength >= 1300) ? 1 : ((datalength > 400) ? (this.random.Next(128) + 1) : (this.random.Next(1024) + 3));
			outlength = num + datalength + 6;
			if (datalength > 0)
			{
				Array.Copy(data, 0, outdata, num + 2, datalength);
			}
			outdata[0] = (byte)(outlength >> 8);
			outdata[1] = (byte)outlength;
			if (num < 128)
			{
				outdata[2] = (byte)num;
			}
			else
			{
				outdata[2] = 255;
				outdata[3] = (byte)(num >> 8);
				outdata[4] = (byte)num;
			}
			BitConverter.GetBytes((uint)Adler32.CalcAdler32(outdata, outlength - 4)).CopyTo(outdata, outlength - 4);
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x0000DC6F File Offset: 0x0000BE6F
		public static List<string> SupportedObfs()
		{
			return new List<string>(AuthSHA1V2._obfs.Keys);
		}

		// Token: 0x040000C8 RID: 200
		protected static RNGCryptoServiceProvider g_random = new RNGCryptoServiceProvider();

		// Token: 0x040000C7 RID: 199
		protected bool has_recv_header;

		// Token: 0x040000C6 RID: 198
		protected bool has_sent_header;

		// Token: 0x040000C5 RID: 197
		private static Dictionary<string, int[]> _obfs = new Dictionary<string, int[]>
		{
			{
				"auth_sha1_v2",
				new int[]
				{
					1,
					0,
					1
				}
			}
		};
	}
}
