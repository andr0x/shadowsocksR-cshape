using System;

namespace OpenDNS
{
	// Token: 0x02000095 RID: 149
	public enum ResponseCodes
	{
		// Token: 0x0400038D RID: 909
		NoError,
		// Token: 0x0400038E RID: 910
		FormatError,
		// Token: 0x0400038F RID: 911
		ServerFailure,
		// Token: 0x04000390 RID: 912
		NameError,
		// Token: 0x04000391 RID: 913
		NotImplemented,
		// Token: 0x04000392 RID: 914
		Refused,
		// Token: 0x04000393 RID: 915
		Reserved = 15
	}
}
