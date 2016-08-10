using System;
using System.Text;

namespace OpenDNS
{
	// Token: 0x02000098 RID: 152
	public class ResourceRecord
	{
		// Token: 0x06000543 RID: 1347 RVA: 0x00007729 File Offset: 0x00005929
		public ResourceRecord()
		{
		}

		// Token: 0x06000544 RID: 1348 RVA: 0x0002B9E3 File Offset: 0x00029BE3
		public ResourceRecord(string _Name, Types _Type, Classes _Class, int _TimeToLive)
		{
			this.Name = _Name;
			this.Type = _Type;
			this.Class = _Class;
			this.TimeToLive = _TimeToLive;
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x0002BA08 File Offset: 0x00029C08
		public ResourceRecord(string _Name, Types _Type, Classes _Class, int _TimeToLive, string _RText)
		{
			this.Name = _Name;
			this.Type = _Type;
			this.Class = _Class;
			this.TimeToLive = _TimeToLive;
			this.RText = _RText;
		}

		// Token: 0x06000546 RID: 1350 RVA: 0x0002BA38 File Offset: 0x00029C38
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Concat(new object[]
			{
				"Name=",
				this.Name,
				"&Type=",
				this.Type,
				"&Class=",
				this.Class,
				"&TTL=",
				this.TimeToLive
			}));
			return stringBuilder.ToString();
		}

		// Token: 0x040003B4 RID: 948
		public Classes Class;

		// Token: 0x040003B2 RID: 946
		public string Name;

		// Token: 0x040003B6 RID: 950
		public string RText;

		// Token: 0x040003B5 RID: 949
		public int TimeToLive;

		// Token: 0x040003B3 RID: 947
		public Types Type;
	}
}
