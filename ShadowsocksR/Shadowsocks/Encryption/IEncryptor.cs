using System;

namespace Shadowsocks.Encryption
{
	// Token: 0x0200003A RID: 58
	public interface IEncryptor : IDisposable
	{
		// Token: 0x06000218 RID: 536
		void Decrypt(byte[] buf, int length, byte[] outbuf, out int outlength);

		// Token: 0x06000217 RID: 535
		void Encrypt(byte[] buf, int length, byte[] outbuf, out int outlength);

		// Token: 0x0600021B RID: 539
		byte[] getIV();

		// Token: 0x0600021C RID: 540
		byte[] getKey();

		// Token: 0x0600021A RID: 538
		void ResetDecrypt();

		// Token: 0x06000219 RID: 537
		void ResetEncrypt();
	}
}
