using System;
using System.Net;

namespace OpenDNS
{
	// Token: 0x02000098 RID: 152
	public class Address : ResourceRecord
	{
		// Token: 0x0600054A RID: 1354 RVA: 0x0002ABF1 File Offset: 0x00028DF1
		public Address(string _Name, Types _Type, Classes _Class, int _TimeToLive, string _ResourceAddress) : base(_Name, _Type, _Class, _TimeToLive)
		{
			this.ResourceAddress = _ResourceAddress;
			this.RText = _ResourceAddress;
		}

		// Token: 0x17000092 RID: 146
		public IPAddress IP
		{
			// Token: 0x06000549 RID: 1353 RVA: 0x0002ABD0 File Offset: 0x00028DD0
			get
			{
				if (this._IP == null)
				{
					this._IP = IPAddress.Parse(this.ResourceAddress);
				}
				return this._IP;
			}
		}

		// Token: 0x040003AB RID: 939
		public string ResourceAddress;

		// Token: 0x040003AC RID: 940
		private IPAddress _IP;
	}
}
