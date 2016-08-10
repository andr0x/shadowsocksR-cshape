using System;

namespace OpenDNS
{
	// Token: 0x02000097 RID: 151
	public class MX : ResourceRecord
	{
		// Token: 0x06000542 RID: 1346 RVA: 0x0002B9C6 File Offset: 0x00029BC6
		public MX(string _Name, Types _Type, Classes _Class, int _TimeToLive, int _Preference, string _Exchange) : base(_Name, _Type, _Class, _TimeToLive)
		{
			this.Preference = _Preference;
			this.Exchange = _Exchange;
		}

		// Token: 0x040003B1 RID: 945
		public string Exchange;

		// Token: 0x040003B0 RID: 944
		public int Preference;
	}
}
