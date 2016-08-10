using System;
using System.Collections.Generic;

namespace Shadowsocks.Encryption
{
	// Token: 0x02000034 RID: 52
	public class LibcryptoEncryptor : IVEncryptor, IDisposable
	{
		// Token: 0x060001E1 RID: 481 RVA: 0x000140C1 File Offset: 0x000122C1
		public LibcryptoEncryptor(string method, string password) : base(method, password)
		{
			base.InitKey(method, password);
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0001429A File Offset: 0x0001249A
		protected override void cipherUpdate(bool isCipher, int length, byte[] buf, byte[] outbuf)
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}
			Libcrypto.update(isCipher ? this._encryptCtx : this._decryptCtx, buf, length, outbuf);
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x000142CB File Offset: 0x000124CB
		public override void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0001430C File Offset: 0x0001250C
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

		// Token: 0x060001E9 RID: 489 RVA: 0x000142DC File Offset: 0x000124DC
		~LibcryptoEncryptor()
		{
			this.Dispose(false);
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x000141B0 File Offset: 0x000123B0
		protected override Dictionary<string, int[]> getCiphers()
		{
			return LibcryptoEncryptor._ciphers;
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x000140EC File Offset: 0x000122EC
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

		// Token: 0x060001E6 RID: 486 RVA: 0x000141B8 File Offset: 0x000123B8
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

		// Token: 0x060001E4 RID: 484 RVA: 0x000141A9 File Offset: 0x000123A9
		public static bool isSupport()
		{
			return Libcrypto.isSupport();
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x00014198 File Offset: 0x00012398
		public static List<string> SupportedCiphers()
		{
			return new List<string>(LibcryptoEncryptor._ciphers.Keys);
		}

		// Token: 0x04000185 RID: 389
		private const int CIPHER_AES = 1;

		// Token: 0x04000187 RID: 391
		private const int CIPHER_CAMELLIA = 3;

		// Token: 0x04000188 RID: 392
		private const int CIPHER_OTHER_CFB = 4;

		// Token: 0x04000186 RID: 390
		private const int CIPHER_RC4 = 2;

		// Token: 0x0400018B RID: 395
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

		// Token: 0x0400018A RID: 394
		private IntPtr _decryptCtx = IntPtr.Zero;

		// Token: 0x0400018C RID: 396
		private bool _disposed;

		// Token: 0x04000189 RID: 393
		private IntPtr _encryptCtx = IntPtr.Zero;
	}
}
