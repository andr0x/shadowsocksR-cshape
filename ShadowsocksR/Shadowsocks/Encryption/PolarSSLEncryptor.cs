using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Shadowsocks.Encryption
{
	// Token: 0x02000037 RID: 55
	public class PolarSSLEncryptor : IVEncryptor, IDisposable
	{
		// Token: 0x06000200 RID: 512 RVA: 0x000146E8 File Offset: 0x000128E8
		public PolarSSLEncryptor(string method, string password) : base(method, password)
		{
			base.InitKey(method, password);
		}

		// Token: 0x06000204 RID: 516 RVA: 0x00014848 File Offset: 0x00012A48
		protected override void cipherUpdate(bool isCipher, int length, byte[] buf, byte[] outbuf)
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}
			byte[] iv;
			int num;
			IntPtr ctx;
			if (isCipher)
			{
				iv = this._encryptIV;
				num = this._encryptIVOffset;
				ctx = this._encryptCtx;
			}
			else
			{
				iv = this._decryptIV;
				num = this._decryptIVOffset;
				ctx = this._decryptCtx;
			}
			int cipher = this._cipher;
			if (cipher != 1)
			{
				if (cipher != 2)
				{
					return;
				}
				PolarSSL.arc4_crypt(ctx, length, buf, outbuf);
				return;
			}
			else
			{
				PolarSSL.aes_crypt_cfb128(ctx, isCipher ? 1 : 0, length, ref num, iv, buf, outbuf);
				if (isCipher)
				{
					this._encryptIVOffset = num;
					return;
				}
				this._decryptIVOffset = num;
				return;
			}
		}

		// Token: 0x06000205 RID: 517 RVA: 0x000148DB File Offset: 0x00012ADB
		public override void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0001491C File Offset: 0x00012B1C
		protected virtual void Dispose(bool disposing)
		{
			lock (this)
			{
				if (this._disposed)
				{
					return;
				}
				this._disposed = true;
			}
			if (disposing)
			{
				if (this._encryptCtx != IntPtr.Zero)
				{
					int cipher = this._cipher;
					if (cipher != 1)
					{
						if (cipher == 2)
						{
							PolarSSL.arc4_free(this._encryptCtx);
						}
					}
					else
					{
						PolarSSL.aes_free(this._encryptCtx);
					}
					Marshal.FreeHGlobal(this._encryptCtx);
					this._encryptCtx = IntPtr.Zero;
				}
				if (this._decryptCtx != IntPtr.Zero)
				{
					int cipher = this._cipher;
					if (cipher != 1)
					{
						if (cipher == 2)
						{
							PolarSSL.arc4_free(this._decryptCtx);
						}
					}
					else
					{
						PolarSSL.aes_free(this._decryptCtx);
					}
					Marshal.FreeHGlobal(this._decryptCtx);
					this._decryptCtx = IntPtr.Zero;
				}
			}
		}

		// Token: 0x06000206 RID: 518 RVA: 0x000148EC File Offset: 0x00012AEC
		~PolarSSLEncryptor()
		{
			this.Dispose(false);
		}

		// Token: 0x06000202 RID: 514 RVA: 0x00014721 File Offset: 0x00012921
		protected override Dictionary<string, int[]> getCiphers()
		{
			return PolarSSLEncryptor._ciphers;
		}

		// Token: 0x06000203 RID: 515 RVA: 0x00014728 File Offset: 0x00012928
		protected override void initCipher(byte[] iv, bool isCipher)
		{
			base.initCipher(iv, isCipher);
			IntPtr intPtr;
			if (isCipher)
			{
				if (this._encryptCtx == IntPtr.Zero)
				{
					intPtr = Marshal.AllocHGlobal(this._cipherInfo[3]);
					this._encryptCtx = intPtr;
				}
				else
				{
					intPtr = this._encryptCtx;
				}
			}
			else if (this._decryptCtx == IntPtr.Zero)
			{
				intPtr = Marshal.AllocHGlobal(this._cipherInfo[3]);
				this._decryptCtx = intPtr;
			}
			else
			{
				intPtr = this._decryptCtx;
			}
			byte[] key;
			if (this._method == "rc4-md5")
			{
				byte[] array = new byte[this.keyLen + this.ivLen];
				key = new byte[this.keyLen];
				Array.Copy(this._key, 0, array, 0, this.keyLen);
				Array.Copy(iv, 0, array, this.keyLen, this.ivLen);
				key = MbedTLS.MbedTLSMD5(array);
			}
			else
			{
				key = this._key;
			}
			if (this._cipher == 1)
			{
				PolarSSL.aes_init(intPtr);
				PolarSSL.aes_setkey_enc(intPtr, key, this.keyLen * 8);
				return;
			}
			if (this._cipher == 2)
			{
				PolarSSL.arc4_init(intPtr);
				PolarSSL.arc4_setup(intPtr, key, this.keyLen);
			}
		}

		// Token: 0x06000201 RID: 513 RVA: 0x00014710 File Offset: 0x00012910
		public static List<string> SupportedCiphers()
		{
			return new List<string>(PolarSSLEncryptor._ciphers.Keys);
		}

		// Token: 0x04000194 RID: 404
		private const int CIPHER_AES = 1;

		// Token: 0x04000195 RID: 405
		private const int CIPHER_RC4 = 2;

		// Token: 0x04000198 RID: 408
		private static Dictionary<string, int[]> _ciphers = new Dictionary<string, int[]>
		{
			{
				"aes-128-cfb",
				new int[]
				{
					16,
					16,
					1,
					280
				}
			},
			{
				"aes-192-cfb",
				new int[]
				{
					24,
					16,
					1,
					280
				}
			},
			{
				"aes-256-cfb",
				new int[]
				{
					32,
					16,
					1,
					280
				}
			},
			{
				"rc4-md5",
				new int[]
				{
					16,
					16,
					2,
					264
				}
			}
		};

		// Token: 0x04000197 RID: 407
		private IntPtr _decryptCtx = IntPtr.Zero;

		// Token: 0x04000199 RID: 409
		private bool _disposed;

		// Token: 0x04000196 RID: 406
		private IntPtr _encryptCtx = IntPtr.Zero;
	}
}
