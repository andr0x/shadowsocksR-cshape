using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Shadowsocks.Encryption
{
	// Token: 0x02000030 RID: 48
	public abstract class IVEncryptor : EncryptorBase
	{
		// Token: 0x060001A5 RID: 421 RVA: 0x00013071 File Offset: 0x00011271
		public IVEncryptor(string method, string password) : base(method, password)
		{
			this.InitKey(method, password);
		}

		// Token: 0x060001AA RID: 426 RVA: 0x00013218 File Offset: 0x00011418
		protected void bytesToKey(byte[] password, byte[] key)
		{
			byte[] array = new byte[password.Length + 16];
			int i = 0;
			byte[] array2 = null;
			while (i < key.Length)
			{
				if (i == 0)
				{
					array2 = MbedTLS.MbedTLSMD5(password);
				}
				else
				{
					array2.CopyTo(array, 0);
					password.CopyTo(array, array2.Length);
					array2 = MbedTLS.MbedTLSMD5(array);
				}
				array2.CopyTo(key, i);
				i += array2.Length;
			}
		}

		// Token: 0x060001AD RID: 429
		protected abstract void cipherUpdate(bool isCipher, int length, byte[] buf, byte[] outbuf);

		// Token: 0x060001AF RID: 431 RVA: 0x000133A4 File Offset: 0x000115A4
		public override void Decrypt(byte[] buf, int length, byte[] outbuf, out int outlength)
		{
			if (this._decryptIVReceived < this.ivLen)
			{
				int num = this.ivLen;
				if (this._decryptIVReceived + length < this.ivLen)
				{
					if (this._decryptIV == null)
					{
						this._decryptIV = new byte[this.ivLen];
					}
					Buffer.BlockCopy(buf, 0, this._decryptIV, this._decryptIVReceived, length);
					outlength = 0;
					this._decryptIVReceived += length;
				}
				else if (this._decryptIVReceived == 0)
				{
					this.initCipher(buf, false);
					outlength = length - this.ivLen;
					this._decryptIVReceived = this.ivLen;
				}
				else
				{
					num = this.ivLen - this._decryptIVReceived;
					byte[] array = new byte[this.ivLen];
					Buffer.BlockCopy(this._decryptIV, 0, array, 0, this._decryptIVReceived);
					Buffer.BlockCopy(buf, 0, array, this._decryptIVReceived, num);
					this.initCipher(array, false);
					outlength = length - num;
					this._decryptIVReceived = this.ivLen;
				}
				if (outlength <= 0)
				{
					return;
				}
				byte[] obj = IVEncryptor.tempbuf;
				lock (obj)
				{
					Buffer.BlockCopy(buf, num, IVEncryptor.tempbuf, 0, outlength);
					this.cipherUpdate(false, outlength, IVEncryptor.tempbuf, outbuf);
					return;
				}
			}
			outlength = length;
			this.cipherUpdate(false, length, buf, outbuf);
		}

		// Token: 0x060001AE RID: 430 RVA: 0x000132F8 File Offset: 0x000114F8
		public override void Encrypt(byte[] buf, int length, byte[] outbuf, out int outlength)
		{
			if (!this._encryptIVSent)
			{
				this._encryptIVSent = true;
				Buffer.BlockCopy(this._iv, 0, outbuf, 0, this.ivLen);
				this.initCipher(outbuf, true);
				outlength = length + this.ivLen;
				byte[] obj = IVEncryptor.tempbuf;
				lock (obj)
				{
					this.cipherUpdate(true, length, buf, IVEncryptor.tempbuf);
					outlength = length + this.ivLen;
					Buffer.BlockCopy(IVEncryptor.tempbuf, 0, outbuf, this.ivLen, length);
					return;
				}
			}
			outlength = length;
			this.cipherUpdate(true, length, buf, outbuf);
		}

		// Token: 0x060001A6 RID: 422
		protected abstract Dictionary<string, int[]> getCiphers();

		// Token: 0x060001A7 RID: 423 RVA: 0x00013083 File Offset: 0x00011283
		public override byte[] getIV()
		{
			return this._iv;
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x0001308C File Offset: 0x0001128C
		public override byte[] getKey()
		{
			byte[] result = (byte[])this._key.Clone();
			Array.Resize<byte>(ref result, this.keyLen);
			return result;
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00013298 File Offset: 0x00011498
		protected virtual void initCipher(byte[] iv, bool isCipher)
		{
			if (this.ivLen > 0)
			{
				if (isCipher)
				{
					this._encryptIV = new byte[this.ivLen];
					Array.Copy(iv, this._encryptIV, this.ivLen);
					return;
				}
				this._decryptIV = new byte[this.ivLen];
				Array.Copy(iv, this._decryptIV, this.ivLen);
			}
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x000130B8 File Offset: 0x000112B8
		protected void InitKey(string method, string password)
		{
			method = method.ToLower();
			this._method = method;
			string key = method + ":" + password;
			this.ciphers = this.getCiphers();
			this._cipherInfo = this.ciphers[this._method];
			this._cipher = this._cipherInfo[2];
			if (this._cipher == 0)
			{
				throw new Exception("method not found");
			}
			this.keyLen = this.ciphers[this._method][0];
			this.ivLen = this.ciphers[this._method][1];
			if (!IVEncryptor.CachedKeys.ContainsKey(key))
			{
				Dictionary<string, byte[]> cachedKeys = IVEncryptor.CachedKeys;
				lock (cachedKeys)
				{
					if (!IVEncryptor.CachedKeys.ContainsKey(key))
					{
						byte[] bytes = Encoding.UTF8.GetBytes(password);
						this._key = new byte[32];
						//new byte[16];
						this.bytesToKey(bytes, this._key);
						IVEncryptor.CachedKeys[key] = this._key;
					}
				}
			}
			if (this._key == null)
			{
				this._key = IVEncryptor.CachedKeys[key];
			}
			Array.Resize<byte>(ref this._iv, this.ivLen);
			IVEncryptor.randBytes(this._iv, this.ivLen);
		}

		// Token: 0x060001AB RID: 427 RVA: 0x00013270 File Offset: 0x00011470
		protected static void randBytes(byte[] buf, int length)
		{
			byte[] array = new byte[length];
			new RNGCryptoServiceProvider().GetBytes(array);
			array.CopyTo(buf, 0);
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x00013521 File Offset: 0x00011721
		public override void ResetDecrypt()
		{
			this._decryptIVReceived = 0;
			this._decryptIVOffset = 0;
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x00013500 File Offset: 0x00011700
		public override void ResetEncrypt()
		{
			this._encryptIVSent = false;
			this._encryptIVOffset = 0;
			IVEncryptor.randBytes(this._iv, this.ivLen);
		}

		// Token: 0x0400016B RID: 363
		private static readonly Dictionary<string, byte[]> CachedKeys = new Dictionary<string, byte[]>();

		// Token: 0x0400016A RID: 362
		protected Dictionary<string, int[]> ciphers;

		// Token: 0x04000178 RID: 376
		protected int ivLen;

		// Token: 0x04000176 RID: 374
		protected int keyLen;

		// Token: 0x04000169 RID: 361
		protected static byte[] tempbuf = new byte[65536];

		// Token: 0x04000173 RID: 371
		protected int _cipher;

		// Token: 0x04000174 RID: 372
		protected int[] _cipherInfo;

		// Token: 0x0400016D RID: 365
		protected byte[] _decryptIV;

		// Token: 0x04000171 RID: 369
		protected int _decryptIVOffset;

		// Token: 0x0400016E RID: 366
		protected int _decryptIVReceived;

		// Token: 0x0400016C RID: 364
		protected byte[] _encryptIV;

		// Token: 0x04000170 RID: 368
		protected int _encryptIVOffset;

		// Token: 0x0400016F RID: 367
		protected bool _encryptIVSent;

		// Token: 0x04000177 RID: 375
		protected byte[] _iv;

		// Token: 0x04000175 RID: 373
		protected byte[] _key;

		// Token: 0x04000172 RID: 370
		protected string _method;
	}
}
