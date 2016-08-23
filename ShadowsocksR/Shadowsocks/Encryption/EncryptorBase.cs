using System;
using System.Text;

namespace Shadowsocks.Encryption
{
	// Token: 0x0200002E RID: 46
	public abstract class EncryptorBase : IEncryptor, IDisposable
	{
		// Token: 0x06000199 RID: 409 RVA: 0x00012E84 File Offset: 0x00011084
		protected EncryptorBase(string method, string password)
		{
			this.Method = method;
			this.Password = password;
		}

		// Token: 0x0600019C RID: 412
		public abstract void Decrypt(byte[] buf, int length, byte[] outbuf, out int outlength);

		// Token: 0x0600019F RID: 415
		public abstract void Dispose();

		// Token: 0x0600019B RID: 411
		public abstract void Encrypt(byte[] buf, int length, byte[] outbuf, out int outlength);

		// Token: 0x060001A0 RID: 416
		public abstract byte[] getIV();

		// Token: 0x060001A1 RID: 417
		public abstract byte[] getKey();

		// Token: 0x0600019A RID: 410 RVA: 0x00012E9A File Offset: 0x0001109A
		protected byte[] GetPasswordHash()
		{
			return MbedTLS.MbedTLSMD5(Encoding.UTF8.GetBytes(this.Password));
		}

		// Token: 0x0600019E RID: 414
		public abstract void ResetDecrypt();

		// Token: 0x0600019D RID: 413
		public abstract void ResetEncrypt();

		// Token: 0x04000163 RID: 355
		public const int MAX_INPUT_SIZE = 65536;

		// Token: 0x04000164 RID: 356
		protected string Method;

		// Token: 0x04000165 RID: 357
		protected string Password;
	}
}
