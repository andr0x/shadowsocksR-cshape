using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Shadowsocks.Encryption
{
	// Token: 0x02000032 RID: 50
	public abstract class IVEncryptor : EncryptorBase
	{
		// Token: 0x060001B9 RID: 441 RVA: 0x000138E1 File Offset: 0x00011AE1
		public IVEncryptor(string method, string password) : base(method, password)
		{
			this.InitKey(method, password);
		}

		// Token: 0x060001BE RID: 446 RVA: 0x00013A88 File Offset: 0x00011C88
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

		// Token: 0x060001C1 RID: 449
		protected abstract void cipherUpdate(bool isCipher, int length, byte[] buf, byte[] outbuf);

		// Token: 0x060001C3 RID: 451 RVA: 0x00013C14 File Offset: 0x00011E14
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

		// Token: 0x060001C2 RID: 450 RVA: 0x00013B68 File Offset: 0x00011D68
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

		// Token: 0x060001BA RID: 442
		protected abstract Dictionary<string, int[]> getCiphers();

		// Token: 0x060001BB RID: 443 RVA: 0x000138F3 File Offset: 0x00011AF3
		public override byte[] getIV()
		{
			return this._iv;
		}

		// Token: 0x060001BC RID: 444 RVA: 0x000138FC File Offset: 0x00011AFC
		public override byte[] getKey()
		{
			byte[] result = (byte[])this._key.Clone();
			Array.Resize<byte>(ref result, this.keyLen);
			return result;
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x00013B08 File Offset: 0x00011D08
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

		// Token: 0x060001BD RID: 445 RVA: 0x00013928 File Offset: 0x00011B28
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

		// Token: 0x060001BF RID: 447 RVA: 0x00013AE0 File Offset: 0x00011CE0
		protected static void randBytes(byte[] buf, int length)
		{
			byte[] array = new byte[length];
			new RNGCryptoServiceProvider().GetBytes(array);
			array.CopyTo(buf, 0);
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x00013D91 File Offset: 0x00011F91
		public override void ResetDecrypt()
		{
			this._decryptIVReceived = 0;
			this._decryptIVOffset = 0;
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x00013D70 File Offset: 0x00011F70
		public override void ResetEncrypt()
		{
			this._encryptIVSent = false;
			this._encryptIVOffset = 0;
			IVEncryptor.randBytes(this._iv, this.ivLen);
		}

		// Token: 0x04000175 RID: 373
		private static readonly Dictionary<string, byte[]> CachedKeys = new Dictionary<string, byte[]>();

		// Token: 0x04000174 RID: 372
		protected Dictionary<string, int[]> ciphers;

		// Token: 0x04000182 RID: 386
		protected int ivLen;

		// Token: 0x04000180 RID: 384
		protected int keyLen;

		// Token: 0x04000173 RID: 371
		protected static byte[] tempbuf = new byte[65536];

		// Token: 0x0400017D RID: 381
		protected int _cipher;

		// Token: 0x0400017E RID: 382
		protected int[] _cipherInfo;

		// Token: 0x04000177 RID: 375
		protected byte[] _decryptIV;

		// Token: 0x0400017B RID: 379
		protected int _decryptIVOffset;

		// Token: 0x04000178 RID: 376
		protected int _decryptIVReceived;

		// Token: 0x04000176 RID: 374
		protected byte[] _encryptIV;

		// Token: 0x0400017A RID: 378
		protected int _encryptIVOffset;

		// Token: 0x04000179 RID: 377
		protected bool _encryptIVSent;

		// Token: 0x04000181 RID: 385
		protected byte[] _iv;

		// Token: 0x0400017F RID: 383
		protected byte[] _key;

		// Token: 0x0400017C RID: 380
		protected string _method;
	}
}
