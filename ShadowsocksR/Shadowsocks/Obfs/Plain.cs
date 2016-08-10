using System;
using System.Collections.Generic;

namespace Shadowsocks.Obfs
{
	// Token: 0x0200001C RID: 28
	public class Plain : ObfsBase
	{
		// Token: 0x06000122 RID: 290 RVA: 0x0000FDD5 File Offset: 0x0000DFD5
		public Plain(string method) : base(method)
		{
		}

		// Token: 0x06000126 RID: 294 RVA: 0x0000FE0C File Offset: 0x0000E00C
		public override byte[] ClientDecode(byte[] encryptdata, int datalength, out int outlength, out bool needsendback)
		{
			outlength = datalength;
			needsendback = false;
			return encryptdata;
		}

		// Token: 0x06000125 RID: 293 RVA: 0x0000FDF6 File Offset: 0x0000DFF6
		public override byte[] ClientEncode(byte[] encryptdata, int datalength, out int outlength)
		{
			outlength = datalength;
			this.SentLength += (long)outlength;
			return encryptdata;
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00009AEF File Offset: 0x00007CEF
		public override void Dispose()
		{
		}

		// Token: 0x06000124 RID: 292 RVA: 0x0000FDEF File Offset: 0x0000DFEF
		public override Dictionary<string, int[]> GetObfs()
		{
			return Plain._obfs;
		}

		// Token: 0x06000123 RID: 291 RVA: 0x0000FDDE File Offset: 0x0000DFDE
		public static List<string> SupportedObfs()
		{
			return new List<string>(Plain._obfs.Keys);
		}

		// Token: 0x040000ED RID: 237
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
