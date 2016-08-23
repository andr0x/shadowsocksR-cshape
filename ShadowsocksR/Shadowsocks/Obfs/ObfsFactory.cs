using System;
using System.Collections.Generic;

namespace Shadowsocks.Obfs
{
	// Token: 0x02000019 RID: 25
	public static class ObfsFactory
	{
		// Token: 0x0600010C RID: 268 RVA: 0x0000F08C File Offset: 0x0000D28C
		static ObfsFactory()
		{
			ObfsFactory._constructorTypes = new Type[]
			{
				typeof(string)
			};
			ObfsFactory._registeredObfs = new Dictionary<string, Type>();
			foreach (string current in Plain.SupportedObfs())
			{
				ObfsFactory._registeredObfs.Add(current, typeof(Plain));
			}
			foreach (string current2 in HttpSimpleObfs.SupportedObfs())
			{
				ObfsFactory._registeredObfs.Add(current2, typeof(HttpSimpleObfs));
			}
			foreach (string current3 in TlsTicketAuthObfs.SupportedObfs())
			{
				ObfsFactory._registeredObfs.Add(current3, typeof(TlsTicketAuthObfs));
			}
			foreach (string current4 in VerifySimpleObfs.SupportedObfs())
			{
				ObfsFactory._registeredObfs.Add(current4, typeof(VerifySimpleObfs));
			}
			foreach (string current5 in VerifyDeflateObfs.SupportedObfs())
			{
				ObfsFactory._registeredObfs.Add(current5, typeof(VerifyDeflateObfs));
			}
			foreach (string current6 in VerifySHA1Obfs.SupportedObfs())
			{
				ObfsFactory._registeredObfs.Add(current6, typeof(VerifySHA1Obfs));
			}
			foreach (string current7 in AuthSHA1.SupportedObfs())
			{
				ObfsFactory._registeredObfs.Add(current7, typeof(AuthSHA1));
			}
			foreach (string current8 in AuthSHA1V2.SupportedObfs())
			{
				ObfsFactory._registeredObfs.Add(current8, typeof(AuthSHA1V2));
			}
		}

		// Token: 0x0600010D RID: 269 RVA: 0x0000F344 File Offset: 0x0000D544
		public static IObfs GetObfs(string method)
		{
			if (string.IsNullOrEmpty(method))
			{
				method = "plain";
			}
			method = method.ToLowerInvariant();
			return (IObfs)ObfsFactory._registeredObfs[method].GetConstructor(ObfsFactory._constructorTypes).Invoke(new object[]
			{
				method
			});
		}

		// Token: 0x040000E1 RID: 225
		private static Type[] _constructorTypes;

		// Token: 0x040000E0 RID: 224
		private static Dictionary<string, Type> _registeredObfs;
	}
}
