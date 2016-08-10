using System;

namespace Shadowsocks.Obfs
{
	// Token: 0x0200001E RID: 30
	public abstract class VerifySimpleBase : ObfsBase
	{
		// Token: 0x0600012A RID: 298 RVA: 0x0000FE53 File Offset: 0x0000E053
		public VerifySimpleBase(string method) : base(method)
		{
		}

		// Token: 0x0600012E RID: 302 RVA: 0x0000FE0C File Offset: 0x0000E00C
		public override byte[] ClientDecode(byte[] encryptdata, int datalength, out int outlength, out bool needsendback)
		{
			outlength = datalength;
			needsendback = false;
			return encryptdata;
		}

		// Token: 0x0600012D RID: 301 RVA: 0x0000F9D0 File Offset: 0x0000DBD0
		public override byte[] ClientEncode(byte[] encryptdata, int datalength, out int outlength)
		{
			outlength = datalength;
			return encryptdata;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000FE77 File Offset: 0x0000E077
		public override object InitData()
		{
			return new VerifyData();
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000FE7E File Offset: 0x0000E07E
		public override void SetServerInfo(ServerInfo serverInfo)
		{
			base.SetServerInfo(serverInfo);
		}

		// Token: 0x040000F1 RID: 241
		protected Random random = new Random();

		// Token: 0x040000EE RID: 238
		protected const int RecvBufferSize = 131072;

		// Token: 0x040000EF RID: 239
		protected byte[] recv_buf = new byte[131072];

		// Token: 0x040000F0 RID: 240
		protected int recv_buf_len;
	}
}
