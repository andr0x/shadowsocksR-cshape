using System;
using System.Collections;

namespace OpenDNS
{
	// Token: 0x02000099 RID: 153
	public class ResourceRecordCollection : ArrayList
	{
		// Token: 0x06000547 RID: 1351 RVA: 0x0002BAB6 File Offset: 0x00029CB6
		public void Sort(ResourceRecordCollection.SortFields sortField, bool isAscending)
		{
			if (sortField != ResourceRecordCollection.SortFields.Name)
			{
				if (sortField == ResourceRecordCollection.SortFields.TTL)
				{
					base.Sort(new ResourceRecordCollection.TTLComparer());
				}
			}
			else
			{
				base.Sort(new ResourceRecordCollection.NameComparer());
			}
			if (!isAscending)
			{
				base.Reverse();
			}
		}

		// Token: 0x020000C8 RID: 200
		private sealed class NameComparer : IComparer
		{
			// Token: 0x060005B6 RID: 1462 RVA: 0x0002CBF0 File Offset: 0x0002ADF0
			public int Compare(object x, object y)
			{
				ResourceRecord arg_0D_0 = (ResourceRecord)x;
				ResourceRecord resourceRecord = (ResourceRecord)y;
				return arg_0D_0.Name.CompareTo(resourceRecord.Name);
			}
		}

		// Token: 0x020000C7 RID: 199
		public enum SortFields
		{
			// Token: 0x0400047A RID: 1146
			Name,
			// Token: 0x0400047B RID: 1147
			TTL
		}

		// Token: 0x020000C9 RID: 201
		private sealed class TTLComparer : IComparer
		{
			// Token: 0x060005B8 RID: 1464 RVA: 0x0002CC1C File Offset: 0x0002AE1C
			public int Compare(object x, object y)
			{
				ResourceRecord arg_0D_0 = (ResourceRecord)x;
				ResourceRecord resourceRecord = (ResourceRecord)y;
				return arg_0D_0.TimeToLive.CompareTo(resourceRecord.TimeToLive);
			}
		}
	}
}
