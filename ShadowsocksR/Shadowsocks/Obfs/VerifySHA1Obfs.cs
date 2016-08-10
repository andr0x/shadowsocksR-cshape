using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Shadowsocks.Obfs
{
	// Token: 0x02000021 RID: 33
	public class VerifySHA1Obfs : VerifySimpleBase
	{
		// Token: 0x0600013F RID: 319 RVA: 0x000103FA File Offset: 0x0000E5FA
		public VerifySHA1Obfs(string method) : base(method)
		{
			this.has_sent_header = false;
			this.pack_id = 0u;
		}

		// Token: 0x06000145 RID: 325 RVA: 0x0000F9D0 File Offset: 0x0000DBD0
		public override byte[] ClientPostDecrypt(byte[] plaindata, int datalength, out int outlength)
		{
			outlength = datalength;
			return plaindata;
		}

		// Token: 0x06000144 RID: 324 RVA: 0x00010568 File Offset: 0x0000E768
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

		// Token: 0x06000146 RID: 326 RVA: 0x000106A8 File Offset: 0x0000E8A8
		public override byte[] ClientUdpPreEncrypt(byte[] plaindata, int datalength, out int outlength)
		{
			byte[] array = new byte[datalength + 10];
			outlength = datalength + 10;
			int num;
			this.PackAuthData(plaindata, datalength, array, out num);
			return array;
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00009AEF File Offset: 0x00007CEF
		public override void Dispose()
		{
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00010422 File Offset: 0x0000E622
		public override Dictionary<string, int[]> GetObfs()
		{
			return VerifySHA1Obfs._obfs;
		}

		// Token: 0x06000143 RID: 323 RVA: 0x000104D0 File Offset: 0x0000E6D0
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

		// Token: 0x06000142 RID: 322 RVA: 0x0001042C File Offset: 0x0000E62C
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

		// Token: 0x06000140 RID: 320 RVA: 0x00010411 File Offset: 0x0000E611
		public static List<string> SupportedObfs()
		{
			return new List<string>(VerifySHA1Obfs._obfs.Keys);
		}

		// Token: 0x040000F5 RID: 245
		private bool has_sent_header;

		// Token: 0x040000F6 RID: 246
		private uint pack_id;

		// Token: 0x040000F4 RID: 244
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
