using System;

namespace Shadowsocks.Model
{
	// Token: 0x02000026 RID: 38
	public class UriVisitTime : IComparable
	{
		// Token: 0x0600016D RID: 365 RVA: 0x000117C0 File Offset: 0x0000F9C0
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

		// Token: 0x04000114 RID: 276
		public int index;

		// Token: 0x04000113 RID: 275
		public string uri;

		// Token: 0x04000112 RID: 274
		public DateTime visitTime;
	}
}
