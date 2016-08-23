using System;

namespace Shadowsocks.Encryption
{
	// Token: 0x02000038 RID: 56
	public interface IEncryptor : IDisposable
	{
		// Token: 0x06000203 RID: 515
		void Decrypt(byte[] buf, int length, byte[] outbuf, out int outlength);

		// Token: 0x06000202 RID: 514
		void Encrypt(byte[] buf, int length, byte[] outbuf, out int outlength);

		// Token: 0x06000206 RID: 518
		byte[] getIV();

		// Token: 0x06000207 RID: 519
		byte[] getKey();

		// Token: 0x06000205 RID: 517
		void ResetDecrypt();

		// Token: 0x06000204 RID: 516
		void ResetEncrypt();
	}
}
