using System;
using System.Net;

namespace OpenDNS
{
	// Token: 0x02000096 RID: 150
	public class Address : ResourceRecord
	{
		// Token: 0x06000541 RID: 1345 RVA: 0x0002B9A9 File Offset: 0x00029BA9
		public Address(string _Name, Types _Type, Classes _Class, int _TimeToLive, string _ResourceAddress) : base(_Name, _Type, _Class, _TimeToLive)
		{
			this.ResourceAddress = _ResourceAddress;
			this.RText = _ResourceAddress;
		}

		// Token: 0x1700008F RID: 143
		public IPAddress IP
		{
			// Token: 0x06000540 RID: 1344 RVA: 0x0002B988 File Offset: 0x00029B88
			get
			{
				if (this._IP == null)
				{
					this._IP = IPAddress.Parse(this.ResourceAddress);
				}
				return this._IP;
			}
		}

		// Token: 0x040003AE RID: 942
		public string ResourceAddress;

		// Token: 0x040003AF RID: 943
		private IPAddress _IP;
	}
}
