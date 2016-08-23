using System;
using System.Collections;

namespace OpenDNS
{
	// Token: 0x0200009B RID: 155
	public class ResourceRecordCollection : ArrayList
	{
		// Token: 0x06000550 RID: 1360 RVA: 0x0002ACFE File Offset: 0x00028EFE
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

		// Token: 0x020000C9 RID: 201
		private sealed class NameComparer : IComparer
		{
			// Token: 0x060005BB RID: 1467 RVA: 0x0002BE38 File Offset: 0x0002A038
			public int Compare(object x, object y)
			{
				ResourceRecord arg_0D_0 = (ResourceRecord)x;
				ResourceRecord resourceRecord = (ResourceRecord)y;
				return arg_0D_0.Name.CompareTo(resourceRecord.Name);
			}
		}

		// Token: 0x020000C8 RID: 200
		public enum SortFields
		{
			// Token: 0x04000477 RID: 1143
			Name,
			// Token: 0x04000478 RID: 1144
			TTL
		}

		// Token: 0x020000CA RID: 202
		private sealed class TTLComparer : IComparer
		{
			// Token: 0x060005BD RID: 1469 RVA: 0x0002BE64 File Offset: 0x0002A064
			public int Compare(object x, object y)
			{
				ResourceRecord arg_0D_0 = (ResourceRecord)x;
				ResourceRecord resourceRecord = (ResourceRecord)y;
				return arg_0D_0.TimeToLive.CompareTo(resourceRecord.TimeToLive);
			}
		}
	}
}
