using System;
using System.Collections.Generic;

namespace Shadowsocks.Encryption
{
	// Token: 0x02000032 RID: 50
	public class LibcryptoEncryptor : IVEncryptor, IDisposable
	{
		// Token: 0x060001CC RID: 460 RVA: 0x00013831 File Offset: 0x00011A31
		public LibcryptoEncryptor(string method, string password) : base(method, password)
		{
			base.InitKey(method, password);
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x00013A0A File Offset: 0x00011C0A
		protected override void cipherUpdate(bool isCipher, int length, byte[] buf, byte[] outbuf)
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}
			Libcrypto.update(isCipher ? this._encryptCtx : this._decryptCtx, buf, length, outbuf);
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x00013A3B File Offset: 0x00011C3B
		public override void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x00013A7C File Offset: 0x00011C7C
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
					Libcrypto.clean(this._encryptCtx);
				}
				if (this._decryptCtx != IntPtr.Zero)
				{
					Libcrypto.clean(this._decryptCtx);
				}
				this._encryptCtx = IntPtr.Zero;
				this._decryptCtx = IntPtr.Zero;
			}
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x00013A4C File Offset: 0x00011C4C
		~LibcryptoEncryptor()
		{
			this.Dispose(false);
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x00013920 File Offset: 0x00011B20
		protected override Dictionary<string, int[]> getCiphers()
		{
			return LibcryptoEncryptor._ciphers;
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0001385C File Offset: 0x00011A5C
		public static void InitAviable()
		{
			List<string> list = new List<string>();
			foreach (string current in LibcryptoEncryptor._ciphers.Keys)
			{
				if (!Libcrypto.is_cipher(current))
				{
					list.Add(current);
				}
			}
			foreach (string current2 in list)
			{
				LibcryptoEncryptor._ciphers.Remove(current2);
			}
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x00013928 File Offset: 0x00011B28
		protected override void initCipher(byte[] iv, bool isCipher)
		{
			base.initCipher(iv, isCipher);
			byte[] key;
			if (this._method.StartsWith("rc4-md5"))
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
			if (isCipher)
			{
				IntPtr intPtr;
				if (this._encryptCtx == IntPtr.Zero)
				{
					intPtr = Libcrypto.init(this.Method, key, iv, 1);
					this._encryptCtx = intPtr;
					return;
				}
				intPtr = this._encryptCtx;
				return;
			}
			else
			{
				IntPtr intPtr;
				if (this._decryptCtx == IntPtr.Zero)
				{
					intPtr = Libcrypto.init(this.Method, key, iv, 0);
					this._decryptCtx = intPtr;
					return;
				}
				intPtr = this._decryptCtx;
				return;
			}
		}

		// Token: 0x060001CF RID: 463 RVA: 0x00013919 File Offset: 0x00011B19
		public static bool isSupport()
		{
			return Libcrypto.isSupport();
		}

		// Token: 0x060001CE RID: 462 RVA: 0x00013908 File Offset: 0x00011B08
		public static List<string> SupportedCiphers()
		{
			return new List<string>(LibcryptoEncryptor._ciphers.Keys);
		}

		// Token: 0x0400017B RID: 379
		private const int CIPHER_AES = 1;

		// Token: 0x0400017D RID: 381
		private const int CIPHER_CAMELLIA = 3;

		// Token: 0x0400017E RID: 382
		private const int CIPHER_OTHER_CFB = 4;

		// Token: 0x0400017C RID: 380
		private const int CIPHER_RC4 = 2;

		// Token: 0x04000181 RID: 385
		private static Dictionary<string, int[]> _ciphers = new Dictionary<string, int[]>
		{
			{
				"aes-128-cfb",
				new int[]
				{
					16,
					16,
					1
				}
			},
			{
				"aes-192-cfb",
				new int[]
				{
					24,
					16,
					1
				}
			},
			{
				"aes-256-cfb",
				new int[]
				{
					32,
					16,
					1
				}
			},
			{
				"aes-128-ctr",
				new int[]
				{
					16,
					16,
					1
				}
			},
			{
				"aes-192-ctr",
				new int[]
				{
					24,
					16,
					1
				}
			},
			{
				"aes-256-ctr",
				new int[]
				{
					32,
					16,
					1
				}
			},
			{
				"camellia-128-cfb",
				new int[]
				{
					16,
					16,
					3
				}
			},
			{
				"camellia-192-cfb",
				new int[]
				{
					24,
					16,
					3
				}
			},
			{
				"camellia-256-cfb",
				new int[]
				{
					32,
					16,
					3
				}
			},
			{
				"bf-cfb",
				new int[]
				{
					16,
					8,
					4
				}
			},
			{
				"cast5-cfb",
				new int[]
				{
					16,
					8,
					4
				}
			},
			{
				"des-cfb",
				new int[]
				{
					8,
					8,
					4
				}
			},
			{
				"des-ede3-cfb",
				new int[]
				{
					24,
					8,
					4
				}
			},
			{
				"idea-cfb",
				new int[]
				{
					16,
					8,
					4
				}
			},
			{
				"rc2-cfb",
				new int[]
				{
					16,
					8,
					4
				}
			},
			{
				"rc4-md5",
				new int[]
				{
					16,
					16,
					2
				}
			},
			{
				"rc4-md5-6",
				new int[]
				{
					16,
					6,
					2
				}
			},
			{
				"seed-cfb",
				new int[]
				{
					16,
					16,
					4
				}
			}
		};

		// Token: 0x04000180 RID: 384
		private IntPtr _decryptCtx = IntPtr.Zero;

		// Token: 0x04000182 RID: 386
		private bool _disposed;

		// Token: 0x0400017F RID: 383
		private IntPtr _encryptCtx = IntPtr.Zero;
	}
}
