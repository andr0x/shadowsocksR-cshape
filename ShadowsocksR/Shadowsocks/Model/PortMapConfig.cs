using System;

namespace Shadowsocks.Model
{
	// Token: 0x02000025 RID: 37
	[Serializable]
	public class PortMapConfig
	{
		// Token: 0x0400010B RID: 267
		public bool enable;

		// Token: 0x0400010C RID: 268
		public string id;

		// Token: 0x0400010A RID: 266
		public static int MemberCount = 4;

		// Token: 0x0400010D RID: 269
		public string server_addr;

		// Token: 0x0400010E RID: 270
		public int server_port;
	}
}
