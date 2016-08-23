using System;

namespace ZXing
{
	// Token: 0x0200006B RID: 107
	[Serializable]
	public sealed class WriterException : Exception
	{
		// Token: 0x060003DB RID: 987 RVA: 0x0002066A File Offset: 0x0001E86A
		public WriterException()
		{
		}

		// Token: 0x060003DC RID: 988 RVA: 0x0000EFB0 File Offset: 0x0000D1B0
		public WriterException(string message) : base(message)
		{
		}

		// Token: 0x060003DD RID: 989 RVA: 0x00020672 File Offset: 0x0001E872
		public WriterException(string message, Exception innerExc) : base(message, innerExc)
		{
		}
	}
}
