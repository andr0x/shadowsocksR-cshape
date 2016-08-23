using System;
using System.Collections.Generic;

namespace Shadowsocks.Obfs
{
	// Token: 0x0200001A RID: 26
	public class Plain : ObfsBase
	{
		// Token: 0x0600010E RID: 270 RVA: 0x0000F391 File Offset: 0x0000D591
		public Plain(string method) : base(method)
		{
		}

		// Token: 0x06000112 RID: 274 RVA: 0x0000F3C8 File Offset: 0x0000D5C8
		public override byte[] ClientDecode(byte[] encryptdata, int datalength, out int outlength, out bool needsendback)
		{
			outlength = datalength;
			needsendback = false;
			return encryptdata;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x0000F3B2 File Offset: 0x0000D5B2
		public override byte[] ClientEncode(byte[] encryptdata, int datalength, out int outlength)
		{
			outlength = datalength;
			this.SentLength += (long)outlength;
			return encryptdata;
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00009C9F File Offset: 0x00007E9F
		public override void Dispose()
		{
		}

		// Token: 0x06000110 RID: 272 RVA: 0x0000F3AB File Offset: 0x0000D5AB
		public override Dictionary<string, int[]> GetObfs()
		{
			return Plain._obfs;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x0000F39A File Offset: 0x0000D59A
		public static List<string> SupportedObfs()
		{
			return new List<string>(Plain._obfs.Keys);
		}

		// Token: 0x040000E2 RID: 226
		private static Dictionary<string, int[]> _obfs = new Dictionary<string, int[]>
		{
			{
				"plain",
				new int[3]
			},
			{
				"origin",
				new int[3]
			}
		};
	}
}
