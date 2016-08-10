using System;
using System.Text;

namespace Shadowsocks.Encryption
{
	// Token: 0x02000030 RID: 48
	public abstract class EncryptorBase : IEncryptor, IDisposable
	{
		// Token: 0x060001AD RID: 429 RVA: 0x000136F4 File Offset: 0x000118F4
		protected EncryptorBase(string method, string password)
		{
			this.Method = method;
			this.Password = password;
		}

		// Token: 0x060001B0 RID: 432
		public abstract void Decrypt(byte[] buf, int length, byte[] outbuf, out int outlength);

		// Token: 0x060001B3 RID: 435
		public abstract void Dispose();

		// Token: 0x060001AF RID: 431
		public abstract void Encrypt(byte[] buf, int length, byte[] outbuf, out int outlength);

		// Token: 0x060001B4 RID: 436
		public abstract byte[] getIV();

		// Token: 0x060001B5 RID: 437
		public abstract byte[] getKey();

		// Token: 0x060001AE RID: 430 RVA: 0x0001370A File Offset: 0x0001190A
		protected byte[] GetPasswordHash()
		{
			return MbedTLS.MbedTLSMD5(Encoding.UTF8.GetBytes(this.Password));
		}

		// Token: 0x060001B2 RID: 434
		public abstract void ResetDecrypt();

		// Token: 0x060001B1 RID: 433
		public abstract void ResetEncrypt();

		// Token: 0x0400016D RID: 365
		public const int MAX_INPUT_SIZE = 65536;

		// Token: 0x0400016E RID: 366
		protected string Method;

		// Token: 0x0400016F RID: 367
		protected string Password;
	}
}
