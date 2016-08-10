using System;
using System.Collections.Generic;

namespace Shadowsocks.Encryption
{
	// Token: 0x02000031 RID: 49
	public static class EncryptorFactory
	{
		// Token: 0x060001B6 RID: 438 RVA: 0x00013724 File Offset: 0x00011924
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

		// Token: 0x060001B7 RID: 439 RVA: 0x00013888 File Offset: 0x00011A88
		public static List<string> GetEncryptor()
		{
			return EncryptorFactory._registeredEncryptorNames;
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x00013890 File Offset: 0x00011A90
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

		// Token: 0x04000172 RID: 370
		private static Type[] _constructorTypes;

		// Token: 0x04000171 RID: 369
		private static List<string> _registeredEncryptorNames;

		// Token: 0x04000170 RID: 368
		private static Dictionary<string, Type> _registeredEncryptors;
	}
}
