using System;

namespace OpenDNS
{
	// Token: 0x02000094 RID: 148
	public class DnsResponse
	{
		// Token: 0x06000548 RID: 1352 RVA: 0x0002AB64 File Offset: 0x00028D64
		public DnsResponse(int ID, bool AA, bool TC, bool RD, bool RA, int RC)
		{
			this._QueryID = ID;
			this._AuthorativeAnswer = AA;
			this._IsTruncated = TC;
			this._RecursionDesired = RD;
			this._RecursionAvailable = RA;
			this._ResponseCode = (ResponseCodes)RC;
			this._ResourceRecords = new ResourceRecordCollection();
			this._Answers = new ResourceRecordCollection();
			this._Authorities = new ResourceRecordCollection();
			this._AdditionalRecords = new ResourceRecordCollection();
		}

		// Token: 0x17000090 RID: 144
		public ResourceRecordCollection AdditionalRecords
		{
			// Token: 0x06000546 RID: 1350 RVA: 0x0002AA14 File Offset: 0x00028C14
			get
			{
				return this._AdditionalRecords;
			}
		}

		// Token: 0x1700008E RID: 142
		public ResourceRecordCollection Answers
		{
			// Token: 0x06000544 RID: 1348 RVA: 0x0002AA04 File Offset: 0x00028C04
			get
			{
				return this._Answers;
			}
		}

		// Token: 0x17000089 RID: 137
		public bool AuthorativeAnswer
		{
			// Token: 0x0600053F RID: 1343 RVA: 0x0002A9DC File Offset: 0x00028BDC
			get
			{
				return this._AuthorativeAnswer;
			}
		}

		// Token: 0x1700008F RID: 143
		public ResourceRecordCollection Authorities
		{
			// Token: 0x06000545 RID: 1349 RVA: 0x0002AA0C File Offset: 0x00028C0C
			get
			{
				return this._Authorities;
			}
		}

		// Token: 0x1700008A RID: 138
		public bool IsTruncated
		{
			// Token: 0x06000540 RID: 1344 RVA: 0x0002A9E4 File Offset: 0x00028BE4
			get
			{
				return this._IsTruncated;
			}
		}

		// Token: 0x17000088 RID: 136
		public int QueryID
		{
			// Token: 0x0600053E RID: 1342 RVA: 0x0002A9D4 File Offset: 0x00028BD4
			get
			{
				return this._QueryID;
			}
		}

		// Token: 0x1700008C RID: 140
		public bool RecursionAvailable
		{
			// Token: 0x06000542 RID: 1346 RVA: 0x0002A9F4 File Offset: 0x00028BF4
			get
			{
				return this._RecursionAvailable;
			}
		}

		// Token: 0x1700008B RID: 139
		public bool RecursionRequested
		{
			// Token: 0x06000541 RID: 1345 RVA: 0x0002A9EC File Offset: 0x00028BEC
			get
			{
				return this._RecursionDesired;
			}
		}

		// Token: 0x17000091 RID: 145
		public ResourceRecordCollection ResourceRecords
		{
			// Token: 0x06000547 RID: 1351 RVA: 0x0002AA1C File Offset: 0x00028C1C
			get
			{
				if (this._ResourceRecords.Count == 0 && this._Answers.Count > 0 && this._Authorities.Count > 0 && this._AdditionalRecords.Count > 0)
				{
					foreach (ResourceRecord value in this.Answers)
					{
						this._ResourceRecords.Add(value);
					}
					foreach (ResourceRecord value2 in this.Authorities)
					{
						this._ResourceRecords.Add(value2);
					}
					foreach (ResourceRecord value3 in this.AdditionalRecords)
					{
						this._ResourceRecords.Add(value3);
					}
				}
				return this._ResourceRecords;
			}
		}

		// Token: 0x1700008D RID: 141
		public ResponseCodes ResponseCode
		{
			// Token: 0x06000543 RID: 1347 RVA: 0x0002A9FC File Offset: 0x00028BFC
			get
			{
				return this._ResponseCode;
			}
		}

		// Token: 0x0400038B RID: 907
		private ResourceRecordCollection _AdditionalRecords;

		// Token: 0x04000389 RID: 905
		private ResourceRecordCollection _Answers;

		// Token: 0x04000383 RID: 899
		private bool _AuthorativeAnswer;

		// Token: 0x0400038A RID: 906
		private ResourceRecordCollection _Authorities;

		// Token: 0x04000384 RID: 900
		private bool _IsTruncated;

		// Token: 0x04000382 RID: 898
		private int _QueryID;

		// Token: 0x04000386 RID: 902
		private bool _RecursionAvailable;

		// Token: 0x04000385 RID: 901
		private bool _RecursionDesired;

		// Token: 0x04000388 RID: 904
		private ResourceRecordCollection _ResourceRecords;

		// Token: 0x04000387 RID: 903
		private ResponseCodes _ResponseCode;
	}
}
