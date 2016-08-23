using System;
using System.Text;

namespace OpenDNS
{
	// Token: 0x0200009A RID: 154
	public class ResourceRecord
	{
		// Token: 0x0600054C RID: 1356 RVA: 0x00007881 File Offset: 0x00005A81
		public ResourceRecord()
		{
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x0002AC2B File Offset: 0x00028E2B
		public ResourceRecord(string _Name, Types _Type, Classes _Class, int _TimeToLive)
		{
			this.Name = _Name;
			this.Type = _Type;
			this.Class = _Class;
			this.TimeToLive = _TimeToLive;
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x0002AC50 File Offset: 0x00028E50
		public ResourceRecord(string _Name, Types _Type, Classes _Class, int _TimeToLive, string _RText)
		{
			this.Name = _Name;
			this.Type = _Type;
			this.Class = _Class;
			this.TimeToLive = _TimeToLive;
			this.RText = _RText;
		}

		// Token: 0x0600054F RID: 1359 RVA: 0x0002AC80 File Offset: 0x00028E80
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

		// Token: 0x040003B1 RID: 945
		public Classes Class;

		// Token: 0x040003AF RID: 943
		public string Name;

		// Token: 0x040003B3 RID: 947
		public string RText;

		// Token: 0x040003B2 RID: 946
		public int TimeToLive;

		// Token: 0x040003B0 RID: 944
		public Types Type;
	}
}
