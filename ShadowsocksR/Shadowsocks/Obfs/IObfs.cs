using System;

namespace Shadowsocks.Obfs
{
	// Token: 0x02000017 RID: 23
	public interface IObfs : IDisposable
	{
		// Token: 0x060000F5 RID: 245
		byte[] ClientDecode(byte[] encryptdata, int datalength, out int outlength, out bool needsendback);

		// Token: 0x060000F4 RID: 244
		byte[] ClientEncode(byte[] encryptdata, int datalength, out int outlength);

		// Token: 0x060000F6 RID: 246
		byte[] ClientPostDecrypt(byte[] plaindata, int datalength, out int outlength);

		// Token: 0x060000F3 RID: 243
		byte[] ClientPreEncrypt(byte[] plaindata, int datalength, out int outlength);

		// Token: 0x060000F8 RID: 248
		byte[] ClientUdpPostDecrypt(byte[] plaindata, int datalength, out int outlength);

		// Token: 0x060000F7 RID: 247
		byte[] ClientUdpPreEncrypt(byte[] plaindata, int datalength, out int outlength);

		// Token: 0x060000FC RID: 252
		long getSentLength();

		// Token: 0x060000F9 RID: 249
		object InitData();

		// Token: 0x060000F2 RID: 242
		string Name();

		// Token: 0x060000FA RID: 250
		void SetServerInfo(ServerInfo serverInfo);

		// Token: 0x060000FB RID: 251
		void SetServerInfoIV(byte[] iv);
	}
}
