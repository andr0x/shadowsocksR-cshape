using System;

namespace Shadowsocks.Model
{
	// Token: 0x02000024 RID: 36
	public class UriVisitTime : IComparable
	{
		// Token: 0x06000159 RID: 345 RVA: 0x00010DFC File Offset: 0x0000EFFC
		public int CompareTo(object other)
		{
			if (!(other is UriVisitTime))
			{
				throw new InvalidOperationException("CompareTo: Not a UriVisitTime");
			}
			if (this.Equals(other))
			{
				return 0;
			}
			return this.visitTime.CompareTo(((UriVisitTime)other).visitTime);
		}

		// Token: 0x04000109 RID: 265
		public int index;

		// Token: 0x04000108 RID: 264
		public string uri;

		// Token: 0x04000107 RID: 263
		public DateTime visitTime;
	}
}
