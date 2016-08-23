using System;
using System.Collections.Generic;

namespace Shadowsocks.Encryption
{
	// Token: 0x02000037 RID: 55
	public class SodiumEncryptor : IVEncryptor, IDisposable
	{
		// Token: 0x060001FA RID: 506 RVA: 0x00014274 File Offset: 0x00012474
		public SodiumEncryptor(string method, string password) : base(method, password)
		{
			base.InitKey(method, password);
			this._encryptBuf = new byte[65600];
			this._decryptBuf = new byte[65600];
		}

		// Token: 0x060001FD RID: 509 RVA: 0x000142C0 File Offset: 0x000124C0
		protected override void cipherUpdate(bool isCipher, int length, byte[] buf, byte[] outbuf)
		{
			int num;
			ulong num2;
			byte[] array;
			byte[] n;
			if (isCipher)
			{
				num = this._encryptBytesRemaining;
				num2 = this._encryptIC;
				array = this._encryptBuf;
				n = this._encryptIV;
			}
			else
			{
				num = this._decryptBytesRemaining;
				num2 = this._decryptIC;
				array = this._decryptBuf;
				n = this._decryptIV;
			}
			int num3 = num;
			Buffer.BlockCopy(buf, 0, array, num3, length);
			switch (this._cipher)
			{
			case 1:
			{
				byte[] expr_6B = array;
				Sodium.crypto_stream_salsa20_xor_ic(expr_6B, expr_6B, (ulong)((long)(num3 + length)), n, num2, this._key);
				break;
			}
			case 2:
			{
				byte[] expr_81 = array;
				Sodium.crypto_stream_chacha20_xor_ic(expr_81, expr_81, (ulong)((long)(num3 + length)), n, num2, this._key);
				break;
			}
			case 3:
			{
				byte[] expr_97 = array;
				Sodium.crypto_stream_chacha20_ietf_xor_ic(expr_97, expr_97, (ulong)((long)(num3 + length)), n, (uint)num2, this._key);
				break;
			}
			}
			Buffer.BlockCopy(array, num3, outbuf, 0, length);
			num3 += length;
			num2 += (ulong)((long)num3 / 64L);
			num = num3 % 64;
			if (isCipher)
			{
				this._encryptBytesRemaining = num;
				this._encryptIC = num2;
				return;
			}
			this._decryptBytesRemaining = num;
			this._decryptIC = num2;
		}

		// Token: 0x06000200 RID: 512 RVA: 0x00009C9F File Offset: 0x00007E9F
		public override void Dispose()
		{
		}

		// Token: 0x060001FB RID: 507 RVA: 0x000142A6 File Offset: 0x000124A6
		protected override Dictionary<string, int[]> getCiphers()
		{
			return SodiumEncryptor._ciphers;
		}

		// Token: 0x060001FF RID: 511 RVA: 0x000143D3 File Offset: 0x000125D3
		public override void ResetDecrypt()
		{
			this._decryptIVReceived = 0;
			this._decryptIC = 0uL;
			this._decryptBytesRemaining = 0;
		}

		// Token: 0x060001FE RID: 510 RVA: 0x000143BB File Offset: 0x000125BB
		public override void ResetEncrypt()
		{
			this._encryptIVSent = false;
			this._encryptIC = 0uL;
			this._encryptBytesRemaining = 0;
		}

		// Token: 0x060001FC RID: 508 RVA: 0x000142AD File Offset: 0x000124AD
		public static List<string> SupportedCiphers()
		{
			return new List<string>(SodiumEncryptor._ciphers.Keys);
		}

		// Token: 0x04000192 RID: 402
		private const int CIPHER_CHACHA20 = 2;

		// Token: 0x04000193 RID: 403
		private const int CIPHER_CHACHA20_IETF = 3;

		// Token: 0x04000191 RID: 401
		private const int CIPHER_SALSA20 = 1;

		// Token: 0x04000194 RID: 404
		private const int SODIUM_BLOCK_SIZE = 64;

		// Token: 0x0400019B RID: 411
		private static Dictionary<string, int[]> _ciphers = new Dictionary<string, int[]>
		{
			{
				"salsa20",
				new int[]
				{
					32,
					8,
					1
				}
			},
			{
				"chacha20",
				new int[]
				{
					32,
					8,
					2
				}
			},
			{
				"chacha20-ietf",
				new int[]
				{
					32,
					12,
					3
				}
			}
		};

		// Token: 0x0400019A RID: 410
		protected byte[] _decryptBuf;

		// Token: 0x04000196 RID: 406
		protected int _decryptBytesRemaining;

		// Token: 0x04000198 RID: 408
		protected ulong _decryptIC;

		// Token: 0x04000199 RID: 409
		protected byte[] _encryptBuf;

		// Token: 0x04000195 RID: 405
		protected int _encryptBytesRemaining;

		// Token: 0x04000197 RID: 407
		protected ulong _encryptIC;
	}
}
