using System;

namespace OpenDNS
{
	// Token: 0x02000099 RID: 153
	public class MX : ResourceRecord
	{
		// Token: 0x0600054B RID: 1355 RVA: 0x0002AC0E File Offset: 0x00028E0E
		public MX(string _Name, Types _Type, Classes _Class, int _TimeToLive, int _Preference, string _Exchange) : base(_Name, _Type, _Class, _TimeToLive)
		{
			this.Preference = _Preference;
			this.Exchange = _Exchange;
		}

		// Token: 0x040003AE RID: 942
		public string Exchange;

		// Token: 0x040003AD RID: 941
		public int Preference;
	}
}
