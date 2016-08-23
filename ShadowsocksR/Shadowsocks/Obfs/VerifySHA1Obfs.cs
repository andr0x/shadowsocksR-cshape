using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Shadowsocks.Obfs
{
	// Token: 0x0200001F RID: 31
	public class VerifySHA1Obfs : VerifySimpleBase
	{
		// Token: 0x0600012B RID: 299 RVA: 0x0000F9B6 File Offset: 0x0000DBB6
		public VerifySHA1Obfs(string method) : base(method)
		{
			this.has_sent_header = false;
			this.pack_id = 0u;
		}

		// Token: 0x06000131 RID: 305 RVA: 0x0000F02C File Offset: 0x0000D22C
		public override byte[] ClientPostDecrypt(byte[] plaindata, int datalength, out int outlength)
		{
			outlength = datalength;
			return plaindata;
		}

		// Token: 0x06000130 RID: 304 RVA: 0x0000FB24 File Offset: 0x0000DD24
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
				int num;
				this.PackAuthData(array3, headSize, array2, out num);
				this.has_sent_header = true;
				if (array.Length < outlength + num)
				{
					Array.Resize<byte>(ref array, (outlength + num) * 2);
				}
				Array.Copy(array2, 0, array, outlength, num);
				outlength += num;
				datalength -= headSize;
				byte[] array4 = new byte[datalength];
				Array.Copy(array3, headSize, array4, 0, array4.Length);
				array3 = array4;
			}
			while (datalength > 8100)
			{
				int num2;
				this.PackData(array3, 8100, array2, out num2);
				if (array.Length < outlength + num2)
				{
					Array.Resize<byte>(ref array, (outlength + num2) * 2);
				}
				Array.Copy(array2, 0, array, outlength, num2);
				outlength += num2;
				datalength -= 8100;
				byte[] array5 = new byte[datalength];
				Array.Copy(array3, 8100, array5, 0, array5.Length);
				array3 = array5;
			}
			if (datalength > 0)
			{
				int num3;
				this.PackData(array3, datalength, array2, out num3);
				if (array.Length < outlength + num3)
				{
					Array.Resize<byte>(ref array, (outlength + num3) * 2);
				}
				Array.Copy(array2, 0, array, outlength, num3);
				outlength += num3;
			}
			return array;
		}

		// Token: 0x06000132 RID: 306 RVA: 0x0000FC64 File Offset: 0x0000DE64
		public override byte[] ClientUdpPreEncrypt(byte[] plaindata, int datalength, out int outlength)
		{
			byte[] array = new byte[datalength + 10];
			outlength = datalength + 10;
			int num;
			this.PackAuthData(plaindata, datalength, array, out num);
			return array;
		}

		// Token: 0x06000133 RID: 307 RVA: 0x00009C9F File Offset: 0x00007E9F
		public override void Dispose()
		{
		}

		// Token: 0x0600012D RID: 301 RVA: 0x0000F9DE File Offset: 0x0000DBDE
		public override Dictionary<string, int[]> GetObfs()
		{
			return VerifySHA1Obfs._obfs;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x0000FA8C File Offset: 0x0000DC8C
		public void PackAuthData(byte[] data, int datalength, byte[] outdata, out int outlength)
		{
			byte[] array = new byte[this.Server.iv.Length + this.Server.key.Length];
			this.Server.iv.CopyTo(array, 0);
			this.Server.key.CopyTo(array, this.Server.iv.Length);
			Array.Copy(data, 0, outdata, 0, datalength);
			int expr_62_cp_1 = 0;
			outdata[expr_62_cp_1] |= 16;
			Array.Copy(new HMACSHA1(array).ComputeHash(outdata, 0, datalength), 0, outdata, datalength, 10);
			outlength = datalength + 10;
		}

		// Token: 0x0600012E RID: 302 RVA: 0x0000F9E8 File Offset: 0x0000DBE8
		public void PackData(byte[] data, int datalength, byte[] outdata, out int outlength)
		{
			byte[] array = new byte[this.Server.iv.Length + 4];
			this.Server.iv.CopyTo(array, 0);
			byte[] arg_40_0 = BitConverter.GetBytes(this.pack_id);
			this.pack_id += 1u;
			Array.Reverse(arg_40_0);
			arg_40_0.CopyTo(array, this.Server.iv.Length);
			Array.Copy(data, 0, outdata, 12, datalength);
			Array.Copy(new HMACSHA1(array).ComputeHash(data, 0, datalength), 0, outdata, 2, 10);
			outdata[0] = (byte)(datalength >> 8);
			outdata[1] = (byte)(datalength & 255);
			outlength = datalength + 12;
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000F9CD File Offset: 0x0000DBCD
		public static List<string> SupportedObfs()
		{
			return new List<string>(VerifySHA1Obfs._obfs.Keys);
		}

		// Token: 0x040000EA RID: 234
		private bool has_sent_header;

		// Token: 0x040000EB RID: 235
		private uint pack_id;

		// Token: 0x040000E9 RID: 233
		private static Dictionary<string, int[]> _obfs = new Dictionary<string, int[]>
		{
			{
				"verify_sha1",
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
