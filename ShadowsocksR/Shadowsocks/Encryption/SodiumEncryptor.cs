using System;
using System.Collections.Generic;

namespace Shadowsocks.Encryption
{
	// Token: 0x02000039 RID: 57
	public class SodiumEncryptor : IVEncryptor, IDisposable
	{
		// Token: 0x0600020F RID: 527 RVA: 0x00014B04 File Offset: 0x00012D04
		public SodiumEncryptor(string method, string password) : base(method, password)
		{
			base.InitKey(method, password);
			this._encryptBuf = new byte[65600];
			this._decryptBuf = new byte[65600];
		}

		// Token: 0x06000212 RID: 530 RVA: 0x00014B50 File Offset: 0x00012D50
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

		// Token: 0x06000215 RID: 533 RVA: 0x00009AEF File Offset: 0x00007CEF
		public override void Dispose()
		{
		}

		// Token: 0x06000210 RID: 528 RVA: 0x00014B36 File Offset: 0x00012D36
		protected override Dictionary<string, int[]> getCiphers()
		{
			return SodiumEncryptor._ciphers;
		}

		// Token: 0x06000214 RID: 532 RVA: 0x00014C63 File Offset: 0x00012E63
		public override void ResetDecrypt()
		{
			this._decryptIVReceived = 0;
			this._decryptIC = 0uL;
			this._decryptBytesRemaining = 0;
		}

		// Token: 0x06000213 RID: 531 RVA: 0x00014C4B File Offset: 0x00012E4B
		public override void ResetEncrypt()
		{
			this._encryptIVSent = false;
			this._encryptIC = 0uL;
			this._encryptBytesRemaining = 0;
		}

		// Token: 0x06000211 RID: 529 RVA: 0x00014B3D File Offset: 0x00012D3D
		public static List<string> SupportedCiphers()
		{
			return new List<string>(SodiumEncryptor._ciphers.Keys);
		}

		// Token: 0x0400019C RID: 412
		private const int CIPHER_CHACHA20 = 2;

		// Token: 0x0400019D RID: 413
		private const int CIPHER_CHACHA20_IETF = 3;

		// Token: 0x0400019B RID: 411
		private const int CIPHER_SALSA20 = 1;

		// Token: 0x0400019E RID: 414
		private const int SODIUM_BLOCK_SIZE = 64;

		// Token: 0x040001A5 RID: 421
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

		// Token: 0x040001A4 RID: 420
		protected byte[] _decryptBuf;

		// Token: 0x040001A0 RID: 416
		protected int _decryptBytesRemaining;

		// Token: 0x040001A2 RID: 418
		protected ulong _decryptIC;

		// Token: 0x040001A3 RID: 419
		protected byte[] _encryptBuf;

		// Token: 0x0400019F RID: 415
		protected int _encryptBytesRemaining;

		// Token: 0x040001A1 RID: 417
		protected ulong _encryptIC;
	}
}
