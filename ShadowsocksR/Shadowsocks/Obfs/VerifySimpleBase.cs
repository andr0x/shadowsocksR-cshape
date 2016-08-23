using System;

namespace Shadowsocks.Obfs
{
	// Token: 0x0200001C RID: 28
	public abstract class VerifySimpleBase : ObfsBase
	{
		// Token: 0x06000116 RID: 278 RVA: 0x0000F40F File Offset: 0x0000D60F
		public VerifySimpleBase(string method) : base(method)
		{
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000F3C8 File Offset: 0x0000D5C8
		public override byte[] ClientDecode(byte[] encryptdata, int datalength, out int outlength, out bool needsendback)
		{
			outlength = datalength;
			needsendback = false;
			return encryptdata;
		}

		// Token: 0x06000119 RID: 281 RVA: 0x0000F02C File Offset: 0x0000D22C
		public override byte[] ClientEncode(byte[] encryptdata, int datalength, out int outlength)
		{
			outlength = datalength;
			return encryptdata;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x0000F433 File Offset: 0x0000D633
		public override object InitData()
		{
			return new VerifyData();
		}

		// Token: 0x06000118 RID: 280 RVA: 0x0000F43A File Offset: 0x0000D63A
		public override void SetServerInfo(ServerInfo serverInfo)
		{
			base.SetServerInfo(serverInfo);
		}

		// Token: 0x040000E6 RID: 230
		protected Random random = new Random();

		// Token: 0x040000E3 RID: 227
		protected const int RecvBufferSize = 131072;

		// Token: 0x040000E4 RID: 228
		protected byte[] recv_buf = new byte[131072];

		// Token: 0x040000E5 RID: 229
		protected int recv_buf_len;
	}
}
