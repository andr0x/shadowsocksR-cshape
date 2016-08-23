using System;
using System.Collections.Generic;
using Shadowsocks.Util;

namespace Shadowsocks.Obfs
{
	// Token: 0x0200001D RID: 29
	public class VerifySimpleObfs : VerifySimpleBase
	{
		// Token: 0x0600011B RID: 283 RVA: 0x0000F443 File Offset: 0x0000D643
		public VerifySimpleObfs(string method) : base(method)
		{
		}

		// Token: 0x06000120 RID: 288 RVA: 0x0000F584 File Offset: 0x0000D784
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
				if (!CRC32.CheckCRC32(this.recv_buf, num))
				{
					throw new ObfsException("ClientPostDecrypt data uncorrect CRC32");
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

		// Token: 0x0600011F RID: 287 RVA: 0x0000F4B4 File Offset: 0x0000D6B4
		public override byte[] ClientPreEncrypt(byte[] plaindata, int datalength, out int outlength)
		{
			int expr_01 = datalength;
			byte[] array = new byte[expr_01 + expr_01 / 10 + 32];
			byte[] array2 = new byte[9000];
			byte[] array3 = plaindata;
			outlength = 0;
			while (datalength > 8100)
			{
				int num;
				this.PackData(array3, 8100, array2, out num);
				if (array.Length < outlength + num)
				{
					Array.Resize<byte>(ref array, (outlength + num) * 2);
				}
				Array.Copy(array2, 0, array, outlength, num);
				outlength += num;
				datalength -= 8100;
				byte[] array4 = new byte[datalength];
				Array.Copy(array3, 8100, array4, 0, array4.Length);
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

		// Token: 0x06000121 RID: 289 RVA: 0x00009C9F File Offset: 0x00007E9F
		public override void Dispose()
		{
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000F45D File Offset: 0x0000D65D
		public override Dictionary<string, int[]> GetObfs()
		{
			return VerifySimpleObfs._obfs;
		}

		// Token: 0x0600011E RID: 286 RVA: 0x0000F464 File Offset: 0x0000D664
		public void PackData(byte[] data, int datalength, byte[] outdata, out int outlength)
		{
			int num = this.random.Next(16) + 1;
			outlength = num + datalength + 6;
			outdata[0] = (byte)(outlength >> 8);
			outdata[1] = (byte)outlength;
			outdata[2] = (byte)num;
			Array.Copy(data, 0, outdata, num + 2, datalength);
			CRC32.SetCRC32(outdata, outlength);
		}

		// Token: 0x0600011C RID: 284 RVA: 0x0000F44C File Offset: 0x0000D64C
		public static List<string> SupportedObfs()
		{
			return new List<string>(VerifySimpleObfs._obfs.Keys);
		}

		// Token: 0x040000E7 RID: 231
		private static Dictionary<string, int[]> _obfs = new Dictionary<string, int[]>
		{
			{
				"verify_simple",
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
