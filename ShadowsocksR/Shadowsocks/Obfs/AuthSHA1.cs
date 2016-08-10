﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Shadowsocks.Util;

namespace Shadowsocks.Obfs
{
	// Token: 0x02000011 RID: 17
	public class AuthSHA1 : VerifySimpleBase
	{
		// Token: 0x060000CD RID: 205 RVA: 0x0000D730 File Offset: 0x0000B930
		public AuthSHA1(string method) : base(method)
		{
			this.has_sent_header = false;
			this.has_recv_header = false;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x0000DB14 File Offset: 0x0000BD14
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
				int num2 = (int)(this.recv_buf[2] + 2);
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

		// Token: 0x060000D3 RID: 211 RVA: 0x0000D9B8 File Offset: 0x0000BBB8
		public override byte[] ClientPreEncrypt(byte[] plaindata, int datalength, out int outlength)
		{
			int expr_01 = datalength;
			byte[] array = new byte[expr_01 + expr_01 / 10 + 32];
			byte[] array2 = new byte[9000];
			byte[] array3 = plaindata;
			outlength = 0;
			if (!this.has_sent_header)
			{
				int headSize = ObfsBase.GetHeadSize(plaindata, 30);
				int num = Math.Min(this.random.Next(32) + headSize, datalength);
				int num2;
				this.PackAuthData(array3, num, array2, out num2);
				this.has_sent_header = true;
				if (array.Length < outlength + num2)
				{
					Array.Resize<byte>(ref array, (outlength + num2) * 2);
				}
				Array.Copy(array2, 0, array, outlength, num2);
				outlength += num2;
				datalength -= num;
				byte[] array4 = new byte[datalength];
				Array.Copy(array3, num, array4, 0, array4.Length);
				array3 = array4;
			}
			while (datalength > 8100)
			{
				int num3;
				this.PackData(array3, 8100, array2, out num3);
				if (array.Length < outlength + num3)
				{
					Array.Resize<byte>(ref array, (outlength + num3) * 2);
				}
				Array.Copy(array2, 0, array, outlength, num3);
				outlength += num3;
				datalength -= 8100;
				byte[] array5 = new byte[datalength];
				Array.Copy(array3, 8100, array5, 0, array5.Length);
				array3 = array5;
			}
			if (datalength > 0)
			{
				int num4;
				this.PackData(array3, datalength, array2, out num4);
				if (array.Length < outlength + num4)
				{
					Array.Resize<byte>(ref array, (outlength + num4) * 2);
				}
				Array.Copy(array2, 0, array, outlength, num4);
				outlength += num4;
			}
			return array;
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00009AEF File Offset: 0x00007CEF
		public override void Dispose()
		{
		}

		// Token: 0x060000CF RID: 207 RVA: 0x0000D758 File Offset: 0x0000B958
		public override Dictionary<string, int[]> GetObfs()
		{
			return AuthSHA1._obfs;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x0000D2DC File Offset: 0x0000B4DC
		public override object InitData()
		{
			return new AuthData();
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x0000D7C8 File Offset: 0x0000B9C8
		public void PackAuthData(byte[] data, int datalength, byte[] outdata, out int outlength)
		{
			int num = this.random.Next(250) + 1;
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
					authData.clientID = new byte[4];
					AuthSHA1.g_random.GetBytes(authData.clientID);
					authData.connectionID = (uint)this.random.Next(16777216);
				}
				authData.connectionID += 1u;
				Array.Copy(authData.clientID, 0, outdata, num2 + 4, 4);
				Array.Copy(BitConverter.GetBytes(authData.connectionID), 0, outdata, num2 + 8, 4);
			}
			Array.Copy(BitConverter.GetBytes((uint)((ulong)Math.Floor(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds))), 0, outdata, num2, 4);
			Array.Copy(data, 0, outdata, num2 + 12, datalength);
			outdata[4] = (byte)(outlength >> 8);
			outdata[5] = (byte)outlength;
			outdata[6] = (byte)num;
			BitConverter.GetBytes((uint)CRC32.CalcCRC32(this.Server.key, this.Server.key.Length, unchecked ( (ulong)-1))).CopyTo(outdata, 0);
			byte[] array = new byte[this.Server.iv.Length + this.Server.key.Length];
			this.Server.iv.CopyTo(array, 0);
			this.Server.key.CopyTo(array, this.Server.iv.Length);
			Array.Copy(new HMACSHA1(array).ComputeHash(outdata, 0, outlength - 10), 0, outdata, outlength - 10, 10);
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x0000D760 File Offset: 0x0000B960
		public void PackData(byte[] data, int datalength, byte[] outdata, out int outlength)
		{
			int num = this.random.Next(16) + 1;
			outlength = num + datalength + 6;
			if (datalength > 0)
			{
				Array.Copy(data, 0, outdata, num + 2, datalength);
			}
			outdata[0] = (byte)(outlength >> 8);
			outdata[1] = (byte)outlength;
			outdata[2] = (byte)num;
			BitConverter.GetBytes((uint)Adler32.CalcAdler32(outdata, outlength - 4)).CopyTo(outdata, outlength - 4);
		}

		// Token: 0x060000CE RID: 206 RVA: 0x0000D747 File Offset: 0x0000B947
		public static List<string> SupportedObfs()
		{
			return new List<string>(AuthSHA1._obfs.Keys);
		}

		// Token: 0x040000C4 RID: 196
		protected static RNGCryptoServiceProvider g_random = new RNGCryptoServiceProvider();

		// Token: 0x040000C3 RID: 195
		protected bool has_recv_header;

		// Token: 0x040000C2 RID: 194
		protected bool has_sent_header;

		// Token: 0x040000C1 RID: 193
		private static Dictionary<string, int[]> _obfs = new Dictionary<string, int[]>
		{
			{
				"auth_sha1",
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
