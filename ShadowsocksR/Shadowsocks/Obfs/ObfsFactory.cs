using System;
using System.Collections.Generic;

namespace Shadowsocks.Obfs
{
	// Token: 0x0200001B RID: 27
	public static class ObfsFactory
	{
		// Token: 0x06000120 RID: 288 RVA: 0x0000FA30 File Offset: 0x0000DC30
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
			foreach (string current3 in TlsAuthObfs.SupportedObfs())
			{
				ObfsFactory._registeredObfs.Add(current3, typeof(TlsAuthObfs));
			}
			foreach (string current4 in TlsTicketAuthObfs.SupportedObfs())
			{
				ObfsFactory._registeredObfs.Add(current4, typeof(TlsTicketAuthObfs));
			}
			foreach (string current5 in VerifySimpleObfs.SupportedObfs())
			{
				ObfsFactory._registeredObfs.Add(current5, typeof(VerifySimpleObfs));
			}
			foreach (string current6 in VerifyDeflateObfs.SupportedObfs())
			{
				ObfsFactory._registeredObfs.Add(current6, typeof(VerifyDeflateObfs));
			}
			foreach (string current7 in VerifySHA1Obfs.SupportedObfs())
			{
				ObfsFactory._registeredObfs.Add(current7, typeof(VerifySHA1Obfs));
			}
			foreach (string current8 in AuthSimple.SupportedObfs())
			{
				ObfsFactory._registeredObfs.Add(current8, typeof(AuthSimple));
			}
			foreach (string current9 in AuthSHA1.SupportedObfs())
			{
				ObfsFactory._registeredObfs.Add(current9, typeof(AuthSHA1));
			}
			foreach (string current10 in AuthSHA1V2.SupportedObfs())
			{
				ObfsFactory._registeredObfs.Add(current10, typeof(AuthSHA1V2));
			}
		}

		// Token: 0x06000121 RID: 289 RVA: 0x0000FD88 File Offset: 0x0000DF88
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

		// Token: 0x040000EC RID: 236
		private static Type[] _constructorTypes;

		// Token: 0x040000EB RID: 235
		private static Dictionary<string, Type> _registeredObfs;
	}
}
