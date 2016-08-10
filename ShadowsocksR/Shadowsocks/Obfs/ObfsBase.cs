using System;
using System.Collections.Generic;

namespace Shadowsocks.Obfs
{
	// Token: 0x0200001A RID: 26
	public abstract class ObfsBase : IObfs, IDisposable
	{
		// Token: 0x06000111 RID: 273 RVA: 0x0000F9B9 File Offset: 0x0000DBB9
		protected ObfsBase(string method)
		{
			this.Method = method;
		}

		// Token: 0x06000116 RID: 278
		public abstract byte[] ClientDecode(byte[] encryptdata, int datalength, out int outlength, out bool needsendback);

		// Token: 0x06000115 RID: 277
		public abstract byte[] ClientEncode(byte[] encryptdata, int datalength, out int outlength);

		// Token: 0x06000117 RID: 279 RVA: 0x0000F9D0 File Offset: 0x0000DBD0
		public virtual byte[] ClientPostDecrypt(byte[] plaindata, int datalength, out int outlength)
		{
			outlength = datalength;
			return plaindata;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x0000F9D0 File Offset: 0x0000DBD0
		public virtual byte[] ClientPreEncrypt(byte[] plaindata, int datalength, out int outlength)
		{
			outlength = datalength;
			return plaindata;
		}

		// Token: 0x06000119 RID: 281 RVA: 0x0000F9D0 File Offset: 0x0000DBD0
		public virtual byte[] ClientUdpPostDecrypt(byte[] plaindata, int datalength, out int outlength)
		{
			outlength = datalength;
			return plaindata;
		}

		// Token: 0x06000118 RID: 280 RVA: 0x0000F9D0 File Offset: 0x0000DBD0
		public virtual byte[] ClientUdpPreEncrypt(byte[] plaindata, int datalength, out int outlength)
		{
			outlength = datalength;
			return plaindata;
		}

		// Token: 0x0600011A RID: 282
		public abstract void Dispose();

		// Token: 0x0600011E RID: 286 RVA: 0x0000F9F0 File Offset: 0x0000DBF0
		public static int GetHeadSize(byte[] plaindata, int defaultValue)
		{
			if (plaindata == null || plaindata.Length < 2)
			{
				return defaultValue;
			}
			int num = (int)(plaindata[0] & 7);
			if (num == 1)
			{
				return 7;
			}
			if (num == 4)
			{
				return 19;
			}
			if (num == 3)
			{
				return (int)(4 + plaindata[1]);
			}
			return defaultValue;
		}

		// Token: 0x06000112 RID: 274
		public abstract Dictionary<string, int[]> GetObfs();

		// Token: 0x0600011F RID: 287 RVA: 0x0000FA26 File Offset: 0x0000DC26
		public long getSentLength()
		{
			return this.SentLength;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000F9D6 File Offset: 0x0000DBD6
		public virtual object InitData()
		{
			return null;
		}

		// Token: 0x06000113 RID: 275 RVA: 0x0000F9C8 File Offset: 0x0000DBC8
		public string Name()
		{
			return this.Method;
		}

		// Token: 0x0600011C RID: 284 RVA: 0x0000F9D9 File Offset: 0x0000DBD9
		public virtual void SetServerInfo(ServerInfo serverInfo)
		{
			this.Server = serverInfo;
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000F9E2 File Offset: 0x0000DBE2
		public virtual void SetServerInfoIV(byte[] iv)
		{
			this.Server.SetIV(iv);
		}

		// Token: 0x040000E8 RID: 232
		protected string Method;

		// Token: 0x040000EA RID: 234
		protected long SentLength;

		// Token: 0x040000E9 RID: 233
		protected ServerInfo Server;
	}
}
