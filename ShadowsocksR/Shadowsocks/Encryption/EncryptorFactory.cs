using System;
using System.Collections.Generic;

namespace Shadowsocks.Encryption
{
	// Token: 0x0200002F RID: 47
	public static class EncryptorFactory
	{
		// Token: 0x060001A2 RID: 418 RVA: 0x00012EB4 File Offset: 0x000110B4
		static EncryptorFactory()
		{
			EncryptorFactory._constructorTypes = new Type[]
			{
				typeof(string),
				typeof(string)
			};
			EncryptorFactory._registeredEncryptors = new Dictionary<string, Type>();
			EncryptorFactory._registeredEncryptorNames = new List<string>();
			if (LibcryptoEncryptor.isSupport())
			{
				LibcryptoEncryptor.InitAviable();
				using (List<string>.Enumerator enumerator = LibcryptoEncryptor.SupportedCiphers().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string current = enumerator.Current;
						EncryptorFactory._registeredEncryptorNames.Add(current);
						EncryptorFactory._registeredEncryptors.Add(current, typeof(LibcryptoEncryptor));
					}
					goto IL_E1;
				}
			}
			foreach (string current2 in PolarSSLEncryptor.SupportedCiphers())
			{
				EncryptorFactory._registeredEncryptorNames.Add(current2);
				EncryptorFactory._registeredEncryptors.Add(current2, typeof(PolarSSLEncryptor));
			}
			IL_E1:
			foreach (string current3 in SodiumEncryptor.SupportedCiphers())
			{
				EncryptorFactory._registeredEncryptorNames.Add(current3);
				EncryptorFactory._registeredEncryptors.Add(current3, typeof(SodiumEncryptor));
			}
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x00013018 File Offset: 0x00011218
		public static List<string> GetEncryptor()
		{
			return EncryptorFactory._registeredEncryptorNames;
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x00013020 File Offset: 0x00011220
		public static IEncryptor GetEncryptor(string method, string password)
		{
			if (string.IsNullOrEmpty(method))
			{
				method = "aes-256-cfb";
			}
			method = method.ToLowerInvariant();
			return (IEncryptor)EncryptorFactory._registeredEncryptors[method].GetConstructor(EncryptorFactory._constructorTypes).Invoke(new object[]
			{
				method,
				password
			});
		}

		// Token: 0x04000168 RID: 360
		private static Type[] _constructorTypes;

		// Token: 0x04000167 RID: 359
		private static List<string> _registeredEncryptorNames;

		// Token: 0x04000166 RID: 358
		private static Dictionary<string, Type> _registeredEncryptors;
	}
}
