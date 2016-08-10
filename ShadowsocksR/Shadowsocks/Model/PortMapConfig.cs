using System;

namespace Shadowsocks.Model
{
	// Token: 0x02000027 RID: 39
	[Serializable]
	public class PortMapConfig
	{
		// Token: 0x04000116 RID: 278
		public bool enable;

		// Token: 0x04000117 RID: 279
		public string id;

		// Token: 0x04000115 RID: 277
		public static int MemberCount = 4;

		// Token: 0x04000118 RID: 280
		public string server_addr;

		// Token: 0x04000119 RID: 281
		public int server_port;
	}
}
