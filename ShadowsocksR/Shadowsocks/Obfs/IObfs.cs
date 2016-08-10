using System;

namespace Shadowsocks.Obfs
{
	// Token: 0x02000019 RID: 25
	public interface IObfs : IDisposable
	{
		// Token: 0x06000109 RID: 265
		byte[] ClientDecode(byte[] encryptdata, int datalength, out int outlength, out bool needsendback);

		// Token: 0x06000108 RID: 264
		byte[] ClientEncode(byte[] encryptdata, int datalength, out int outlength);

		// Token: 0x0600010A RID: 266
		byte[] ClientPostDecrypt(byte[] plaindata, int datalength, out int outlength);

		// Token: 0x06000107 RID: 263
		byte[] ClientPreEncrypt(byte[] plaindata, int datalength, out int outlength);

		// Token: 0x0600010C RID: 268
		byte[] ClientUdpPostDecrypt(byte[] plaindata, int datalength, out int outlength);

		// Token: 0x0600010B RID: 267
		byte[] ClientUdpPreEncrypt(byte[] plaindata, int datalength, out int outlength);

		// Token: 0x06000110 RID: 272
		long getSentLength();

		// Token: 0x0600010D RID: 269
		object InitData();

		// Token: 0x06000106 RID: 262
		string Name();

		// Token: 0x0600010E RID: 270
		void SetServerInfo(ServerInfo serverInfo);

		// Token: 0x0600010F RID: 271
		void SetServerInfoIV(byte[] iv);
	}
}
