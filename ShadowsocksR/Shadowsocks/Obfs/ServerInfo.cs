using System;

namespace Shadowsocks.Obfs
{
	// Token: 0x02000018 RID: 24
	public class ServerInfo
	{
		// Token: 0x06000104 RID: 260 RVA: 0x0000F960 File Offset: 0x0000DB60
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

		// Token: 0x06000105 RID: 261 RVA: 0x0000F9B0 File Offset: 0x0000DBB0
		public void SetIV(byte[] iv)
		{
			this.iv = iv;
		}

		// Token: 0x040000E3 RID: 227
		public object data;

		// Token: 0x040000E7 RID: 231
		public int head_len;

		// Token: 0x040000E0 RID: 224
		public string host;

		// Token: 0x040000E5 RID: 229
		public byte[] iv;

		// Token: 0x040000E6 RID: 230
		public byte[] key;

		// Token: 0x040000E2 RID: 226
		public string param;

		// Token: 0x040000E1 RID: 225
		public int port;

		// Token: 0x040000E4 RID: 228
		public int tcp_mss;
	}
}
