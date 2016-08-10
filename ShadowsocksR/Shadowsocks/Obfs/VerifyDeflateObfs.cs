using System;
using System.Collections.Generic;
using Shadowsocks.Controller;
using Shadowsocks.Util;

namespace Shadowsocks.Obfs
{
	// Token: 0x02000020 RID: 32
	public class VerifyDeflateObfs : VerifySimpleBase
	{
		// Token: 0x06000137 RID: 311 RVA: 0x0000FE87 File Offset: 0x0000E087
		public VerifyDeflateObfs(string method) : base(method)
		{
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00010258 File Offset: 0x0000E458
		public override byte[] ClientPostDecrypt(byte[] plaindata, int datalength, out int outlength)
		{
			byte[] array = new byte[this.recv_buf_len + datalength * 2 + 16];
			Array.Copy(plaindata, 0, this.recv_buf, this.recv_buf_len, datalength);
			this.recv_buf_len += datalength;
			outlength = 0;
			while (this.recv_buf_len > 2)
			{
				int num = ((int)this.recv_buf[0] << 8) + (int)this.recv_buf[1];
				if (num >= 32768 || num < 6)
				{
					throw new ObfsException("ClientPostDecrypt data error");
				}
				if (num > this.recv_buf_len)
				{
					break;
				}
				int num2;
				byte[] array2 = FileManager.DeflateDecompress(this.recv_buf, 2, num - 6, out num2);
				if (array2 == null)
				{
					throw new ObfsException("ClientPostDecrypt data decompress ERROR");
				}
				ulong num3 = Adler32.CalcAdler32(array2, num2);
				if (this.recv_buf[num - 4] != (byte)(num3 >> 24) || this.recv_buf[num - 3] != (byte)(num3 >> 16) || this.recv_buf[num - 2] != (byte)(num3 >> 8) || this.recv_buf[num - 1] != (byte)num3)
				{
					throw new ObfsException("ClientPostDecrypt data decompress ERROR");
				}
				if (outlength + num2 > array.Length)
				{
					Array.Resize<byte>(ref array, (outlength + num2) * 2);
				}
				Array.Copy(array2, 0, array, outlength, num2);
				outlength += num2;
				this.recv_buf_len -= num;
				Array.Copy(this.recv_buf, num, this.recv_buf, 0, this.recv_buf_len);
				num = ((int)this.recv_buf[0] << 8) + (int)this.recv_buf[1];
			}
			return array;
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00010188 File Offset: 0x0000E388
		public override byte[] ClientPreEncrypt(byte[] plaindata, int datalength, out int outlength)
		{
			int expr_01 = datalength;
			byte[] array = new byte[expr_01 + expr_01 / 10 + 32];
			byte[] array2 = new byte[32768];
			byte[] array3 = plaindata;
			outlength = 0;
			while (datalength > 32700)
			{
				int num;
				this.PackData(array3, 32700, array2, out num);
				if (array.Length < outlength + num)
				{
					Array.Resize<byte>(ref array, (outlength + num) * 2);
				}
				Array.Copy(array2, 0, array, outlength, num);
				outlength += num;
				datalength -= 32700;
				byte[] array4 = new byte[datalength];
				Array.Copy(array3, 32700, array4, 0, array4.Length);
				array3 = array4;
			}
			if (datalength > 0)
			{
				int num2;
				this.PackData(array3, datalength, array2, out num2);
				if (array.Length < outlength + num2)
				{
					Array.Resize<byte>(ref array, (outlength + num2) * 2);
				}
				Array.Copy(array2, 0, array, outlength, num2);
				outlength += num2;
			}
			return array;
		}

		// Token: 0x0600013D RID: 317 RVA: 0x00009AEF File Offset: 0x00007CEF
		public override void Dispose()
		{
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00010113 File Offset: 0x0000E313
		public override Dictionary<string, int[]> GetObfs()
		{
			return VerifyDeflateObfs._obfs;
		}

		// Token: 0x0600013A RID: 314 RVA: 0x0001011C File Offset: 0x0000E31C
		public void PackData(byte[] data, int datalength, byte[] outdata, out int outlength)
		{
			int num;
			Array arg_26_0 = FileManager.DeflateCompress(data, 0, datalength, out num);
			outlength = num + 2 + 4;
			outdata[0] = (byte)(outlength >> 8);
			outdata[1] = (byte)outlength;
			Array.Copy(arg_26_0, 0, outdata, 2, num);
			ulong num2 = Adler32.CalcAdler32(data, datalength);
			outdata[outlength - 4] = (byte)(num2 >> 24);
			outdata[outlength - 3] = (byte)(num2 >> 16);
			outdata[outlength - 2] = (byte)(num2 >> 8);
			outdata[outlength - 1] = (byte)num2;
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00010102 File Offset: 0x0000E302
		public static List<string> SupportedObfs()
		{
			return new List<string>(VerifyDeflateObfs._obfs.Keys);
		}

		// Token: 0x040000F3 RID: 243
		private static Dictionary<string, int[]> _obfs = new Dictionary<string, int[]>
		{
			{
				"verify_deflate",
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
