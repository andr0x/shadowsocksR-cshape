using System;

namespace Shadowsocks.Obfs
{
	// Token: 0x02000016 RID: 22
	public class ServerInfo
	{
		// Token: 0x060000F0 RID: 240 RVA: 0x0000EFBC File Offset: 0x0000D1BC
		public ServerInfo(string host, int port, string param, object data, byte[] iv, byte[] key, int head_len, int tcp_mss)
		{
			this.host = host;
			this.port = port;
			this.param = param;
			this.data = data;
			this.iv = iv;
			this.key = key;
			this.head_len = head_len;
			this.tcp_mss = tcp_mss;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x0000F00C File Offset: 0x0000D20C
		public void SetIV(byte[] iv)
		{
			this.iv = iv;
		}

		// Token: 0x040000D8 RID: 216
		public object data;

		// Token: 0x040000DC RID: 220
		public int head_len;

		// Token: 0x040000D5 RID: 213
		public string host;

		// Token: 0x040000DA RID: 218
		public byte[] iv;

		// Token: 0x040000DB RID: 219
		public byte[] key;

		// Token: 0x040000D7 RID: 215
		public string param;

		// Token: 0x040000D6 RID: 214
		public int port;

		// Token: 0x040000D9 RID: 217
		public int tcp_mss;
	}
}
