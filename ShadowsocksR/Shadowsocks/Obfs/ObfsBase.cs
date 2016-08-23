using System;
using System.Collections.Generic;

namespace Shadowsocks.Obfs
{
	// Token: 0x02000018 RID: 24
	public abstract class ObfsBase : IObfs, IDisposable
	{
		// Token: 0x060000FD RID: 253 RVA: 0x0000F015 File Offset: 0x0000D215
		protected ObfsBase(string method)
		{
			this.Method = method;
		}

		// Token: 0x06000102 RID: 258
		public abstract byte[] ClientDecode(byte[] encryptdata, int datalength, out int outlength, out bool needsendback);

		// Token: 0x06000101 RID: 257
		public abstract byte[] ClientEncode(byte[] encryptdata, int datalength, out int outlength);

		// Token: 0x06000103 RID: 259 RVA: 0x0000F02C File Offset: 0x0000D22C
		public virtual byte[] ClientPostDecrypt(byte[] plaindata, int datalength, out int outlength)
		{
			outlength = datalength;
			return plaindata;
		}

		// Token: 0x06000100 RID: 256 RVA: 0x0000F02C File Offset: 0x0000D22C
		public virtual byte[] ClientPreEncrypt(byte[] plaindata, int datalength, out int outlength)
		{
			outlength = datalength;
			return plaindata;
		}

		// Token: 0x06000105 RID: 261 RVA: 0x0000F02C File Offset: 0x0000D22C
		public virtual byte[] ClientUdpPostDecrypt(byte[] plaindata, int datalength, out int outlength)
		{
			outlength = datalength;
			return plaindata;
		}

		// Token: 0x06000104 RID: 260 RVA: 0x0000F02C File Offset: 0x0000D22C
		public virtual byte[] ClientUdpPreEncrypt(byte[] plaindata, int datalength, out int outlength)
		{
			outlength = datalength;
			return plaindata;
		}

		// Token: 0x06000106 RID: 262
		public abstract void Dispose();

		// Token: 0x0600010A RID: 266 RVA: 0x0000F04C File Offset: 0x0000D24C
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

		// Token: 0x060000FE RID: 254
		public abstract Dictionary<string, int[]> GetObfs();

		// Token: 0x0600010B RID: 267 RVA: 0x0000F082 File Offset: 0x0000D282
		public long getSentLength()
		{
			return this.SentLength;
		}

		// Token: 0x06000107 RID: 263 RVA: 0x0000F032 File Offset: 0x0000D232
		public virtual object InitData()
		{
			return null;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x0000F024 File Offset: 0x0000D224
		public string Name()
		{
			return this.Method;
		}

		// Token: 0x06000108 RID: 264 RVA: 0x0000F035 File Offset: 0x0000D235
		public virtual void SetServerInfo(ServerInfo serverInfo)
		{
			this.Server = serverInfo;
		}

		// Token: 0x06000109 RID: 265 RVA: 0x0000F03E File Offset: 0x0000D23E
		public virtual void SetServerInfoIV(byte[] iv)
		{
			this.Server.SetIV(iv);
		}

		// Token: 0x040000DD RID: 221
		protected string Method;

		// Token: 0x040000DF RID: 223
		protected long SentLength;

		// Token: 0x040000DE RID: 222
		protected ServerInfo Server;
	}
}
