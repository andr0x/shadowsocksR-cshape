using System;

namespace ZXing
{
	// Token: 0x02000069 RID: 105
	[Serializable]
	public sealed class WriterException : Exception
	{
		// Token: 0x060003D2 RID: 978 RVA: 0x00021422 File Offset: 0x0001F622
		public WriterException()
		{
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x0000F954 File Offset: 0x0000DB54
		public WriterException(string message) : base(message)
		{
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x0002142A File Offset: 0x0001F62A
		public WriterException(string message, Exception innerExc) : base(message, innerExc)
		{
		}
	}
}
